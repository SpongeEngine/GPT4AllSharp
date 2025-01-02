namespace SpongeEngine.GPT4AllSharp.Models
{
    public class ClientOptions
    {
        /// <summary>
        /// The port number for the GPT4All API server. Default is 4891.
        /// </summary>
        public int Port { get; set; } = 4891;

        /// <summary>
        /// HTTP request timeout in seconds. Default is 600 seconds (10 minutes).
        /// </summary>
        public int TimeoutSeconds { get; set; } = 600;
    }
}