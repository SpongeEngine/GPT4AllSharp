using LocalAI.NET.GPT4All.Providers;

namespace LocalAI.NET.GPT4All.Models
{
    public class Exception : System.Exception
    {
        public Provider Provider { get; }
        public int? StatusCode { get; }
        public string? ResponseContent { get; }

        public Exception(
            string message,
            Provider provider,
            int? statusCode = null,
            string? responseContent = null) 
            : base(message)
        {
            Provider = provider;
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public Exception(string message, Provider provider) : base(message)
        {
            Provider = provider;
        }

        public Exception(string message, System.Exception innerException, Provider provider) 
            : base(message, innerException)
        {
            Provider = provider;
        }
    }
}