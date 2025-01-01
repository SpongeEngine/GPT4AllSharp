using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Chat
{
    public class ChatMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
    
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
}