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
    /// Class Library for the Elgato Light.
    /// </summary>
    public class ElgatoLight
    {
        /// <summary>
        /// Endpoint template used for getting the status of the Elgato Light.
        /// </summary>
        private const string StatusInfoEndPointTemplate = @"http://{0}:{1}/elgato/lights";

        /// <summary>
        /// Endpoint template used for getting the settings of the Elgato Light.
        /// </summary>
        private const string SettingsEndPointTemplate = @"http://{0}:{1}/elgato/lights/settings";

        /// <summary>
        /// Minimum Temperature value for the Elgato Light.
        /// </summary>
        public const int MinimumTemperature = 143;

        /// <summary>
        /// Maximum Temperature value for the Elgato Light.
        /// </summary>
        public const int MaximumTemperature = 344;

        /// <summary>
        /// Default Temperature value for the Elgato Light.
        /// </summary>
        public const int DefaultTemperature = 213;

        /// <summary>
        /// Minimum Kelvin value for the Elgato Light.
        /// </summary>
        public const int MinKelvin = 2900;

        /// <Maximum >
        /// Default Kelvin value for the Elgato Light.
        /// </summary>
        public const int MaxKelvin = 7000;

        /// <summary>
        /// Default values for the Elgato Light
        /// </summary>
        public const int WhiteRangeKelvin = 4100;

        /// <summary>
        /// Minimum Brightness value for the Elgato Light.
        /// </summary>
        public const int MinimumBrightness = 3;

        /// <summary>
        /// Maximum Brightness value for the Elgato Light.
        public const int MaximumBrightness = 100;

        /// <summary>
        /// Half Brightness value for the Elgato Light.
        public const int HalfBrightness = 50;

        /// <summary>
        /// Default to turn the device On for the Elgato Light.
        /// </summary>
        public const bool DefaultOn = true;

        /// <summary>
        /// Default Hue Value for the Elgato Light.
        /// </summary>
        public const double DefaultHue = 31.0;

        /// <summary>
        /// Default Saturation Value for the Elgato Light.
        /// </summary>
        public const double DefaultSaturation = 33.0;

        /// <summary>
        /// Default Brightness Value for the Elgato Light.
        /// </summary>
        public const int DefaultBrightness = 20;

        /// <summary>
        /// Default Duration Switch On in Milliseconds Value for the Elgato Light.
        /// </summary>
        public const int DefaultDurationSwitchOnMs = 100;

        /// <summary>
        /// Default Duration Switch Off in Milliseconds Value for the Elgato Light.
        /// </summary>
        public const int DefaultDurationSwitchOffMs = 300;

        /// <summary>
        /// Default Duration Color Change in Milliseconds Value for the Elgato Light.
        /// </summary>
        public const int DefaultsDurationColorChangeMs = 100;

        /// <summary>
        /// Default Duration Power Watts in Hrz Value for the Elgato Light.
        /// </summary>
        public const int DefaultDefaultPWMHz = 300;

        /// <summary>
        /// Product Name
        /// </summary>
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// Hardware Board Type
        /// </summary>
        [JsonPropertyName("hardwareBoardType")]
        public int HardwareBoardType { get; set; }

        /// <summary>
        /// Firmware Build Number
        /// </summary>
        [JsonPropertyName("firmwareBuildNumber")]
        public int FirmwareBuildNumber { get; set; }

        /// <summary>
        /// Firmware Version
        /// </summary>
        [JsonPropertyName("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Serial Number
        /// </summary>
        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Display Name
        /// </summary>
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Features
        /// </summary>
        [JsonPropertyName("features")]
        public string[] Features { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if the light is on.
        /// </summary>
        [JsonPropertyName("on")]
        public int On { get; set; }

        /// <summary>
        /// brightness
        /// </summary>
        [JsonPropertyName("brightness")]
        public int Brightness { get; set; }

        /// <summary>
        /// temperature
        /// </summary>
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        /// <summary>
        /// Settings
        /// </summary>
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

        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint
        {
            get
            {
                return $"http://{Address}:{Port}/elgato/lights";
            }
        }

        /// <summary>
        /// IsOn
        /// </summary>
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

        /// <summary>
        /// Async turns on the light.
        /// </summary>
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

        /// <summary>
        /// Async turns off the light.
        /// </summary>
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


        /// <summary>
        /// Increases Brightness Async
        /// </summary>
        public async Task IncreaseBrightnessAsync(int amount)
        {
            await SetBrightnessAsync(Brightness + amount);
        }

        /// <summary>
        /// Decreases Brightness Async
        /// </summary>
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

            var receivedStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(receivedStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(receivedStatus);

            Debug.WriteLine($"SetBrightnessAsync New Values: {ToString()}");
        }

        /// <summary>
        ///  Sets the color Temperture Must be 2900-7000.
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        /// <exception cref="ElgatoLightNotOnException">Thrown if trying to set the brightness and the device isn't turned on.</exception>
        /// <exception cref="ElgatoLightOutOfRangeException">Thrown if the new value is out of range. The minimum range is <see cref="MinimumTemperature"/> and maximum temperature is <see cref="MaximumTemperature"/>.</exception>
        public async Task SetColorTemperatureAsync(int temp)
        {
            if (IsOn == false)
            {
                throw new ElgatoLightNotOnException("The light must be turned On in order to turn the set the temperature.");
            }

            await UpdateStatusAsync();

            Debug.WriteLine($"SetColorTemperatureAsync Old Values: {ToString()}");

            if (temp < MinimumTemperature || temp > MaximumTemperature)
            {
                throw new ElgatoLightOutOfRangeException($"Temperature Out of Range. {temp} is an invalid temperature.");
            }

            var targetStatusStatus = LightsStatus.FromLight(this);

            targetStatusStatus.Lights[0].Temperature = temp;

            var targetStatusJson = targetStatusStatus.ToJson();

            var receivedStatus = await UpdateDevice(targetStatusJson);

            if (string.IsNullOrWhiteSpace(receivedStatus))
            {
                Debug.WriteLine($"The network is either offline or the device returned null data.");
                return;
            }

            await ParseLightStatus(receivedStatus);

            Debug.WriteLine($"SetColorTemperatureAsync New Values: {ToString()}");
        }

        /// <summary>
        /// Increase Color Temperature Async
        /// </summary>
        public async Task IncreaseColorTemperatureAsync(int amount)
        {
            await SetColorTemperatureAsync(Temperature + amount);
        }

        /// <summary>
        /// Decrease Color Temperature Async
        /// </summary>
        public async Task DecreaseColorTemperatureAsync(int amount)
        {
            await SetColorTemperatureAsync(Temperature - amount);
        }

        public override string ToString()
        {
            return $@"Elgato Light {SerialNumber} @ {Address}:{Port}\nOn: \t\t{IsOn}\nBrightness:\t{Brightness}\nTemperature:\t{ Temperature }";
        }

        /// <summary>
        /// Update the light Status Async.
        /// </summary>
        internal async Task UpdateStatusAsync()
        {
            var statusEndPoint = string.Format(StatusInfoEndPointTemplate, Address, Port);

            if (NetworkDiscoveryHelper.IsNetworkAvailable() == false)
            {
                return;
            }

            using var client = new HttpClient();
            var streamTask = client.GetStreamAsync(statusEndPoint);

            LightsStatus status = await JsonSerializer.DeserializeAsync<LightsStatus>(await streamTask);

            if (status != null && status.Lights != null && status.Lights.Count > 0)
            {
                On = status.Lights[0].On;
                Brightness = status.Lights[0].Brightness;
                Temperature = status.Lights[0].Temperature;
            }
        }

        /// <summary>
        /// Update Settings Async
        /// </summary>
        internal async Task UpdateSettings()
        {
            var settingEndPoint = string.Format(SettingsEndPointTemplate, Address, Port);

            using var client = new HttpClient();
            var streamTask = client.GetStreamAsync(settingEndPoint);

            var settings = await JsonSerializer.DeserializeAsync<ElgatoLightSettings>(await streamTask);

            Settings = settings;
        }

        /// <summary>
        /// Parse Light Status Async
        /// </summary>
        internal async Task ParseLightStatus(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace", nameof(json));
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            using MemoryStream stream = new(byteArray);
            var status = await JsonSerializer.DeserializeAsync<LightsStatus>(stream);

            if (status != null && status.Lights != null && status.Lights.Count > 0)
            {
                On = status.Lights[0].On;
                Brightness = status.Lights[0].Brightness;
                Temperature = status.Lights[0].Temperature;
            }
        }

        /// <summary>
        /// Updates the Device Async
        /// </summary>
        private async Task<string> UpdateDevice(string lightStatusJson)
        {
            if (string.IsNullOrWhiteSpace(lightStatusJson))
            {
                throw new ArgumentException($"'{nameof(lightStatusJson)}' cannot be null or whitespace.", nameof(lightStatusJson));
            }

            return await UpdateDevice(EndPoint, lightStatusJson);
        }

        /// <summary>
        /// Update the Device Async
        /// </summary>
        private async Task<string> UpdateDevice(string endPoint, string lightStatusJson)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentException($"'{nameof(endPoint)}' cannot be null or whitespace.", nameof(endPoint));
            }

            if (string.IsNullOrWhiteSpace(lightStatusJson))
            {
                throw new ArgumentException($"'{nameof(lightStatusJson)}' cannot be null or whitespace.", nameof(lightStatusJson));
            }

            if (NetworkDiscoveryHelper.IsNetworkAvailable() == false)
            {
                return string.Empty;
            }

            using var client = new HttpClient();
            using var content = new StringContent(lightStatusJson, Encoding.UTF8);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            using var req = new HttpRequestMessage(HttpMethod.Put, endPoint);
            req.Content = content;

            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            using HttpResponseMessage resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadAsStringAsync();
        }
    }
}
