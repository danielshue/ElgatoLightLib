using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    /// <summary>
    ///  LightsStatus, represents the status of a group of lights. 
    /// </summary>
    public class LightsStatus
    {
        /// <summary>
        /// An integer property that represents the total number of lights in the group.
        /// </summary>
        [JsonPropertyName("numberOfLights")]
        public int NumberOfLights { get; set; }

        /// <summary>
        ///  A list of Lights objects that represents the individual lights in the group.
        /// </summary>
        [JsonPropertyName("lights")]
        public IList<Lights> Lights { get; set; }

        /// <summary>
        /// This method creates a new LightsStatus object based on a single ElgatoLight object. 
        /// It creates a new Lights object using the properties of the ElgatoLight object, then adds it 
        /// to a list of Lights objects. Finally, it returns 
        /// </summary>
        /// <returns>A new LightsStatus object with NumberOfLights set to 0 and the list of lights.</returns>
        public string ToJson()
        {
            NumberOfLights = 1;

            string status = JsonSerializer.Serialize<LightsStatus>(this);

            return status;
        }

        /// <summary>
        /// FromLight, is a static method of the LightsStatus class. It creates a new LightsStatus object based on a single ElgatoLight object.
        /// </summary>
        /// <param name="light">It takes an ElgatoLight object as a parameter.</param>
        /// <returns>It creates a new LightsStatus object called newLight and initializes its properties 
        /// (Brightness, On, and Temperature) with the corresponding properties of the ElgatoLight object.
        /// </returns>
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
                NumberOfLights = 1, // Corrected from 0 to 1
                Lights = lights
            };
        }
    }
}
