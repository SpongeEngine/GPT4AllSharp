using LocalAI.NET.GPT4All.Models.Chat;
using LocalAI.NET.GPT4All.Models.Completion;
using LocalAI.NET.GPT4All.Models.Embedding;
using LocalAI.NET.GPT4All.Models.Model;

namespace LocalAI.NET.GPT4All.Providers.Native
{
    public interface INativeProvider : IDisposable
    {
        Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default);
        Task<Model> GetModelAsync(string modelId, CancellationToken cancellationToken = default);
        Task<CompletionResponse> CompleteAsync(CompletionRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamCompletionAsync(CompletionRequest request, CancellationToken cancellationToken = default);
        Task<ChatResponse> ChatCompleteAsync(ChatRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
        Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}