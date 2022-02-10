# Frontend

Building a frontend essentially consists of two parts. The first part is building a UI, in this example that will be razor-views. The code for these views is mainly HTML with some added razor-syntax. The second part is building the controllers, these controllers will supply the data for the views and make sure the correct razor-view is displayed.

## The UI

As the visual studio template for a .NET core web application already contains bootstrap, the easiest way to start is to build a simple bootstrap based UI. For more info about bootstrap visit [get bootstrap](http://getbootstrap.com). The big advantage of using bootstrap is that the resulting site will be responsive out-off-the-box, meaning it will work on al screen sizes (provided some considerations are made).

So for the main page, that is the page displaying a number of posts, there will be a version for smaller screens and a version for bigger screens. The version for small screens will show a list of posts (title, publication date and excerpt) in a wider column, and a list of categories in a smaller column. On the smaller screens, the list of categories will be shown first, followed by the list of posts.

For the post page, this is a page with the full body of the post, The title, publication date and content will be shown, followed by the comments. In the comments section, some care should be taken to visualize the replies differently.

Using the layout provided by the template allows for a relatively easy start, as no work needs to be done on the general look and feel. Although it might be wise to invest some time to personalize this layout.

## The controller

For the blog to actually display something, a controller is needed. In this case the BlogController will be created under the "Controllers" folder. In the most basic case, for the blog, this controller will have 2 methods, the Index method and the Post method. The default routing mechanism will the route all requests for /blog to the index method, while all requests for /post will be handled by the post method. additional parameters can be passed is. For the post method, an id parameter (holding the id of the post to show) will be passed in. For the index method, an optional parameter holding the page number will be needed. The default routing will make sure the id parameter on the post method gets mapped correctly when using /post/_postid_.
For the page number, a custom route will be defined, using an attribute on the method like:

```
    [Route("/blog/{page:int?}")]
    public IActionResult Index([FromRoute]int page = 0)
```

Note the "FromRoute" attribute decorating the parameter, this is to make sure the parameter is mapped correctly.

The controller will also have a constructor accepting a parameter of type IBlogService. This is done to take advantage of the dependency injection, if the concrete implementation of the IBlogService is correctly registered with the dependency injection container, the code in the controller will be able to use all methods defined in the IBlogService. This will make the implementation of the methods really straightforward.

Note that in this case the concrete implementation for the IBlogService is the FileBlogService which will be registered with the dependency injection container as a singleton (this ensures one instance of the class will always be reused, only constructing the instance once at application startup). This is done like so:

```
builder.Services.AddSingleton<IBlogService, FileBlogService>();
```

## Comments

One aspect of the post page is the possibility to leave a comment, or to reply to a comment. This can be done using some simple javascript and a bootstrap model containing a form. The form submit would then trigger the save of the comment or reply and refresh the page. The only distinction between a comment and a reply would be that the reply contains a reference to the already existing comment. In the modal it might be nice, when replying to show the original comment.

## Noticing things and changing accordingly

While working on the UI, it is not at all exceptional to notice some things are missing, or are implemented a bit strange. This section will detail some of these quirks and the solution implemented.

### Markdown

In order to display the markdown content correctly, a markdown interpreter is needed. Fortunately there is no need to write this, as there are options available. For this project Westwind.AspNetCore.Markdown was selected, this was installed through NuGet. After installation a change to the \_ViewImports.cshtml file is needed in order to use the available taghelper. Two lines need to be added:

```
@using Westwind.AspNetCore.Markdown

@addTagHelper *, Westwind.AspNetCore.Markdown
```

After that, in the view it is possible to wrap the markdown content in markdown-tags. This will make sure the markdown code is displayed properly.

```
<markdown>@Model.Content)</markdown>
```

### counting the posts

As the list of posts can be filtered by category, the need quickly arises to count the posts per category. This was not implemented in the IBlogService, and subsequently not implemented in the FileBlogService.
Adjusting for this means a change in the IBlogService and in the FileBlogService. after changing the IBlogService, the definition for the GetPostCount looks like:

```
int GetPostCount(string category ="",bool published  = true);
```
