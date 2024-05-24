using System.Net.NetworkInformation;

namespace ElgatoLightLib
{
    /// <summary>
    /// Network Discovery Helper
    /// </summary>
    internal static class NetworkDiscoveryHelper
    {
        /// <summary>
        /// Is Network Available
        /// </summary>
        /// <returns>Returns true if the network is available. Otherwise, it will return false.</returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        /// <summary>
        /// It checks if the network is available and if the speed of any network interface is greater than or equal to the minimum speed provided.
        /// </summary>
        /// <param name="minimumSpeed">Minimum Speed</param>
        /// <returns>
        /// If the network is available and there is at least one network interface that meets the specified conditions 
        /// (operational status is up, not a loopback or tunnel interface, and has a speed greater than or equal to the minimum speed), 
        /// the method will return true. Otherwise, it will return false.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel || networkInterface.Speed < minimumSpeed)
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}
