using Newtonsoft.Json;

namespace SpongeEngine.GPT4AllSharp.Models.Chat
{
    public class ChatMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
    
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
}