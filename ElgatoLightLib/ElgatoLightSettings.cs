using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    /// <summary>
    /// ElgatoLightSettings, represents the settings for an Elgato light. It contains properties that define 
    /// various aspects of the light's behavior, such as power on behavior, brightness, temperature, 
    /// switch on duration, switch off duration, and color change duration. Each property is annotated 
    /// with the [JsonPropertyName] attribute, which specifies the corresponding JSON property name when 
    /// serializing or deserializing the object. This class serves as a data model for storing and 
    /// manipulating the settings of an Elgato light.
    /// </summary>
    public class ElgatoLightSettings
    {
        /// <summary>
        /// Power On Behavior Value
        /// </summary>
        [JsonPropertyName("powerOnBehavior")]
        public int PowerOnBehavior { get; set; }

        /// <summary>
        /// Power On Brightness Value
        /// </summary>
        [JsonPropertyName("powerOnBrightness")]
        public int PowerOnBrightness { get; set; }

        /// <summary>
        /// Power On Temperature Value.
        /// </summary>
        [JsonPropertyName("powerOnTemperature")]
        public int PowerOnTemperature { get; set; }

        /// <summary>
        /// Switch On Duration in Milliseconds
        /// </summary>
        [JsonPropertyName("switchOnDurationMs")]
        public int SwitchOnDurationMs { get; set; }

        /// <summary>
        /// SwitchO ff Duration in Milliseconds.
        /// </summary>
        [JsonPropertyName("switchOffDurationMs")]
        public int SwitchOffDurationMs { get; set; }

        /// <summary>
        /// Color Change Duration in Milliseconds.
        /// </summary>
        [JsonPropertyName("colorChangeDurationMs")]
        public int ColorChangeDurationMs { get; set; }

    }
}
