using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eelightlib
{
    /// <summary>
    /// Class Library for the Elgato Light
    /// </summary>
    public class ElgatoLight
    {
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
        public bool On { get; set; }

        /// <exclude />
        [JsonPropertyName("brightness")]
        public int Brightness { get; set; }

        /// <exclude />
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        /// <exclude />
        public string EndPoint
        {
            get
            {
                return $"http://{Address}:{Port}/elgato/lights";
            }
        }

        public async Task OnAsync()
        {
            var json = "{ \"numberOfLights\":1,\"lights\":[{ \"on\":1}]}";

            var ret = await HttpPut(json);

            ParseReturn(ret);

            Debug.WriteLine($"OnAsync Values:\n{ToInfo()}");

            if (On == false)
            {
                throw new LightNotOnException("Failed to turn light On.");
            }
        }

        public async Task OffAsync()
        {
            var json = "{ \"numberOfLights\":1,\"lights\":[{ \"on\":0}]}";

            var ret = await HttpPut(json);

            ParseReturn(ret);

            Debug.WriteLine($"OffAsync Values:\n{ToInfo()}");

            if (On)
            {
                throw new LightNotOnException("Failed to turn light Off.");
            }
        }

        /// <summary>
        /// Sets the light to a specific brightness level of 0-100.
        /// </summary>
        /// <param name="level">The level amount to set the brightness.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new value is out of range. The minimum total range 0 and maximum of 100.</exception>
        public async Task SetBrightnessAsync(int level)
        {
            if (On == false)
            {
                throw new LightNotOnException("The light must be turned On in order to turn the set the brightness.");
            }

            if (level < 0)
            {
                throw new ArgumentOutOfRangeException($"The requested level of {level} would exceed the minimum range of 0.");
            }

            if (level > 100)
            {
                throw new ArgumentOutOfRangeException($"The requested level of {level} would exceed the maxmium range of 100.");
            }

            var json = $"{{ \"numberOfLights\": 1,\"lights\": [{{\"brightness\":{level} }}]}}";

            var ret = await HttpPut(json);

            ParseReturn(ret);
        }

        public async Task IncreaseBrightnessAsync(int amount)
        {
            if (On == false)
            {
                throw new LightNotOnException("The light must be turned On in order to turn the increase the brightness.");
            }

            if (Brightness + amount < 0)
            {
                throw new ArgumentOutOfRangeException($"Brightness currently set at {Brightness}. The requested amount {amount} would exceed the minimum range of 0.");
            }

            if (Brightness + amount > 100)
            {
                throw new ArgumentOutOfRangeException($"Brightness currently set at {Brightness}. The requested amount of {amount} would exceed the maxmium range of 100.");
            }

            await SetBrightnessAsync(Brightness + amount);
        }

        public async Task DecreaseBrightnessAsync(int amount)
        {
            if (On == false)
            {
                throw new LightNotOnException("The light must be turned On in order to turn the decrease the brightness.");
            }

            if (Brightness - amount < 0)
            {
                throw new ArgumentOutOfRangeException($"Brightness currently set at {Brightness}. The requested amount of {amount} would exceed the minimum range of 0.");
            }

            if (Brightness - amount > 100)
            {
                throw new ArgumentOutOfRangeException($"Brightness currently set at {Brightness}. The requested amount of {amount} would exceed the maxmium range of 100.");
            }

            await SetBrightnessAsync(Brightness - amount);
        }

        /// <summary>
        ///  Must be 2900-7000
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public async Task SetColorTemperatureAsync(int temp)
        {
            var json = $"{{ \"numberOfLights\": 1,\"lights\": [{{\"temperature\":{  temp } }}]}}";

            var ret = await HttpPut(json);

            ParseReturn(ret);
        }

        public async Task IncreaseColorTemperatureAsync(int amount)
        {
            Debug.WriteLine($"IncreaseColorTemperatureAsync Old Values: {ToInfo()}");

            await SetColorTemperatureAsync(Temperature + amount);

            Debug.WriteLine($"IncreaseColorTemperatureAsync New Values: {ToInfo()}");
        }

        public async Task DecreaseColorTemperatureAsync(int amount)
        {
            await OnAsync(); // used to ensure the latest values are set

            Debug.WriteLine($"DecreaseColorTemperatureAsync Old Values: {ToInfo()}");

            await SetColorTemperatureAsync(Temperature - amount);

            Debug.WriteLine($"DecreaseColorTemperatureAsync New Values: {ToInfo()}");
        }

        public override string ToString()
        {
            return string.Format(@"Elgato Light {0} @ {1}:{2}", SerialNumber, Address, Port);
        }

        public string ToInfo()
        {
            return $"On: \t\t{On}\nBrightness:\t{Brightness}\nTemperature:\t{ Temperature }";
        }

        internal async Task<string> HttpPut(string data)
        {
            return await HttpPut(EndPoint, data);
        }

        private void ParseReturn(string ret)
        {
            var document = JsonDocument.Parse(ret);

            foreach (var element in document.RootElement.EnumerateObject())
            {
                if (element.Name == "lights")
                {
                    var value = element.Value;

                    if (value.ValueKind == JsonValueKind.Array)
                    {
                        var doc2 = JsonDocument.Parse(value.GetRawText());

                        foreach (JsonElement el1 in doc2.RootElement.EnumerateArray())
                        {
                            int onValue = el1.GetProperty("on").GetInt32();
                            On = onValue == 1;
                            Brightness = el1.GetProperty("brightness").GetInt32();
                            Temperature = el1.GetProperty("temperature").GetInt32();
                        }
                    }
                }
            }
        }

        internal async Task<string> HttpPut(string endPoint, string data)
        {
            using (var client = new HttpClient())
            {
                using (var content = new StringContent(data, Encoding.UTF8))
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
