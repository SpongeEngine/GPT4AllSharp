using LocalAI.NET.GPT4All.Models.Base;
using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Embedding
{
    public class EmbeddingResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; } = "list";
    
        [JsonProperty("data")]
        public List<EmbeddingData> Data { get; set; } = new();
    
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;
    
        [JsonProperty("usage")]
        public Usage Usage { get; set; } = new();
    
        public class EmbeddingData
        {
            [JsonProperty("object")]
            public string Object { get; set; } = "embedding";
        
            [JsonProperty("embedding")]
            public float[] Embedding { get; set; } = Array.Empty<float>();
        
            [JsonProperty("index")]
            public int Index { get; set; }
        }
    }
}