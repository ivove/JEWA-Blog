# The File-Blog-Service

This is the point where it is getting interesting, implementing the IBlogService to a concrete implementation using the XML-file storage from the previous post. Once the interface is implemented, only one more step is needed to start seeing the results of the labour. This step would be building a simple interface to display the blog. But that is for a bit later, now focusing on implementing the IBlogService.

The groundwork for implementing the interface is easy, create a class implementing the interface like so:

```
public class FileBlogService:IBlogService
```

and let visual studio create the methods. This will result in a class containing all the methods defined by the interface, all throwing an exception. Not really useful code, but at least the frame is in place. Next thing to do is to build the implementation. This will require some thinking and some decisions.

One thing to realize, using XML-files means reading from disk. This is a slow process, so limiting the times disk IO is needed will improve performance. So how to go about minimizing the number of times the files are read from disk. On way to do this is to implement the interface in such a way, it reads the files only once. So if the FileBlogService would have a field containing a list of posts, this list can be populated in the constructor, disk reads are only going to happen when the constructor is called. This is, useless if the constructor is going to be called on every page request. So there should be some way to only call the constructor once. This can be achieved by registering the FileBlogService as a singleton with the dependency injection container. A detailed overview of dependency injection in ASP.NET core can be found [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0). For now, registering the service as a singleton will kind of make it an in memory database. This will improve performance, but it also means that updating this database isn't just adding files. The files are needed to persist the data if the application restarts, but while the application is running, all data needs to be fetched and written in the list of posts on the FileBlogService class. Adding the field for the list of posts would look like:

```
private List<Post> posts = new List<Post>();
```

In order to avoid some strings being randomly inserted into the code, it would be a good idea to declare some constants. These would then hold the directories containing the XML and markdown files. Declaring these as constants will avoid using these values as stings in the code, increasing maintainability and even readability. in this case two constants are needed, one to hold the foldername for the XML-files, being "posts", and another to hold the foldername for the markdown-files, being "markdown". Another good idea would be to keep two fields holding the full path to the folders containing the XML and markdown-files. All this would look like:

```
private const string POSTS = "posts";

private const string MARKDOWN = "markdown";

private readonly string xmlFolder;

private readonly string markdownFolder;
```

Now the constructor of the class will be responsible to fill the three fields. The fields containing the folder paths are relatively straightforward, the only thing to note is that the constructor will need information from the "web host environment". This is needed to get to the path of the "wwwroot" folder containing the XML-folder. To do this, dependency injection can be leveraged, just adding a parameter to the constructor with type IWebHostEnvironment will make sure the ASP.NET core dependency container resolves this dependency. This opens up a way to interact with the "web host environment". The code to initialize the posts field will be written in a separate method, this will improve readability. At this point the constructor would look something like:

```
public FileBlogService(IWebHostEnvironment env)
{
    if (env is null) { throw new ArgumentNullException(nameof(env)); }
    xmlFolder = Path.Combine(env.WebRootPath,POSTS);
    markdownFolder = Path.Combine(env.WebRootPath,POSTS,MARKDOWN);
    Initialize();
}
```

The Initialize method will be responsible for loading the posts, as in reading the XML and markdown files to populate the posts-field. It might also be a good idea to let the initialize method sort the posts-field, as all posts will most likely be shown in chronological order. Because the sorting of the posts will also be needed when saving a new post, it would be best to extract this to its own method. While doing this, the reading of the posts might as well be done in a separate method. This means the Initialize method will just call these two methods:

```
private void Initialize()
{
    ReadPosts();
    SortPosts();
}
```

The interesting part to implement is the ReadPosts method. But first, to avoid repeating code, it might be wise to implement a method to read a value from an XML-document. Moving on to the reading of the XML-files...

First some housekeeping, checking of the folders exist, and if not creating them. This will ensure proper functioning. After that, looping through the files in the XML-directory will surely not cause any issue.
Foreach XML-file in the XML-folder an new instance of the Post class needs to be created and added to the posts field. While doing this, the properties should be filled with the values from the XML-file. Only exception would be the content of the post, this should be read from the corresponding markdown-file in the markdown-folder. The only special case will be reading the comments, this would best be done in another separate method. For now the code for looping through and reading the XML-files would look like:

```
foreach (var file in Directory.EnumerateFiles(xmlFolder, "*.xml",       SearchOption.TopDirectoryOnly))
{
    var doc = XElement.Load(file);
    var post = new Post
    {
        PostId = ReadXmlValue(doc,"postId"),
        Title = ReadXmlValue(doc, "title"),
        Excerpt = ReadXmlValue(doc, "excerpt"),
        Category = ReadXmlValue(doc, "category"),
        PublicationDate = DateTime.Parse(ReadXmlValue(doc, "publicationDate"), CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal),
        IsPublished = bool.Parse(ReadXmlValue(doc, "isPublished", "true")),
    };
    var markdownFile = Path.Combine(markdownFolder, post.PostId,".md");
    post.Content = File.ReadAllText(markdownFile);
    LoadComments(post,doc);
    posts.Add(post);
}
```

Next the LoadComments method should be implemented. This will consist of reading the comments node in the XML-document and creating a Comments-object for each comment, then adding the comments to the Comments property of the post. As any comment could have a reply, this too should be accounted for.
The code for this might look something like this:

```
private static void LoadComments(Post post,XElement doc)
{
    var comments = doc.Element("comments");

    if (comments is null)
    {
        return;
    }

    foreach (var node in comments.Elements("comment"))
    {
        var comment = new Comment
        {
            CommentId = ReadXmlValue(node, "id"),
            Author = ReadXmlValue(node, "author"),
            Email = ReadXmlValue(node, "email"),
            Content = ReadXmlValue(node, "content"),
            PublicationTime = DateTime.Parse(ReadXmlValue(node, "publicationDate", "2000-01-01 00:00:00"),
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
        };
        LoadReplies(comment,node);
        post.Comments.Add(comment);
    }
}

private static void LoadReplies(Comment comment,XElement doc)
{
    var replies = doc.Element("replies");

    if (replies is null)
    {
        return;
    }

    foreach (var node in replies.Elements("comment"))
    {
        var reply = new Comment
        {
            CommentId = ReadXmlValue(node, "id"),
            Author = ReadXmlValue(node, "author"),
            Email = ReadXmlValue(node, "email"),
            Content = ReadXmlValue(node, "content"),
            ReplyTo = ReadXmlValue(node, "replyTo"),
            PublicationTime = DateTime.Parse(ReadXmlValue(node, "publicationDate", "2000-01-01 00:00:00"),
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
        };
        LoadReplies(reply, node);
        comment.Replies.Add(reply);
    }
}
```

After all this, the post should be loaded in the posts-field of the class. The next step would be implementing the methods getting the data. These are very straightforward, so they won't be discussed here. The methods to add, edit or delete a post will be built later.

Next up, building the interface to see the posts...
