using FluentAssertions;
using SpongeEngine.GPT4AllSharp.Client;
using SpongeEngine.GPT4AllSharp.Models;
using SpongeEngine.GPT4AllSharp.Tests.Common;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.GPT4AllSharp.Tests.Unit.Client
{
    public class Tests : TestBase
    {
        private readonly Gpt4AllClient _client;

        public Tests(ITestOutputHelper output) : base(output)
        {
            _client = new Gpt4AllClient(new ClientOptions
            {
                Port = 4891
            }, Logger);
        }

        [Fact]
        public async Task ListModelsAsync_ShouldReturnModels()
        {
            // Arrange
            var expectedResponse = new ModelsResponse
            {
                Object = "list",
                Data = new[]
                {
                    new Model
                    {
                        Id = "test-model",
                        Object = "model",
                        OwnedBy = "test-publisher"
                    }
                }
            };

            Server
                .Given(Request.Create()
                    .WithPath("/v1/models")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(expectedResponse)));

            // Act
            var response = await _client.ListModelsAsync();

            // Assert
            response.Should().NotBeNull();
            response.Data.Should().HaveCount(1);
            response.Data[0].Id.Should().Be("test-model");
        }

        [Fact]
        public async Task CreateChatCompletionAsync_ShouldReturnValidResponse()
        {
            // Arrange
            var request = new ChatCompletionRequest
            {
                Model = "test-model",
                Messages = new List<ChatMessage>
                {
                    new() { Role = "user", Content = "Hello" }
                },
                Temperature = 0.7f
            };

            var expectedResponse = new ChatCompletionResponse
            {
                Id = "chat-123",
                Object = "chat.completion",
                Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Model = "test-model",
                Choices = new List<ChatCompletionChoice>
                {
                    new()
                    {
                        Index = 0,
                        Message = new ChatMessage
                        {
                            Role = "assistant",
                            Content = "Hello! How can I help you?"
                        },
                        FinishReason = "stop"
                    }
                }
            };

            Server
                .Given(Request.Create()
                    .WithPath("/v1/chat/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(expectedResponse)));

            // Act
            var response = await _client.CreateChatCompletionAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Choices.Should().HaveCount(1);
            response.Choices[0].Message.Content.Should().Be("Hello! How can I help you!");
        }

        [Fact]
        public async Task StreamChatCompletionAsync_ShouldStreamTokens()
        {
            // Arrange
            var request = new ChatCompletionRequest
            {
                Model = "test-model",
                Messages = new List<ChatMessage>
                {
                    new() { Role = "user", Content = "Hello" }
                },
                Temperature = 0.7f,
                Stream = true
            };

            var tokens = new[] { "Hello", " there", "!" };
            var streamResponses = tokens.Select(token =>
                $"data: {{\"choices\": [{{\"delta\": {{\"content\": \"{token}\"}}, \"finish_reason\": null}}]}}\n\n");

            Server
                .Given(Request.Create()
                    .WithPath("/v1/chat/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(string.Join("", streamResponses) + "data: [DONE]\n\n")
                    .WithHeader("Content-Type", "text/event-stream"));

            // Act
            var receivedTokens = new List<string>();
            await foreach (var response in _client.StreamChatCompletionAsync(request))
            {
                if (response.Choices[0].Delta?.Content != null)
                {
                    receivedTokens.Add(response.Choices[0].Delta.Content);
                }
            }

            // Assert
            receivedTokens.Should().BeEquivalentTo(tokens);
        }

        [Fact]
        public async Task ChatCompleteAsync_WithLocalDocs_ShouldIncludeReferences()
        {
            // Arrange
            var request = new ChatCompletionRequest
            {
                Model = "test-model",
                Messages = new List<ChatMessage>
                {
                    new() { Role = "user", Content = "Tell me about APIs" }
                }
            };

            var expectedResponse = new ChatCompletionResponse
            {
                Id = "chat-123",
                Object = "chat.completion",
                Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Model = "test-model",
                Choices = new List<ChatCompletionChoice>
                {
                    new()
                    {
                        Index = 0,
                        Message = new ChatMessage
                        {
                            Role = "assistant",
                            Content = "Based on the documentation..."
                        },
                        FinishReason = "stop",
                        References = new List<Reference>
                        {
                            new()
                            {
                                Text = "The GPT4All Chat Desktop Application comes with a built-in server mode...",
                                Title = "API Documentation",
                                Date = "2024-08-07"
                            }
                        }
                    }
                }
            };

            Server
                .Given(Request.Create()
                    .WithPath("/v1/chat/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(expectedResponse)));

            // Act
            var response = await _client.CreateChatCompletionAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Choices[0].References.Should().NotBeEmpty();
            response.Choices[0].References[0].Text.Should().NotBeEmpty();
            response.Choices[0].References[0].Title.Should().Be("API Documentation");
        }

        [Fact]
        public async Task IsAvailableAsync_WhenServerResponds_ShouldReturnTrue()
        {
            // Arrange
            Server
                .Given(Request.Create()
                    .WithPath("/v1/models")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));

            // Act
            var isAvailable = await _client.ListModelsAsync();

            // Assert
            isAvailable.Should().NotBeNull();
        }

        public override void Dispose()
        {
            _client.Dispose();
            base.Dispose();
        }
    }
}