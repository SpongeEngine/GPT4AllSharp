using System.Text.Json.Serialization;

namespace SpongeEngine.GPT4AllSharp.Models
{
    public class Model
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = "model";

        [JsonPropertyName("owned_by")]
        public string OwnedBy { get; set; } = string.Empty;

        [JsonPropertyName("permissions")]
        public string[]? Permissions { get; set; }
    }
}