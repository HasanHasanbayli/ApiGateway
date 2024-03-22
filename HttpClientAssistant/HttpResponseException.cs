using System.Net;

namespace HttpClientAssistant;

/// <summary>
/// Represents an exception that is thrown when an HTTP response is not successful.
/// </summary>
public class HttpResponseException(HttpStatusCode httpStatusCode, string message) : Exception(message)
{
    /// <summary>
    /// Represents the status codes defined for HTTP/1.1.
    /// </summary>
    public readonly HttpStatusCode HttpStatusCode = httpStatusCode;
}