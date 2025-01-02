namespace SpongeEngine.GPT4AllSharp.Tests.Common
{
    public static class TestConfig
    {
        private const string DefaultHost = "http://localhost:4891";

        public static string NativeApiBaseUrl => 
            Environment.GetEnvironmentVariable("GPT4ALL_BASE_URL") ?? $"{DefaultHost}/api";

        public static string OpenAiApiBaseUrl => 
            Environment.GetEnvironmentVariable("GPT4ALL_OPENAI_BASE_URL") ?? $"{DefaultHost}/v1";
            
        // Extended timeout for large models
        public static int TimeoutSeconds => 120;
    }
}