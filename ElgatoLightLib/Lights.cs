using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    public class Lights
    {
        [JsonPropertyName("on")] 
        public int On { get; set; }

        [JsonPropertyName("brightness")] 
        public int Brightness { get; set; }

        [JsonPropertyName("temperature")] 
        public int Temperature { get; set; }
    }
}
