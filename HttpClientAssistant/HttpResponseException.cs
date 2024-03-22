using System.Net;

namespace HttpClientAssistant;

public class HttpResponseException(HttpStatusCode httpStatusCode, string message) : Exception(message)
{
    public readonly HttpStatusCode HttpStatusCode = httpStatusCode;
}