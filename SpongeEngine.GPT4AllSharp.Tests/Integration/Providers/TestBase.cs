using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpongeEngine.GPT4AllSharp.Providers.GPT4AllSharpOpenAiCompatible;
using SpongeEngine.GPT4AllSharp.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.GPT4AllSharp.Tests.Integration.Providers
{
    public abstract class TestBase : IAsyncLifetime
    {
        protected readonly IGpt4AllSharpOpenAiCompatibleProvider CompatibleProvider;
        protected readonly ITestOutputHelper Output;
        protected readonly ILogger Logger;
        protected bool ServerAvailable;

        protected TestBase(ITestOutputHelper output)
        {
            Output = output;
            Logger = LoggerFactory
                .Create(builder => builder.AddXUnit(output))
                .CreateLogger(GetType());

            var httpClient = new HttpClient 
            { 
                BaseAddress = new Uri(TestConfig.OpenAiApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(TestConfig.TimeoutSeconds)
            };

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            CompatibleProvider = new GPT4AllSharp.Providers.GPT4AllSharpOpenAiCompatible.Gpt4AllSharpOpenAiCompatibleProvider(
                httpClient, 
                modelName: "Llama 3.2 3B Instruct",
                logger: Logger, 
                jsonSettings: jsonSettings);
        }

        public async Task InitializeAsync()
        {
            try
            {
                ServerAvailable = await CompatibleProvider.IsAvailableAsync();
                if (ServerAvailable)
                {
                    Output.WriteLine("OpenAI API endpoint is available");
                }
                else
                {
                    Output.WriteLine("OpenAI API endpoint is not available");
                    throw new SkipException("OpenAI API endpoint is not available");
                }
            }
            catch (Exception ex) when (ex is not SkipException)
            {
                Output.WriteLine($"Failed to connect to OpenAI API endpoint: {ex.Message}");
                throw new SkipException("Failed to connect to OpenAI API endpoint");
            }
        }

        public Task DisposeAsync()
        {
            if (CompatibleProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            return Task.CompletedTask;
        }

        private class SkipException : Exception
        {
            public SkipException(string message) : base(message) { }
        }
    }
}