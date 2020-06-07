using System.Net.NetworkInformation;

namespace ElgatoLightLib
{
    internal static class NetworkDiscoveryHelper
    {
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.Speed >= minimumSpeed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
