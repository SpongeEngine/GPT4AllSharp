using FluentAssertions;
using SpongeEngine.GPT4AllSharp.Providers.GPT4AllSharpOpenAiCompatible;
using SpongeEngine.GPT4AllSharp.Tests.Common;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.GPT4AllSharp.Tests.Unit.Providers.GPT4AllSharpOpenAiCompatible
{
    public class Gpt4AllSharpOpenAiCompatibleProviderTests : TestBase
    {
        private readonly IGpt4AllSharpOpenAiCompatibleProvider _provider;
        private readonly HttpClient _httpClient;
        private const string CompletionsEndpoint = "/v1/completions";
        private const string ModelsEndpoint = "/v1/models";

        public Gpt4AllSharpOpenAiCompatibleProviderTests(ITestOutputHelper output) : base(output)
        {
            _httpClient = CreateHttpClient();
            _provider = CreateProvider();
        }

        private HttpClient CreateHttpClient() => 
            new() { BaseAddress = new Uri(BaseUrl) };

        private IGpt4AllSharpOpenAiCompatibleProvider CreateProvider() => 
            new Gpt4AllSharpOpenAiCompatibleProvider(_httpClient, logger: Logger);

        [Fact]
        public async Task CompleteAsync_WithValidPrompt_ReturnsExpectedResponse()
        {
            // Arrange
            const string expectedResponse = "Test response";
            SetupCompletionEndpoint(expectedResponse);

            // Act
            var response = await _provider.CompleteAsync("Test prompt");

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task StreamCompletionAsync_WithValidPrompt_StreamsTokensCorrectly()
        {
            // Arrange
            var tokens = new[] { "Hello", " world", "!" };
            SetupStreamingEndpoint(tokens);

            // Act
            var receivedTokens = await CollectStreamedTokens("Test prompt");

            // Assert
            receivedTokens.Should().BeEquivalentTo(tokens);
        }

        [Fact]
        public async Task IsAvailableAsync_WhenServerResponds_ReturnsTrue()
        {
            // Arrange
            SetupModelsEndpoint();

            // Act
            var isAvailable = await _provider.IsAvailableAsync();

            // Assert
            isAvailable.Should().BeTrue();
        }

        private void SetupCompletionEndpoint(string expectedResponse)
        {
            Server
                .Given(Request.Create()
                    .WithPath(CompletionsEndpoint)
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody($"{{\"choices\": [{{\"text\": \"{expectedResponse}\"}}]}}"));
        }

        private void SetupStreamingEndpoint(string[] tokens)
        {
            var streamResponses = tokens.Select(token => 
                $"data: {{\"choices\": [{{\"delta\": {{\"content\": \"{token}\"}}, \"finish_reason\": null}}]}}\n\n");

            Server
                .Given(Request.Create()
                    .WithPath(CompletionsEndpoint)
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(string.Join("", streamResponses) + "data: [DONE]\n\n")
                    .WithHeader("Content-Type", "text/event-stream"));
        }

        private void SetupModelsEndpoint()
        {
            Server
                .Given(Request.Create()
                    .WithPath(ModelsEndpoint)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        private async Task<List<string>> CollectStreamedTokens(string prompt)
        {
            var receivedTokens = new List<string>();
            await foreach (var token in _provider.StreamCompletionAsync(prompt))
            {
                receivedTokens.Add(token);
            }
            return receivedTokens;
        }

        public override void Dispose()
        {
            _httpClient.Dispose();
            base.Dispose();
        }
    }
}