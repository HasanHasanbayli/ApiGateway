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

    public async Task Send(HttpMethod httpMethod, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormsData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri: uri, requestUri: requestUri,
            headers: headers, multiPartFormsData: multiPartFormsData);

        await SendAsync(httpRequestMessage: httpRequestMessage, shouldRequestThrow: shouldRequestThrow,
            shouldResponseThrow: shouldResponseThrow, cancellationToken: cancellationToken);
    }

    public async Task Send<T>(HttpMethod httpMethod, T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormsData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri: uri, requestUri: requestUri, arg,
            headers: headers, multiPartFormsData: multiPartFormsData);

        await SendAsync(httpRequestMessage: httpRequestMessage, shouldRequestThrow: shouldRequestThrow,
            shouldResponseThrow: shouldResponseThrow, cancellationToken: cancellationToken);
    }

    public async Task<TResult?> Send<TResult>(HttpMethod httpMethod, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormsData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri: uri, requestUri: requestUri,
            headers: headers, multiPartFormsData: multiPartFormsData);

        return await SendAsync<TResult>(httpRequestMessage: httpRequestMessage, shouldRequestThrow: shouldRequestThrow,
            shouldResponseThrow: shouldResponseThrow, cancellationToken: cancellationToken);
    }

    public async Task<TResult?> Send<T, TResult>(HttpMethod httpMethod, T arg, string uri, string? requestUri = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormsData = null,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true,
        CancellationToken cancellationToken = default)
    {
        HttpRequestMessage httpRequestMessage = CreateHttpRequest(httpMethod, uri: uri, requestUri: requestUri, arg,
            headers: headers, multiPartFormsData: multiPartFormsData);

        return await SendAsync<TResult>(httpRequestMessage: httpRequestMessage, shouldRequestThrow: shouldRequestThrow,
            shouldResponseThrow: shouldResponseThrow, cancellationToken: cancellationToken);
    }

    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string uri, string? requestUri = null,
        object? requestBody = null,
        Dictionary<string, string>? headers = null,
        List<KeyValuePair<string, string>>? multiPartFormsData = null)
    {
        HttpRequestMessage httpRequest = new(method, !string.IsNullOrEmpty(requestUri) ? uri + requestUri : uri);

        if (headers != null)
        {
            FillHeaders(httpRequest: httpRequest, headers: headers);
        }

        if (multiPartFormsData != null)
        {
            FillMultiPartContent(httpRequest: httpRequest, multiPartFormData: multiPartFormsData);
        }

        if (requestBody != null)
        {
            string jsonBody = JsonSerializer.Serialize(requestBody);

            httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, mediaType: "application/json");
        }

        httpRequest.Headers.Accept.Add(item: new MediaTypeWithQualityHeaderValue("application/json"));

        return httpRequest;
    }

    private void FillMultiPartContent(HttpRequestMessage httpRequest,
        List<KeyValuePair<string, string>> multiPartFormData)
    {
        MultipartFormDataContent multiPartContent = new();

        foreach (KeyValuePair<string, string> item in multiPartFormData)
        {
            multiPartContent.Add(new StringContent(item.Value), item.Key);
        }

        httpRequest.Content = multiPartContent;
    }

    private void FillHeaders(HttpRequestMessage httpRequest, Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> header in headers)
        {
            httpRequest.Headers.Add(name: header.Key, header.Value);
        }
    }

    private async Task SendAsync(HttpRequestMessage httpRequestMessage, bool shouldRequestThrow = true,
        bool shouldResponseThrow = true, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response = await SendHttpRequestAsync(httpRequestMessage: httpRequestMessage,
            cancellationToken: cancellationToken, shouldRequestThrow: shouldRequestThrow);

        if (response is null) return;

        if (response.IsSuccessStatusCode) return;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);

        if (shouldResponseThrow)
        {
            throw new HttpResponseException(message: responseData, httpStatusCode: response.StatusCode);
        }
    }

    private async Task<TResult?> SendAsync<TResult>(HttpRequestMessage httpRequestMessage,
        bool shouldRequestThrow = true, bool shouldResponseThrow = true, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response = await SendHttpRequestAsync(httpRequestMessage: httpRequestMessage,
            cancellationToken: cancellationToken, shouldRequestThrow: shouldRequestThrow);

        if (response is null) return default;

        string responseData = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<TResult>(json: responseData);
        }

        if (shouldResponseThrow)
        {
            throw new HttpResponseException(message: responseData, httpStatusCode: response.StatusCode);
        }

        return default;
    }

    private async Task<HttpResponseMessage?> SendHttpRequestAsync(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken, bool shouldRequestThrow)
    {
        try
        {
            return await _httpClient.SendAsync(request: httpRequestMessage, cancellationToken: cancellationToken);
        }
        catch (HttpRequestException)
        {
            if (shouldRequestThrow) throw;
        }
        catch (Exception)
        {
            if (shouldRequestThrow) throw;
        }

        return default;
    }
}