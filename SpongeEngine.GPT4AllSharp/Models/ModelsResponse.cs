using System.Text.Json.Serialization;

namespace SpongeEngine.GPT4AllSharp.Models
{
    public class ModelsResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = "list";

        [JsonPropertyName("data")]
        public Model[] Data { get; set; } = Array.Empty<Model>();
    }
}