using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    public class LightsStatus
    {
        [JsonPropertyName("numberOfLights")]
        public int NumberOfLights { get; set; }

        [JsonPropertyName("lights")]
        public IList<Lights> Lights { get; set; }

        public string ToJson()
        {
            NumberOfLights = 1;

            string status = JsonSerializer.Serialize<LightsStatus>(this);

            return status;
        }

        public static LightsStatus FromLight(ElgatoLight light)
        {
            var newLight = new Lights
            {
                Brightness = light.Brightness,
                On = light.On,
                Temperature = light.Temperature
            };

            var lights = new List<Lights>
            {
                newLight
            };

            return new LightsStatus
            {
                NumberOfLights = 0,
                Lights = lights
            };                
        }
    }
}
