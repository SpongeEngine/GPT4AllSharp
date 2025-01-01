using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Model
{
    public class ModelsResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; } = "list";
    
        [JsonProperty("data")]
        public List<Model> Data { get; set; } = new();
    }
}