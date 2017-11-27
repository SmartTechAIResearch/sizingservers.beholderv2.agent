using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholder.agent.linux {
    internal class Disk : IPayloadRetriever {
        public static Disk _instance = new Disk();

        public static Disk GetInstance() { return _instance; }

        private Disk() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetreiverProxy.GetInfo("Select Size, Model from Win32_DiskDrive where InterfaceType != 'USB'");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("Disk",
                    new PayloadProperty[] {
                        new PayloadProperty("Size", mo["Size"], "bytes"),
                        new PayloadProperty("Model", mo["Model"])
                    })
                );

            return cgs;
        }
    }
}
