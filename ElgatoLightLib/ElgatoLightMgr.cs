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
    /// Implements the <see cref="IElgatoLightMgr"/> interface.
    /// </summary>
    public class ElgatoLightMgr : IElgatoLightMgr
    {
        public static TimeSpan BonjourDiscoveryInterval = TimeSpan.FromSeconds(3.0);

        public static int DefaultPort = 9123;

        /// <summary>
        /// Discover Service for the Elgoto Key Light
        /// </summary>
        private const string BonjourDiscoveryElgatoKeyLightService = "_elg._tcp.local.";
        //private const string ElgatoEveKeyLightService = "_hap._tcp.local.";

        /// <summary>
        /// Template used for the Elgoto Keylight Endpoint 
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
            ResolveOptions elgatoKeyOptions = new ResolveOptions(BonjourDiscoveryElgatoKeyLightService)
            {
                //ScanTime = TimeSpan.FromSeconds(timeout)
            };

            /*
            ResolveOptions eveKeyOptions = new ResolveOptions(ElgatoEveKeyLightService)
            {
                //ScanTime = TimeSpan.FromSeconds(timeout)
            };
            */

            IReadOnlyList<IZeroconfHost> elgatoDiscoveredServices = await ZeroconfResolver.ResolveAsync(elgatoKeyOptions, callback: null, cancellationToken);
           // IReadOnlyList<IZeroconfHost> eveDiscoveredServices = await ZeroconfResolver.ResolveAsync(eveKeyOptions, callback: null, cancellationToken);

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

        private async Task<ElgatoLight> RetrieveKeyLightInfoAsync(string ipaddress, string name, int port)
        {
            var endPoint = string.Format(AccessoryInfoEndPointTemplate, ipaddress, port);

            using (var client = new HttpClient())
            {
                var streamTask = client.GetStreamAsync(endPoint);

                var keylight = await JsonSerializer.DeserializeAsync<ElgatoLight>(await streamTask);

                // populate the keylight with details not included in the serialization
                keylight.Address = ipaddress;
                keylight.Name = name;
                keylight.Port = port;

                return keylight;
            }
        }
    }
}
