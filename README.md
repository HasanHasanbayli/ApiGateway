# HttpClientAssistant Documentation

## Overview
The `HttpClientAssistant` class is designed to simplify making HTTP requests within .NET applications, abstracting the complexities involved in setting up and sending requests using the `HttpClient` class. It provides a flexible interface for sending HTTP requests with various configurations, including HTTP method selection, URI configuration, headers, multi-part form data, and handling of request and response errors. This class supports asynchronous operations and allows for optional cancellation of requests.

## Features
- **Simplified HTTP Request Operations**: Offers a streamlined approach to configure and send HTTP requests.
- **Generic Request and Response Handling**: Supports sending requests with or without a request body and receiving responses into a specified type.
- **Error Handling**: Provides options to throw exceptions on request or response failures, allowing for custom error handling strategies.
- **Cancellation Support**: Integrates with the .NET `CancellationToken` for cancelling ongoing HTTP requests.

## Usage

### Sending a Request Without a Response
This method is suitable for HTTP requests where no response body is expected or required. It supports sending data as a part of the request body, form data, or without any data.

```csharp
public static async Task SendAsync<TRequest>(
    HttpMethod httpMethod,
    string uri,
    TRequest? requestBody = default,
    string? requestUri = null,
    Dictionary<string, string>? headers = null,
    List<KeyValuePair<string, string>>? multiPartFormData = null,
    bool throwOnRequest = true,
    bool throwOnResponse = true,
    CancellationToken cancellationToken = default) where TRequest : class
```

### Sending a Request and Receiving a Typed Response
This method is used when the HTTP request expects a response. It deserializes the response body into the specified type, `TResult`.

```csharp
public static async Task<TResult?> SendAsync<TRequest, TResult>(
    HttpMethod httpMethod,
    string uri,
    TRequest? requestBody = default,
    string? requestUri = null,
    Dictionary<string, string>? headers = null,
    List<KeyValuePair<string, string>>? multiPartFormData = null,
    bool throwOnRequest = true,
    bool throwOnResponse = true,
    CancellationToken cancellationToken = default) where TRequest : class
```

## Parameters
- **httpMethod**: The HTTP method to be used for the request (e.g., GET, POST).
- **uri**: The base URI for the HTTP request.
- **requestUri**: An optional parameter that specifies the relative URI to append to the base URI.
- **headers**: Optional headers to include in the request.
- **multiPartFormData**: Optional multipart form data for the request.
- **throwOnRequest**: Indicates whether to throw an exception on request errors.
- **throwOnResponse**: Indicates whether to throw an exception on response errors.
- **cancellationToken**: Optional cancellation token to cancel the request.

## Examples

### Sending a GET Request
```csharp
await HttpClientAssistant.SendAsync<object, MyResponseType>(
    HttpMethod.Get,
    "https://api.example.com/data",
    null,
    throwOnResponse: false);
```

### Sending a POST Request with JSON Body
```csharp
var myData = new { Name = "John Doe", Age = 30 };
await HttpClientAssistant.SendAsync(
    HttpMethod.Post,
    "https://api.example.com/users",
    myData);
```

## Conclusion
The `HttpClientAssistant` class offers a robust and flexible way to handle HTTP requests in .NET, making it easier to work with external APIs and services by abstracting some of the lower-level plumbing required by the `HttpClient` class. Its design encourages a cleaner, more maintainable codebase by reducing redundancy and promoting reusability.