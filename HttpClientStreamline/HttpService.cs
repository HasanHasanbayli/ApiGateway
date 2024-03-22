using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HttpClientStreamline;

public class HttpService
{
    public static async Task Send(HttpMethod httpMethod, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, requestBody: null,
            headers, multiPartFormData);

        await SendAsync(httpRequestMessage, shouldRequestThrow, shouldResponseThrow, cancellationToken);
    }

    public static async Task Send<T>(HttpMethod httpMethod, T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, arg,
            headers, multiPartFormData);

        await SendAsync(httpRequestMessage, shouldRequestThrow, shouldResponseThrow, cancellationToken);
    }

    public static async Task<TResult?> Send<TResult>(HttpMethod httpMethod, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, requestBody: null,
            headers, multiPartFormData);

        return await SendAsync<TResult>(httpRequestMessage, shouldRequestThrow, shouldResponseThrow, cancellationToken);
    }

    public static async Task<TResult?> Send<T, TResult>(HttpMethod httpMethod, T arg, string uri,
        string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri, requestUri, arg,
            headers, multiPartFormData);

        return await SendAsync<TResult>(httpRequestMessage, shouldRequestThrow, shouldResponseThrow, cancellationToken);
    }

    private static HttpRequestMessage CreateHttpRequest(HttpMethod method, string uri, string? requestUri = null,
        object? requestBody = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormData = null)
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

    private static void FillHeaders(HttpRequestMessage httpRequest, Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> header in headers)
        {
            httpRequest.Headers.Add(name: header.Key, header.Value);
        }
    }

    private static async Task SendAsync(HttpRequestMessage httpRequestMessage, bool shouldRequestThrow = true,
        bool shouldResponseThrow = true, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response =
            await SendHttpRequestAsync(httpRequestMessage, shouldRequestThrow, cancellationToken);

        if (response is null || response.IsSuccessStatusCode) return;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken);

        if (shouldResponseThrow)
        {
            throw new HttpResponseException(response.StatusCode, responseData);
        }
    }

    private static async Task<TResult?> SendAsync<TResult>(HttpRequestMessage httpRequestMessage,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response =
            await SendHttpRequestAsync(httpRequestMessage, shouldRequestThrow, cancellationToken);

        if (response is null) return default;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TResult>(responseData);
        }

        if (shouldResponseThrow)
        {
            throw new HttpResponseException(response.StatusCode, responseData);
        }

        return default;
    }

    private static async Task<HttpResponseMessage?> SendHttpRequestAsync(HttpRequestMessage httpRequestMessage,
        bool shouldRequestThrow, CancellationToken cancellationToken)
    {
        try
        {
            using HttpClient httpClient = new();

            return await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            if (shouldRequestThrow) throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (shouldRequestThrow) throw;
        }

        return default;
    }
}