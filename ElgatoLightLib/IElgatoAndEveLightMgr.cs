using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace eelightlib
{
    /// <summary>
    /// Interface for manging the Elgato Key Light and Eve Light
    /// </summary>
    public interface IElgatoLightMgr
    {
        /// <summary>
        /// Starts discovering the Elgato Key Lights and Even Light Strip asynchronous.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Return a list of Elgato lights on the network.</returns>
        Task<IList<ElgatoLight>> StartDiscoverAsync(int timeout, CancellationToken cancellationToken = default);
    }
}
