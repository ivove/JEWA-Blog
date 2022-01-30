# Data access

When thinking about accessing data, what is really important is to define some sort of "contract" to get to the data. In essence what is needed is some way to get from stored data to populated models. To make a solution a bit future proof, it is always a good idea to try and make some abstraction of the data source. This is done to make it possible to change the data source later on.

In the case of this blog, the initial idea is to start with data stored in files on disc. However if the blog grows very large, it might get necessary to move the data to a database system. If all data access is hardcoded, this might present a challenge and a lot of coding.
To avoid this situation interfaces can be used. An interface represents, in a way, a contract. An interface can define a bunch of method signatures, without the implementation of these methods. If the interface is implemented, all methods defined in the interface should be implemented.
A small example:

There is an interface IBookList containing a method signature GetAllBooks. There is a class BookListFile which implements the IBookList interface, reading a file on disc containing a set of books (one on each line). Some other method might need to get a list of books, so the method can accept a parameter of Type IBookList, and when the method gets called the caller passes an object of Type BookListFile. If later a connection to a SQL-server instance is needed, an implementation of the interface can be built to read data from a SQL-server. Then all one needs to do is pass en instance of the new implementation.

All of the above, combined with dependency injection, will make for highly adaptable code. This is true even for smaller projects. The fact that in ASP.NET core dependency injection is built-in, makes it real easy to use.

## The interface

The interface, this should be a definition of all methods needed to access the data. This is not depending on any knowledge of how the data will be fetched. Al that needs to be done is to think about what methods might be needed to access the data. Giving this some thought is essential, forgetting something won't hurt, but the interface might need to evolve. In a new project, this is no big deal, in en existing project, this might be an issue if implementations exist. Any change in the interface will mean changes in the implementations.

For now, working on a brand new project, nothing to worry about. Thinking about what methods are needed, two areas become visible:

- reading data
- adding/updating/deleting data

To take the second point first. Adding and updating a post might be implemented by one method taking as a parameter the post. Deleting a post would be another method also taking the post as a parameter. If it is possible to add/edit a post, and delete a post, all other actions will be reading data.

On the point of reading data, it might get a bit trickier to decide what methods are needed. It helps to reflect a bit on each method, just to determine what parameters might be needed or handy. Reflecting on the methods for the blog:

There should be a method that allows the fetching of all categories. This should return a list of catagories, no input parameters are needed. At one point it will be necessary to allow visitors to filter the blog on category, why would categories be implemented if not for this? This means it will be required to fetch a list of categories in order to let the visitor pick.

Getting a post by its ID... Sooner or later it will be necessary to fetch a post by its ID. This method will return a post, preferably nullable so null can be returned if there is no post found. Th method should take the ID as an input parameter, the type might vary, but in the case of this blog it should be a string as the ID field for the model is a string.

Users will not remember the ID of a blog post, and for SEO purposes it is always a good idea to build urls with the title for the post, something like /blog/TheBasics . This allows for better search engine results. in order to achieve this, a method to get a post by its title will come in handy. This method would take the title as a parameter.

On the homepage, showing a list of all posts is a fairly common thing to do. This would call for a method to get all posts. Now if the blog gets a lot of posts, it might not be the best idea to really show all posts on the homepage. A pagination system might be a good idea. This would then mean there is a need to provide the number of posts to be returned. But a need to know how many posts need to be skipped is also needed. Thinking a bit further, it would come in handy to have a way to ask this method only to return posts of a certain category. All this would result in a method accepting a count parameter, to know how many posts to return, a skip parameter, how many posts should be skipped, and a category parameter defaulting to empty, in order to filter.

In summary, these methods are needed:

- SavePost(Post post) -> returning nothing
- DeletePost(Post post) -> returning nothing
- GetCategories() -> returning a list of categories
- GetPostById(string id) -> returning a Post or null
- GetPostByTitle(string title) -> returning a Post or null
- GetPosts(int count,int skip=0,string category = string.Empty) -> returning a list of Posts
