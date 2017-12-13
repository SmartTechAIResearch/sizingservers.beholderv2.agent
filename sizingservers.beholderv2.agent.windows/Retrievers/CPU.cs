/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverHelper.GetWmiInfo("Select Name from Win32_Processor");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("CPU",
                    new PayloadProperty[] { new PayloadProperty("Name", mo["Name"]) })
                );

            return cgs;
        }
    }
}
