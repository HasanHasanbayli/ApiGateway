using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiGateway;

public class HttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResult> Post<T, TResult>(T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default,
        bool shouldThrowException = true)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Post, uri, requestUri, arg, headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<TResult>(response, shouldThrowException: shouldThrowException);
    }

    public async Task<T> Get<T>(string uri, string? requestUri = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default, bool shouldThrowException = true)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Get, uri, requestUri, headers: headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<T>(response, shouldThrowException: shouldThrowException);
    }


    public async Task<TResult> Put<T, TResult>(T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default,
        bool shouldThrowException = true)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Put, uri, requestUri, arg, headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<TResult>(response, shouldThrowException: shouldThrowException);
    }

    public async Task Delete(string uri, string? requestUri = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default, bool shouldThrowException = true)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Delete, uri, requestUri, headers: headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        await HandleResponse(response: response, shouldThrowException: shouldThrowException);
    }

    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string uri, string? requestUri = null,
        object? requestBody = null, Dictionary<string, string>? headers = null)
    {
        HttpRequestMessage request = new(method, !string.IsNullOrEmpty(requestUri) ? requestUri : uri);

        if (headers != null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers.Add(name: header.Key, value: header.Value);
            }
        }

        if (requestBody != null)
        {
            string jsonBody = JsonSerializer.Serialize(requestBody);

            request.Content = new StringContent(jsonBody, Encoding.UTF8, mediaType: "application/json");
        }

        request.Headers.Accept.Add(item: new MediaTypeWithQualityHeaderValue("application/json"));

        return request;
    }

    private async Task HandleResponse(HttpResponseMessage response, bool shouldThrowException = true)
    {
        string content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode) return;

        if (shouldThrowException) throw new Exception(content);
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage httpResponse, bool shouldThrowException = true)
    {
        string content = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode) return JsonSerializer.Deserialize<T>(content)!;

        if (shouldThrowException) throw new Exception(content);

        return JsonSerializer.Deserialize<T>(content)!;
    }
}