﻿using Newtonsoft.Json;

namespace LocalAI.NET.GPT4All.Models.Base
{
    public class ModelInfo
    {
        [JsonProperty("arch")]
        public string Architecture { get; set; } = string.Empty;
    
        [JsonProperty("quant")]
        public string Quantization { get; set; } = string.Empty;
    
        [JsonProperty("format")]
        public string Format { get; set; } = string.Empty;
    
        [JsonProperty("context_length")]
        public int ContextLength { get; set; }
    }

}