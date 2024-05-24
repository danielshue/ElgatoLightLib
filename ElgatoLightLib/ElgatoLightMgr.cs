using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace ElgatoLightLib
{
    /// <summary>
    /// The class ElgatoLightMgr is defined and it implements the <see cref="IElgatoLightMgr"/> interface.
    /// </summary>
    public class ElgatoLightMgr : IElgatoLightMgr
    {
        /// <summary>
        /// Bonjour Discovery Interval in used in Bonjour.
        /// </summary>
        /// <remarks>
        /// Default time is 3 seconds.
        /// </remarks>
        internal static TimeSpan BonjourDiscoveryInterval = TimeSpan.FromSeconds(3.0);

        /// <summary>
        /// Default Port used for Communication.
        /// </summary>
        internal static int DefaultPort = 9123;

        /// <summary>
        /// Discover Service for the Elgato Key Light.
        /// </summary>
        private const string BonjourDiscoveryElgatoKeyLightService = "_elg._tcp.local.";
        //private const string ElgatoEveKeyLightService = "_hap._tcp.local.";

        /// <summary>
        /// Template used for the Elgato Keylight Endpoint.
        /// </summary>
        private const string AccessoryInfoEndPointTemplate = @"http://{0}:{1}/elgato/accessory-info";
        
        /// <inheritdoc/>
        public async Task<IList<ElgatoLight>> StartDiscoverAsync(int timeout = 30, CancellationToken cancellationToken = default)
        {
            var lights = new List<ElgatoLight>();

            if (NetworkDiscoveryHelper.IsNetworkAvailable() == false)
            {
                 return lights;
            }

            // discover the set of available lights
            ResolveOptions elgatoKeyOptions = new(BonjourDiscoveryElgatoKeyLightService)
            {
            };

            IReadOnlyList<IZeroconfHost> elgatoDiscoveredServices = await ZeroconfResolver.ResolveAsync(elgatoKeyOptions, callback: null, cancellationToken);

            if (elgatoDiscoveredServices != null)
            {
                // for each light that is discovered, retrieve details about the keylight by calling the device directly
                foreach (var resolvedElgatoLightService in elgatoDiscoveredServices)
                {
                    var light = await RetrieveKeyLightInfoAsync(resolvedElgatoLightService.IPAddress, resolvedElgatoLightService.DisplayName, resolvedElgatoLightService.Services.FirstOrDefault().Value.Port);

                    if(light != null)
                    {

                        await light.UpdateStatusAsync();

                        await light.UpdateSettings();

                        lights.Add(light);

                    }
                }
            }

/*            if (eveDiscoveredServices != null)
            {
                // for each light that is discovered, retrieve details about the keylight by calling the device directly
                foreach (var resolvedEveLightService in eveDiscoveredServices)
                {
                    var light = await RetrieveKeyLightInfoAsync(resolvedEveLightService.IPAddress, resolvedEveLightService.DisplayName, resolvedEveLightService.Services.FirstOrDefault().Value.Port);

                    //lights.Add(light);
                }
            }

*/            return lights;
        }

        /// <summary>
        /// The RetrieveKeyLightInfoAsync method is a private asynchronous method that retrieves information about an Elgato Key Light device.
        /// </summary>
        /// <param name="ipaddress">The IP address of the device.</param>
        /// <param name="name">The display name of the device.</param>
        /// <param name="port">the port number used for communication with the device.</param>
        /// <returns></returns>
        private async Task<ElgatoLight> RetrieveKeyLightInfoAsync(string ipaddress, string name, int port)
        {
            var endPoint = string.Format(AccessoryInfoEndPointTemplate, ipaddress, port);

            using var client = new HttpClient();
            var streamTask = client.GetStreamAsync(endPoint);

            var keylight = await JsonSerializer.DeserializeAsync<ElgatoLight>(await streamTask);

            // Check if keylight is not null before accessing its properties
            if (keylight != null)
            {
                // populate the keylight with details not included in the serialization
                keylight.Address = ipaddress;
                keylight.Name = name;
                keylight.Port = port;
            }

            return keylight;
        }
    }
}
