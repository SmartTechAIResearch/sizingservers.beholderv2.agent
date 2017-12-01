/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    internal class Disk : IPayloadRetriever {
        public static Disk _instance = new Disk();

        public static Disk GetInstance() { return _instance; }

        private Disk() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetWmiInfo("Select Model, SerialNumber from Win32_DiskDrive where InterfaceType != 'USB'");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("Disk",
                    new PayloadProperty[] {
                        new PayloadProperty("Model", mo["Model"]),
                        new PayloadProperty("Serial number", mo["SerialNumber"], true)
                    })
                );

            return cgs;
        }
    }
}
