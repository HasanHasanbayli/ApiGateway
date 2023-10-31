using System.Net;

namespace ApiGateway;

public class HttpResponseException : Exception
{
    public HttpStatusCode HttpStatusCode { get; private set; }

    public HttpResponseException(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }
}