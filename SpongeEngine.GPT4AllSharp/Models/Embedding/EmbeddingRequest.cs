using Newtonsoft.Json;

namespace SpongeEngine.GPT4AllSharp.Models.Embedding
{
    public class EmbeddingRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;
    
        [JsonProperty("input")]
        public string Input { get; set; } = string.Empty;
    }

}