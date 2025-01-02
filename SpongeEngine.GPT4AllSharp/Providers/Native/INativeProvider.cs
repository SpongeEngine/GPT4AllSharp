using SpongeEngine.GPT4AllSharp.Models.Chat;
using SpongeEngine.GPT4AllSharp.Models.Completion;
using SpongeEngine.GPT4AllSharp.Models.Embedding;
using SpongeEngine.GPT4AllSharp.Models.Model;

namespace SpongeEngine.GPT4AllSharp.Providers.Native
{
    public interface INativeProvider : IDisposable
    {
        Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default);
        Task<Gpt4AllSharpModel> GetModelAsync(string modelId, CancellationToken cancellationToken = default);
        Task<CompletionResponse> CompleteAsync(CompletionRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamCompletionAsync(CompletionRequest request, CancellationToken cancellationToken = default);
        Task<ChatResponse> ChatCompleteAsync(ChatRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
        Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}