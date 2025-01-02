using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SpongeEngine.GPT4AllSharp.Models;

namespace SpongeEngine.GPT4AllSharp.Client
{
    public class Gpt4AllClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _logger;
        private bool _disposed;

        public Gpt4AllClient(ClientOptions options, ILogger? logger = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            _logger = logger;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://localhost:{options.Port}/v1/"),
                Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Lists all available models.
        /// GET /v1/models
        /// </summary>
        public async Task<ModelsResponse> ListModelsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ModelsResponse>("models", cancellationToken);
                return response ?? new ModelsResponse { Data = Array.Empty<Model>() };
            }
            catch (Exception ex) when (ex is not Gpt4AllException)
            {
                throw new Gpt4AllException("Failed to list models", ex);
            }
        }

        /// <summary>
        /// Gets details about a specific model.
        /// GET /v1/models/{name}
        /// </summary>
        public async Task<Model> GetModelAsync(string modelName, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Model>($"models/{modelName}", cancellationToken);
                return response ?? throw new Gpt4AllException($"Model {modelName} not found");
            }
            catch (Exception ex) when (ex is not Gpt4AllException)
            {
                throw new Gpt4AllException($"Failed to get model {modelName}", ex);
            }
        }

        /// <summary>
        /// Creates a completion for the provided prompt.
        /// POST /v1/completions
        /// </summary>
        public async Task<CompletionResponse> CreateCompletionAsync(
            CompletionRequest request,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<CompletionResponse>("completions", request, cancellationToken);
        }

        /// <summary>
        /// Creates a chat completion for the provided messages.
        /// POST /v1/chat/completions
        /// </summary>
        public async Task<ChatCompletionResponse> CreateChatCompletionAsync(
            ChatCompletionRequest request,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<ChatCompletionResponse>("chat/completions", request, cancellationToken);
        }

        /// <summary>
        /// Streams a chat completion for the provided messages.
        /// POST /v1/chat/completions with stream=true
        /// </summary>
        public async IAsyncEnumerable<ChatCompletionResponse> StreamChatCompletionAsync(
            ChatCompletionRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            request.Stream = true;

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json")
            };

            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

            using var response = await _httpClient.SendAsync(
                httpRequest, 
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            await EnsureSuccessStatusCodeAsync(response);

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;
                if (!line.StartsWith("data: ")) continue;

                var data = line[6..];
                if (data == "[DONE]") break;

                ChatCompletionResponse? completionResponse = null;

                try
                {
                    completionResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(data);
                }
                catch (JsonException ex)
                {
                    _logger?.LogWarning(ex, "Failed to parse SSE message: {Message}", data);
                    continue;
                }

                if (completionResponse != null)
                {
                    yield return completionResponse;
                }
            }
        }

        private async Task<T> SendRequestAsync<T>(string endpoint, object request, CancellationToken cancellationToken)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
                await EnsureSuccessStatusCodeAsync(response);

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<T>(responseContent);
                return result ?? throw new Gpt4AllException("Null response from API");
            }
            catch (Exception ex) when (ex is not Gpt4AllException)
            {
                throw new Gpt4AllException($"API request to {endpoint} failed", ex);
            }
        }

        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Gpt4AllException(
                    $"API request failed with status {response.StatusCode}: {content}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
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