﻿using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpongeEngine.GPT4AllSharp.Models.Base;
using SpongeEngine.GPT4AllSharp.Models.Chat;
using SpongeEngine.GPT4AllSharp.Models.Completion;
using SpongeEngine.GPT4AllSharp.Models.Embedding;
using SpongeEngine.GPT4AllSharp.Models.Model;
using SpongeEngine.GPT4AllSharp.Providers.LocalAI;
using SpongeEngine.GPT4AllSharp.Providers.Native;

namespace SpongeEngine.GPT4AllSharp.Client
{
    public class Gpt4AllClient : IDisposable
    {
        private readonly INativeProvider? _nativeProvider;
        private readonly ILocalAiProvider? _openAiProvider;
        private readonly Options _options;
        private bool _disposed;

        public string Name { get; set; }
        public string? Version { get; private set; }
        public bool SupportsStreaming => true;

        public Gpt4AllClient(Options options, ILogger? logger = null, JsonSerializerSettings? jsonSettings = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            };

            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(options.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            }

            if (options.UseOpenAiApi)
            {
                _openAiProvider = new LocalAiProvider(httpClient, logger: logger, jsonSettings: jsonSettings);
            }
            else
            {
                _nativeProvider = new NativeProvider(httpClient, logger: logger, jsonSettings: jsonSettings);
            }
        }

        /// <summary>
        /// Lists all loaded and downloaded models.
        /// GET /v1/models
        /// </summary>
        public Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.ListModelsAsync(cancellationToken);
        }

        /// <summary>
        /// Gets info about a specific model.
        /// GET /v1/models/{model}
        /// </summary>
        public Task<Gpt4AllSharpModel> GetModelAsync(string modelId, CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.GetModelAsync(modelId, cancellationToken);
        }

        /// <summary>
        /// Text Completions API. Provides a prompt and receives a completion.
        /// POST /v1/completions
        /// </summary>
        public Task<CompletionResponse> CompleteAsync(
            CompletionRequest request, 
            CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.CompleteAsync(request, cancellationToken);
        }

        /// <summary>
        /// Streaming Text Completions API.
        /// POST /v1/completions with stream=true
        /// </summary>
        public IAsyncEnumerable<string> StreamCompletionAsync(
            CompletionRequest request, 
            CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.StreamCompletionAsync(request, cancellationToken);
        }

        /// <summary>
        /// Chat Completions API. Provides messages array and receives assistant response.
        /// POST /v1/chat/completions
        /// </summary>
        public Task<ChatResponse> ChatCompleteAsync(
            ChatRequest request, 
            CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.ChatCompleteAsync(request, cancellationToken);
        }

        /// <summary>
        /// Streaming Chat Completions API.
        /// POST /v1/chat/completions with stream=true
        /// </summary>
        public IAsyncEnumerable<string> StreamChatAsync(
            ChatRequest request, 
            CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.StreamChatAsync(request, cancellationToken);
        }

        /// <summary>
        /// Text Embeddings API. Provides text and receives embedding vector.
        /// POST /v1/embeddings
        /// </summary>
        public Task<EmbeddingResponse> CreateEmbeddingAsync(
            EmbeddingRequest request, 
            CancellationToken cancellationToken = default)
        {
            EnsureNativeProvider();
            return _nativeProvider!.CreateEmbeddingAsync(request, cancellationToken);
        }

        // OpenAI API methods
        public Task<string> CompleteWithOpenAiAsync(
            string prompt, 
            CompletionOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            EnsureOpenAiProvider();
            return _openAiProvider!.CompleteAsync(prompt, options, cancellationToken);
        }

        public IAsyncEnumerable<string> StreamCompletionWithOpenAiAsync(
            string prompt, 
            CompletionOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            EnsureOpenAiProvider();
            return _openAiProvider!.StreamCompletionAsync(prompt, options, cancellationToken);
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            return _options.UseOpenAiApi 
                ? await _openAiProvider!.IsAvailableAsync(cancellationToken)
                : await _nativeProvider!.IsAvailableAsync(cancellationToken);
        }

        private void EnsureNativeProvider()
        {
            if (_nativeProvider == null)
                throw new InvalidOperationException("Native API is not enabled. Set UseOpenAiApi to false in options.");
        }

        private void EnsureOpenAiProvider()
        {
            if (_openAiProvider == null)
                throw new InvalidOperationException("OpenAI API is not enabled. Set UseOpenAiApi to true in options.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _nativeProvider?.Dispose();
                    _openAiProvider?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}