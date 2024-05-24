using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElgatoLightLib
{
    /// <summary>
    /// Interface for managing the Elgato Key Light and Eve Light.
    /// This interface, IElgatoLightMgr, defines a contract for managing 
    /// the Elgato Key Light and Eve Light. It serves as a blueprint for classes that will 
    /// implement the functionality to interact with these lights.
    /// </summary>
    public interface IElgatoLightMgr
    {
        /// <summary>
        /// Starts discovering the Elgato Key Lights and Even Light Strip asynchronous.
        /// </summary>
        /// <param name="timeout">The timeout which specifies the duration for the discovery process.</param>
        /// <param name="cancellationToken">The cancellation token parameter is used to provide a cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns a Task<IList<ElgatoLight>>, which represents an asynchronous operation that will 
        /// eventually provide a list of Elgato lights found on the network. 
        /// The IList<ElgatoLight> represents a collection of ElgatoLight objects, which likely 
        /// contain information about each discovered light.
        /// </returns>
        Task<IList<ElgatoLight>> StartDiscoverAsync(int timeout, CancellationToken cancellationToken = default);
    }
}
