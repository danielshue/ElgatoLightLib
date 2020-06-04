using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace eelightlib
{
    /// <summary>
    /// Implements the <see cref="IElgatoLightMgr"/> interface.
    /// </summary>
    public class ElgatoLightMgr : IElgatoLightMgr
    {
        /// <summary>
        /// Discover Service for the Elgoto Key Light
        /// </summary>
        private const string ElgatoKeyLightService = "_elg._tcp.local.";

        /// <summary>
        /// Template used for the Elgoto Keylight Endpoint 
        /// </summary>
        private const string DiscoveryEndPointTemplate = @"http://{0}:{1}/elgato/accessory-info";

        /// <inheritdoc/>
        public async Task<IList<ElgatoLight>> StartDiscoverAsync(int timeout = 30, CancellationToken cancellationToken = default)
        {
            var lights = new List<ElgatoLight>();

            // discover the set of available lights
            ResolveOptions options = new ResolveOptions(ElgatoKeyLightService)
            {
                //ScanTime = TimeSpan.FromSeconds(timeout)
            };

            IReadOnlyList<IZeroconfHost> discoveredServices = await ZeroconfResolver.ResolveAsync(options, callback: null, cancellationToken);

            if (discoveredServices != null)
            {
                // for each light that is discovered, retrieve details about the keylight by calling the device directly
                foreach (var resolvedElgatoLightService in discoveredServices)
                {
                    var light = await RetrieveKeyLightInfoAsync(resolvedElgatoLightService.IPAddress, resolvedElgatoLightService.DisplayName, resolvedElgatoLightService.Services.FirstOrDefault().Value.Port);

                    lights.Add(light);
                }
            }

            return lights;
        }

        private async Task<ElgatoLight> RetrieveKeyLightInfoAsync(string ipaddress, string name, int port)
        {
            var endPoint = string.Format(DiscoveryEndPointTemplate, ipaddress, port);

            using (var client = new HttpClient())
            {
                var streamTask = client.GetStreamAsync(endPoint);

                var keylight = await JsonSerializer.DeserializeAsync<ElgatoLight>(await streamTask);

                // populate the keylight with details not included
                keylight.Address = ipaddress;
                keylight.Name = name;
                keylight.Port = port;

                return keylight;
            }

        }
    }
}
