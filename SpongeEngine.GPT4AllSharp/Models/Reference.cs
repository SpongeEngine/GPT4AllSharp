using System.Text.Json.Serialization;

namespace SpongeEngine.GPT4AllSharp.Models
{
    public class Reference
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}