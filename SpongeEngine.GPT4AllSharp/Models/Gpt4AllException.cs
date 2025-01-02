namespace SpongeEngine.GPT4AllSharp.Models
{
    public class Gpt4AllException : Exception
    {
        public int? StatusCode { get; }
        public string? ResponseContent { get; }

        public Gpt4AllException(string message) 
            : base(message)
        {
        }

        public Gpt4AllException(
            string message,
            int? statusCode = null,
            string? responseContent = null) 
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public Gpt4AllException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}