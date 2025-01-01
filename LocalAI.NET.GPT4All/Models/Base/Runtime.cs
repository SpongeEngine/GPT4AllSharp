using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Base
{
    public class Runtime
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    
        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;
    
        [JsonProperty("supported_formats")]
        public List<string> SupportedFormats { get; set; } = new();
    }
}