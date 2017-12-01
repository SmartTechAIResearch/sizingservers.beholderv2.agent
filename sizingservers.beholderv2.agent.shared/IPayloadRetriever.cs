/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using System.Collections.Generic;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Interface for a class that retreives system info.
    /// </summary>
    public interface IPayloadRetriever {
        /// <summary>
        /// Retrieves system info.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ComponentGroup> Retrieve();        
    }
}
