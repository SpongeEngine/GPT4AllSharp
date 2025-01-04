namespace SpongeEngine.GPT4AllSharp.Providers.GPT4AllSharpOpenAiCompatible
{
    public interface IGpt4AllSharpOpenAiCompatibleProvider : IDisposable
    {
        Task<string> CompleteAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default);
        // Streaming not yet supported by the server, as per https://github.com/nomic-ai/gpt4all/blob/c7d734518818be946e40ec44644b8b098dd557ab/gpt4all-chat/src/server.cpp
        // IAsyncEnumerable<string> StreamCompletionAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }

    public class CompletionOptions
    {
        public string? ModelName { get; set; }
        public int? MaxTokens { get; set; }
        public float? Temperature { get; set; }
        public float? TopP { get; set; }
        
        // Stop sequences are not supported as per Streaming not yet supported by the server, as per https://github.com/nomic-ai/gpt4all/blob/c7d734518818be946e40ec44644b8b098dd557ab/gpt4all-chat/src/server.cpp
        // public string[]? StopSequences { get; set; }
    }
}