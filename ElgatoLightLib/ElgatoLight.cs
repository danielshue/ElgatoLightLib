using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ElgatoLightLib
{
    /// <summary>
    /// Class Library for the Elgato Light
    /// </summary>
    public class ElgatoLight
    {
        private const string StatusInfoEndPointTemplate = @"http://{0}:{1}/elgato/lights";
        private const string SettingsEndPointTemplate = @"http://{0}:{1}/elgato/lights/settings";

        public const int MinimumTemperature = 143;
        public const int MaximumTemperature = 344;
        public const int DefaultTemperature = 213;

        public const int MinKelvin = 2900;
        public const int MaxKelvin = 7000;
        public const int WhiteRangeKelvin = 4100;

        public const int MinimumBrightness = 3;
        public const int MaximumBrightness = 100;
        public const int HalfBrightness = 50;

        public const bool DefaultOn = true;
        public const double DefaultHue = 31.0;
        public const double DefaultSaturation = 33.0;
        public const int DefaultBrightness = 20;
        public const int DefaultDurationSwitchOnMs = 100;
        public const int DefaultDurationSwitchOffMs = 300;
        public const int DefaultsDurationColorChangeMs = 100;
        public const int DefaultDefaultPWMHz = 300;

        /// <exclude />
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        /// <exclude />
        [JsonPropertyName("hardwareBoardType")]
        public int HardwareBoardType { get; set; }

        /// <exclude />
        [JsonPropertyName("firmwareBuildNumber")]
        public int FirmwareBuildNumber { get; set; }

        /// <exclude />
        [JsonPropertyName("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <exclude />
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <exclude />
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        /// <exclude />
        [JsonPropertyName("features")]
        public string[] Features { get; set; }

        /// <exclude />
        public string Address { get; set; }

        /// <exclude />
        public int Port { get; set; }

        /// <exclude />
        public string Name { get; set; }

        /// <exclude />
        [JsonPropertyName("on")]
        public int On { get; set; }

        /// <exclude />
        [JsonPropertyName("brightness")]
        public int Brightness { get; set; }

        /// <exclude />
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        public ElgatoLightSettings Settings { get; set; }

        /// <summary>
        /// Enum value of Elgato Lights
        /// </summary>
        /// <seealso cref="HardwareBoardType"/>
        public ElgatoLightType LightType
        {
            get
            {
                return (ElgatoLightType)HardwareBoardType;
            }
        }

        /// <exclude />
        public string EndPoint
        {
            get
            {
                return $"http://{Address}:{Port}/elgato/lights";
            }
        }

        public bool IsOn
        {
            get
            {
                if (On == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task OnAsync()
        {
            await UpdateStatusAsync();

            Debug.WriteLine($"OnAsync Old Values: {ToString()}");

            var targetStatus = LightsStatus.FromLight(this);

            targetStatus.Lights[0].On = 1;

            var targetStatusJson = targetStatus.ToJson();

            var returnedJsonStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(returnedJsonStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(returnedJsonStatus);

            await UpdateSettings();

            Debug.WriteLine($"OnAsync New Values: {ToString()}");
        }

        public async Task OffAsync()
        {
            await UpdateStatusAsync();

            Debug.WriteLine($"OffAsync Old Values: {ToString()}");

            // extract the current settings 
            var targetSettings = LightsStatus.FromLight(this);

            targetSettings.Lights[0].On = 0;

            // convert the current status to json
            var targetStatusJson = targetSettings.ToJson();

            var returnedJsonStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(returnedJsonStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(returnedJsonStatus);

            await UpdateSettings();

            Debug.WriteLine($"OffAsync New Values: {ToString()}");
        }


        public async Task IncreaseBrightnessAsync(int amount)
        {
            await SetBrightnessAsync(Brightness + amount);
        }

        public async Task DecreaseBrightnessAsync(int amount)
        {
            await SetBrightnessAsync(Brightness - amount);
        }

        /// <summary>
        /// Sets the light to a specific brightness level of <see cref="MinimumBrightness"/> and <see cref="MaximumBrightness"/>.
        /// </summary>
        /// <param name="level">The level amount to set the brightness.</param>
        /// <exception cref="ElgatoLightNotOnException">Thrown if trying to set the brightness and the device isn't turned on.</exception>
        /// <exception cref="ElgatoLightOutOfRangeException">Thrown if the new value is out of range. The minimum range is <see cref="MinimumBrightness"/> and maximum brightness is <see cref="MaximumBrightness"/>.</exception>
        public async Task SetBrightnessAsync(int level)
        {
            if (IsOn == false)
            {
                throw new ElgatoLightNotOnException("The light must be turned On in order to turn the set the brightness.");
            }

            await UpdateStatusAsync();

            Debug.WriteLine($"SetBrightnessAsync Old Values: {ToString()}");

            if (level < MinimumBrightness || level > MaximumBrightness)
            {
                throw new ElgatoLightOutOfRangeException($"Brightness Out of Range {level}. The minimum range is {MinimumBrightness} and the Maximum is {MaximumBrightness}.");
            }

            var targetStatus = LightsStatus.FromLight(this);

            targetStatus.Lights[0].Brightness = level;

            var targetStatusJson = targetStatus.ToJson();

            var retreturnedJsonStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(retreturnedJsonStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(retreturnedJsonStatus);

            Debug.WriteLine($"SetBrightnessAsync New Values: {ToString()}");
        }

        /// <summary>
        ///  Must be 2900-7000
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        /// <exception cref="ElgatoLightNotOnException">Thrown if trying to set the brightness and the device isn't turned on.</exception>
        /// <exception cref="ElgatoLightOutOfRangeException">Thrown if the new value is out of range. The minimum range is <see cref="MinimumTemperature"/> and maximum tempature is <see cref="MaximumTemperature"/>.</exception>
        public async Task SetColorTemperatureAsync(int temp)
        {
            if (IsOn == false)
            {
                throw new ElgatoLightNotOnException("The light must be turned On in order to turn the set the tempature.");
            }

            await UpdateStatusAsync();

            Debug.WriteLine($"SetColorTemperatureAsync Old Values: {ToString()}");

            if (temp < MinimumTemperature || temp > MaximumTemperature)
            {
                throw new ElgatoLightOutOfRangeException($"Temperature Out of Range. {temp} is an invlaid temperature");
            }

            var targetStatusStatus = LightsStatus.FromLight(this);

            targetStatusStatus.Lights[0].Temperature = temp;

            var targetStatusJson = targetStatusStatus.ToJson();

            var retreturnedJsonStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(retreturnedJsonStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(retreturnedJsonStatus);

            Debug.WriteLine($"SetColorTemperatureAsync New Values: {ToString()}");
        }

        public async Task IncreaseColorTemperatureAsync(int amount)
        {
            await SetColorTemperatureAsync(Temperature + amount);
        }

        public async Task DecreaseColorTemperatureAsync(int amount)
        {
            await SetColorTemperatureAsync(Temperature - amount);
        }

        public override string ToString()
        {
            return $@"Elgato Light {SerialNumber} @ {Address}:{Port}\nOn: \t\t{IsOn}\nBrightness:\t{Brightness}\nTemperature:\t{ Temperature }";
        }

        internal async Task UpdateStatusAsync()
        {
            var statusEndPoint = string.Format(StatusInfoEndPointTemplate, Address, Port);

            if (NetworkDiscoveryHelper.IsNetworkAvailable() == false)
            {
                return;
            }

            using (var client = new HttpClient())
            {
                var streamTask = client.GetStreamAsync(statusEndPoint);

                LightsStatus status = await JsonSerializer.DeserializeAsync<LightsStatus>(await streamTask);

                if (status != null && status.Lights != null && status.Lights.Count > 0)
                {
                    On = status.Lights[0].On;
                    Brightness = status.Lights[0].Brightness;
                    Temperature = status.Lights[0].Temperature;
                }

            }
        }

        internal async Task UpdateSettings()
        {
            var settingEndPoint = string.Format(SettingsEndPointTemplate, Address, Port);

            using (var client = new HttpClient())
            {
                var streamTask = client.GetStreamAsync(settingEndPoint);

                var settings = await JsonSerializer.DeserializeAsync<ElgatoLightSettings>(await streamTask);

                Settings = settings;
            }
        }

        internal async Task ParseLightStatus(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace", nameof(json));
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                var status = await JsonSerializer.DeserializeAsync<LightsStatus>(stream);

                if (status != null && status.Lights != null && status.Lights.Count > 0)
                {
                    On = status.Lights[0].On;
                    Brightness = status.Lights[0].Brightness;
                    Temperature = status.Lights[0].Temperature;
                }
            }
        }

        private async Task<string> UpdateDevice(string lightStatusJson)
        {
            if (string.IsNullOrWhiteSpace(lightStatusJson))
            {
                throw new ArgumentException($"'{nameof(lightStatusJson)}' cannot be null or whitespace", nameof(lightStatusJson));
            }

            return await UpdateDevice(EndPoint, lightStatusJson);
        }

        private async Task<string> UpdateDevice(string endPoint, string lightStatusJson)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentException($"'{nameof(endPoint)}' cannot be null or whitespace", nameof(endPoint));
            }

            if (string.IsNullOrWhiteSpace(lightStatusJson))
            {
                throw new ArgumentException($"'{nameof(lightStatusJson)}' cannot be null or whitespace", nameof(lightStatusJson));
            }

            if (NetworkDiscoveryHelper.IsNetworkAvailable() == false)
            {
                return string.Empty;
            }

            using (var client = new HttpClient())
            {
                using (var content = new StringContent(lightStatusJson, Encoding.UTF8))
                {
                    content.Headers.Remove("Content-Type");
                    content.Headers.Add("Content-Type", "application/json");

                    using (var req = new HttpRequestMessage(HttpMethod.Put, endPoint))
                    {
                        req.Content = content;

                        // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
                        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                        using (HttpResponseMessage resp = await client.SendAsync(req))
                        {
                            resp.EnsureSuccessStatusCode();

                            return await resp.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
        }
    }
}
