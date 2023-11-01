using ApiGateway;

const string uri = "https://jsonplaceholder.typicode.com";

// Creating an instance of HttpService with HttpClient

// Posting HTTP requests using HttpService
var post = new {title = "foo", body = "bar", userId = 1};
object createdPost = (await HttpService.Send<object, object>(HttpMethod.Post, post, uri + "/posts"))!;

Console.WriteLine($"Created post: {createdPost}");

// Getting HTTP requests using HttpService
var getPosts = await HttpService.Send<IEnumerable<object>>(HttpMethod.Get, uri + "/posts");
Console.WriteLine("All posts:");

foreach (object item in getPosts!) Console.WriteLine(item);

// Putting HTTP requests using HttpService
var put = new {id = 1, title = "foo", body = "bar", userId = 1};
object updatedPost = (await HttpService.Send<object, object>(HttpMethod.Put, put, uri + "/posts/1"))!;
Console.WriteLine($"Updated post: {updatedPost}");

// Deleting HTTP requests using HttpService
await HttpService.Send(HttpMethod.Delete, uri + "/posts/1");

Console.ReadLine();