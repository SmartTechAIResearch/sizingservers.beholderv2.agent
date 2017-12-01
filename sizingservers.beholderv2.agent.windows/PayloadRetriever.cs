/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using sizingservers.beholderv2.agent.shared;
using System;
using System.Collections.Generic;

namespace sizingservers.beholderv2.agent.windows {
    internal class PayloadRetriever : IPayloadRetriever {
        private static PayloadRetriever _instance = new PayloadRetriever();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static PayloadRetriever GetInstance() { return _instance; }

        private PayloadRetriever() { }
        /// <summary>
        /// Retrieves system info.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new List<ComponentGroup>();

            try { cgs.AddRange(Machine.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }
            try { cgs.AddRange(BaseBoard.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }
            try { cgs.AddRange(CPU.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }
            try { cgs.AddRange(RAM.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }
            try { cgs.AddRange(Disk.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }
            try { cgs.AddRange(NIC.GetInstance().Retrieve()); } catch (Exception ex) { LogToConsole.Error(ex); }

            return ComponentGroupConsolidator.Do(cgs);
        }
    }
}
