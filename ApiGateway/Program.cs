using ApiGateway;

const string uri = "https://jsonplaceholder.typicode.com";

// Creating an instance of HttpService with HttpClient
using HttpClient client = new();
HttpService httpService = new(client);

// Posting HTTP requests using HttpService
var post = new {title = "foo", body = "bar", userId = 1};
object createdPost = (await httpService.Send<object, object>(HttpMethod.Post, post, uri + "/posts"))!;

Console.WriteLine($"Created post: {createdPost}");

// Getting HTTP requests using HttpService
var getPosts = await httpService.Send<IEnumerable<object>>(HttpMethod.Get, uri + "/posts");
Console.WriteLine("All posts:");

foreach (object item in getPosts!) Console.WriteLine(item);

// Putting HTTP requests using HttpService
var put = new {id = 1, title = "foo", body = "bar", userId = 1};
object updatedPost = (await httpService.Send<object, object>(HttpMethod.Put, put, uri + "/posts/1"))!;
Console.WriteLine($"Updated post: {updatedPost}");

// Deleting HTTP requests using HttpService
await httpService.Send(HttpMethod.Delete, uri + "/posts/1");

Console.ReadLine();