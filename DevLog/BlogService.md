# The File-Blog-Service

This is the point where it is getting interesting, implementing the IBlogService to a concrete implementation using the XML-file storage from the previous post. Once the interface is implemented, only one more step is needed to start seeing the results of the labour. This step would be building a simple interface to display the blog. But that is for a bit later, now focusing on implementing the IBlogService.

The groundwork for implementing the interface is easy, create a class implementing the interface like so:

```
public class FileBlogService:IBlogService
```

and let visual studio create the methods. This will result in a class containing all the methods defined by the interface, all throwing an exception. Not really useful code, but at least the frame is in place. Next thing to do is to build the implementation. This will require some thinking and some decisions.

One thing to realize, using XML-files means reading from disk. This is a slow process, so limiting the times disk IO is needed will improve performance. So how to go about minimizing the number of times the files are read from disk. On way to do this is to implement the interface in such a way, it reads the files only once. So if the FileBlogService would have a property containing a list of posts, this list can be populated in the constructor, disk reads are only going to happen when the constructor is called. This is, useless if the constructor is going to be called on every page request. So there should be some way to only call the constructor once. This can be achieved by registering the FileBlogService as a singleton with the dependency injection container. A detailed overview of dependency injection in ASP.NET core can be found [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0). For now, registering the service as a singleton will kind of make it an in memory database. This will improve performance, but it also means that updating this database isn't just adding files. The files are needed to persist the data if the application restarts, but while the application is running, all data needs to be fetched and written in the list of posts on the FileBlogService class.
