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
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Post, uri, requestUri, arg, headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<TResult>(response);
    }

    public async Task<T> Get<T>(string uri, string? requestUri = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Get, uri, requestUri, headers: headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<T>(response);
    }


    public async Task<TResult> Put<T, TResult>(T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null, CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Put, uri, requestUri, arg, headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<TResult>(response);
    }

    public async Task Delete(string uri, string? requestUri = null, Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage request = CreateHttpRequest(HttpMethod.Delete, uri, requestUri, headers: headers);

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

        await HandleResponse(response);
    }

    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string uri, string? requestUri = null,
        object? requestBody = null, Dictionary<string, string>? headers = null)
    {
        HttpRequestMessage request = new(method, !string.IsNullOrEmpty(requestUri) ? requestUri : uri);

        if (headers != null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers.Add(name: header.Key, header.Value);
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

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            throw new Exception(errorBody);
        }
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            throw new Exception(errorBody);
        }

        string responseBody = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(responseBody)!;
    }
}