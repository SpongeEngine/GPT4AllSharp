using Newtonsoft.Json;

namespace SpongeEngine.GPT4AllSharp.Models.Model
{
    public class ModelsResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; } = "list";
    
        [JsonProperty("data")]
        public List<SpongeEngine.GPT4AllSharp.Models.Model.Gpt4AllSharpModel> Data { get; set; } = new();
    }
}