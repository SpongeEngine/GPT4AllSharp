using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Embedding
{
    public class EmbeddingRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;
    
        [JsonProperty("input")]
        public string Input { get; set; } = string.Empty;
    }

}