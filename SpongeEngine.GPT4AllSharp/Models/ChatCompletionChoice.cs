using System.Text.Json.Serialization;

using System.Text.Json.Serialization;

namespace SpongeEngine.GPT4AllSharp.Models
{
    public class ChatCompletionChoice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public ChatMessage Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("references")]
        public List<Reference>? References { get; set; }

        [JsonPropertyName("delta")]
        public ChatMessage? Delta { get; set; }
    }
}