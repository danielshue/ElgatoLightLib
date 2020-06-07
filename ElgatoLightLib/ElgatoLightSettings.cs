using System.Text.Json.Serialization;

namespace ElgatoLightLib
{
    public class ElgatoLightSettings
    {
        /// <exclude />
        [JsonPropertyName("powerOnBehavior")]
        public int PowerOnBehavior { get; set; }

        /// <exclude />
        [JsonPropertyName("powerOnBrightness")]
        public int PowerOnBrightness { get; set; }

        /// <exclude />
        [JsonPropertyName("powerOnTemperature")]
        public int PowerOnTemperature { get; set; }

        /// <exclude />
        [JsonPropertyName("switchOnDurationMs")]
        public int SwitchOnDurationMs { get; set; }

        /// <exclude />
        [JsonPropertyName("switchOffDurationMs")]
        public int SwitchOffDurationMs { get; set; }

        /// <exclude />
        [JsonPropertyName("colorChangeDurationMs")]
        public int ColorChangeDurationMs { get; set; }

    }
}
