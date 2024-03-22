using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HttpClientAssistant;

/// <summary>
/// The HttpClientAssistant class provides methods for making HTTP requests using the HttpClient class.
/// </summary>
public class HttpClientAssistant
{
    /// <summary>
    /// Sends an HTTP request using the specified HTTP method, URI, and optional request data.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to use for the request.</param>
    /// <param name="uri">The base URI for the request.</param>
    /// <param name="requestUri">The optional relative URI for the request.</param>
    /// <param name="headers">The optional headers to include in the request.</param>
    /// <param name="multiPartFormData">The optional Multi-Part Form Data content for the request.</param>
    /// <param name="throwOnRequest">Indicates whether to throw an exception on request errors (default: true).</param>
    /// <param name="throwOnResponse">Indicates whether to throw an exception on response errors (default: true).</param>
    /// <param name="cancellationToken">The optional cancellation token to cancel the request.</param>
    public static async Task Send(HttpMethod httpMethod, string uri, string? requestUri = default,
        Dictionary<string, string>? headers = default,
        List<KeyValuePair<string, string>>? multiPartFormData = default,
        bool throwOnRequest = true, bool throwOnResponse = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest<>(httpMethod, uri, requestUri, requestBody: null,
            headers, multiPartFormData);

        await SendAsync(httpRequestMessage, throwOnRequest, throwOnResponse, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request using the provided parameters.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to use.</param>
    /// <param name="arg"></param>
    /// <param name="uri">The base URI for the request.</param>
    /// <param name="requestUri">The optional request URI to append to the base URI.</param>
    /// <param name="headers">The optional headers to include in the request.</param>
    /// <param name="multiPartFormData">The optional multi-part form data to include in the request.</param>
    /// <param name="throwOnRequest">Whether to throw an exception on request failure. Default is true.</param>
    /// <param name="throwOnResponse">Whether to throw an exception on response failure. Default is true.</param>
    /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Send<T>(HttpMethod httpMethod, T arg, string uri, string? requestUri = default,
        Dictionary<string, string>? headers = default,
        List<KeyValuePair<string, string>>? multiPartFormData = default,
        bool throwOnRequest = true, bool throwOnResponse = true,
        CancellationToken cancellationToken = default) where T : class
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, arg,
            headers, multiPartFormData);

        await SendAsync(httpRequestMessage, throwOnRequest, throwOnResponse, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request using the specified method, URI, headers, and optional request body.
    /// </summary>
    /// <param name="httpMethod">The HTTP method to use for the request.</param>
    /// <param name="uri">The base URI for the request.</param>
    /// <param name="requestUri">The relative URI for the request.</param>
    /// <param name="headers">Optional headers to include in the request.</param>
    /// <param name="multiPartFormData">Optional multipart form data to include in the request.</param>
    /// <param name="throwOnRequest">Indicates whether to throw an exception on request failure. Default is true.</param>
    /// <param name="throwOnResponse">Indicates whether to throw an exception on response failure. Default is true.</param>
    /// <param name="cancellationToken">Optional cancellation token for cancelling the request.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task<TResult?> Send<TResult>(HttpMethod httpMethod, string uri, string? requestUri = default,
        Dictionary<string, string>? headers = default,
        List<KeyValuePair<string, string>>? multiPartFormData = default,
        bool throwOnRequest = true, bool throwOnResponse = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest<>(httpMethod, uri, requestUri, requestBody: null,
            headers, multiPartFormData);

        return await SendAsync<TResult>(httpRequestMessage, throwOnRequest, throwOnResponse, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request using the specified HTTP method and URI.
    /// </summary>
    /// <typeparam name="T">The type of the argument to include in the request.</typeparam>
    /// <typeparam name="TResult">The type of the response result.</typeparam>
    /// <param name="httpMethod">The HTTP method to use for the request.</param>
    /// <param name="arg">The argument to include in the request.</param>
    /// <param name="uri">The base URI for the request.</param>
    /// <param name="requestUri">The relative URI for the request (optional). Default is null.</param>
    /// <param name="headers">The headers to add to the request (optional). Default is null.</param>
    /// <param name="multiPartFormData">The multipart form data to add to the request (optional). Default is null.</param>
    /// <param name="throwOnRequest">Indicates whether to throw an exception on request failure (optional). Default is true.</param>
    /// <param name="throwOnResponse">Indicates whether to throw an exception on response failure (optional). Default is true.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the request (optional). Default is default.</param>
    /// <returns>A task representing the asynchronous send operation and an optional response result.</returns>
    public static async Task<TResult?> Send<T, TResult>(HttpMethod httpMethod, T arg, string uri,
        string? requestUri = default,
        Dictionary<string, string>? headers = default,
        List<KeyValuePair<string, string>>? multiPartFormData = default,
        bool throwOnRequest = true, bool throwOnResponse = true,
        CancellationToken cancellationToken = default) where T : class
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, arg,
            headers, multiPartFormData);

        return await SendAsync<TResult>(httpRequestMessage, throwOnRequest, throwOnResponse, cancellationToken);
    }

    /// <summary>
    /// Creates an HttpRequestMessage object with the specified parameters.
    /// </summary>
    /// <param name="method">The HTTP method to be used in the request.</param>
    /// <param name="uri">The base URI of the request.</param>
    /// <param name="requestUri">The optional request URI to be appended to the base URI.</param>
    /// <param name="requestBody">The optional request body object to be serialized as JSON.</param>
    /// <param name="headers">The optional headers to be added to the request.</param>
    /// <param name="multiPartFormData">The optional multipart form data to be added to the request.</param>
    /// <returns>An HttpRequestMessage object constructed with the specified parameters.</returns>
    private static HttpRequestMessage CreateHttpRequest<T>(HttpMethod method, string uri, string? requestUri = default,
        T? requestBody = default,
        Dictionary<string, string>? headers = default,
        List<KeyValuePair<string, string>>? multiPartFormData = default)
    {
        string fullRequestUri = !string.IsNullOrEmpty(requestUri)
            ? uri + requestUri
            : uri;

        HttpRequestMessage httpRequest = new(method, fullRequestUri);

        if (headers != null)
        {
            FillHeaders(httpRequest, headers);
        }

        if (multiPartFormData != null)
        {
            FillMultiPartContent(httpRequest, multiPartFormData);
        }

        if (requestBody != null)
        {
            string jsonBody = JsonSerializer.Serialize(requestBody);

            httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, mediaType: "application/json");
        }

        httpRequest.Headers.Accept.Add(item: new MediaTypeWithQualityHeaderValue("application/json"));

        return httpRequest;
    }

    /// <summary>
    /// Fills the HTTP request message's content with multi-part form data.
    /// </summary>
    /// <param name="httpRequest">The HTTP request message to fill.</param>
    /// <param name="multiPartFormData">The list of key-value pairs representing the multi-part form data.</param>
    private static void FillMultiPartContent(HttpRequestMessage httpRequest,
        List<KeyValuePair<string, string>> multiPartFormData)
    {
        MultipartFormDataContent multiPartContent = new();

        foreach (KeyValuePair<string, string> item in multiPartFormData)
        {
            multiPartContent.Add(new StringContent(item.Value), item.Key);
        }

        httpRequest.Content = multiPartContent;
    }

    /// <summary>
    /// Fills the headers of the given <see cref="HttpRequestMessage"/> with the specified headers.
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequestMessage"/> to fill the headers for.</param>
    /// <param name="headers">The dictionary containing the headers to be added.</param>
    private static void FillHeaders(HttpRequestMessage httpRequest, Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> header in headers)
        {
            httpRequest.Headers.Add(name: header.Key, header.Value);
        }
    }

    /// <summary>
    /// Sends an HTTP request asynchronously.
    /// </summary>
    /// <param name="httpRequestMessage">The HttpRequestMessage object representing the HTTP request.</param>
    /// <param name="throwOnRequest">A boolean value indicating whether to throw an exception on a request error. The default is true.</param>
    /// <param name="throwOnResponse">A boolean value indicating whether to throw an exception on a response error. The default is true.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    private static async Task SendAsync(HttpRequestMessage httpRequestMessage, bool throwOnRequest = true,
        bool throwOnResponse = true, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response =
            await SendHttpRequestAsync(httpRequestMessage, throwOnRequest, cancellationToken);

        if (response is null || response.IsSuccessStatusCode) return;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken);

        if (throwOnResponse)
        {
            throw new HttpResponseException(response.StatusCode, responseData);
        }
    }

    /// <summary>
    /// Sends an HTTP request asynchronously and returns the deserialized response.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to deserialize the response into.</typeparam>
    /// <param name="httpRequestMessage">The HTTP request message to send.</param>
    /// <param name="throwOnRequest">Indicates whether to throw an exception on request errors. Default is true.</param>
    /// <param name="throwOnResponse">Indicates whether to throw an exception on response errors. Default is true.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the async operation. Default is CancellationToken.None.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized response of type TResult.
    /// If the request or response is unsuccessful, an exception is thrown depending on the values of throwOnRequest and throwOnResponse.
    /// </returns>
    private static async Task<TResult?> SendAsync<TResult>(HttpRequestMessage httpRequestMessage,
        bool throwOnRequest = true, bool throwOnResponse = true,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response =
            await SendHttpRequestAsync(httpRequestMessage, throwOnRequest, cancellationToken);

        if (response is null) return default;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TResult>(responseData);
        }

        if (throwOnResponse)
        {
            throw new HttpResponseException(response.StatusCode, responseData);
        }

        return default;
    }

    /// <summary>
    /// Sends an HTTP request asynchronously.
    /// </summary>
    /// <param name="httpRequestMessage">The HttpRequestMessage object containing the request details.</param>
    /// <param name="throwOnRequest">Indicates whether an exception should be thrown if there is an error in the request. Default is true.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request operation. Default is CancellationToken.None.</param>
    /// <returns>Returns a Task that represents the asynchronous operation. The task result is the HttpResponseMessage object received from the request.</returns>
    private static async Task<HttpResponseMessage?> SendHttpRequestAsync(HttpRequestMessage httpRequestMessage,
        bool throwOnRequest, CancellationToken cancellationToken)
    {
        try
        {
            using HttpClient httpClient = new();

            return await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch (HttpRequestException)
        {
            if (throwOnRequest) throw;
        }
        catch (Exception)
        {
            if (throwOnRequest) throw;
        }

        return default;
    }
}