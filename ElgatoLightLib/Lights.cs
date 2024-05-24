using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    /// <summary>
    /// The Lights class represents the Lights value in a JSON payload.
    /// and is used to deserialize JSON data into an object.
    /// </summary>
    public class Lights
    {
        /// <summary>
        /// On property is of type int and represents the state of the lights (whether they are on or off). 
        /// </summary>
        [JsonPropertyName("on")] 
        public int On { get; set; }

        /// <summary>
        /// Brightness property represents the brightness level of the lights.
        /// </summary>
        [JsonPropertyName("brightness")] 
        public int Brightness { get; set; }

        /// <summary>
        /// Temperature property represents the temperature of the lights.
        /// </summary>
        [JsonPropertyName("temperature")] 
        public int Temperature { get; set; }
    }
}
