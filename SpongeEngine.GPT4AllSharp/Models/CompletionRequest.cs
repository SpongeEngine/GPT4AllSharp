using System.Text.Json.Serialization;

namespace SpongeEngine.GPT4AllSharp.Models
{
    public class CompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.7f;

        [JsonPropertyName("top_p")]
        public float TopP { get; set; } = 1.0f;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("stop")]
        public string[]? Stop { get; set; }
    }
}