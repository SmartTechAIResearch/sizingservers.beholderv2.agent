using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholder.agent.windows {
    internal class Disk : IPayloadRetriever {
        public static Disk _instance = new Disk();

        public static Disk GetInstance() { return _instance; }

        private Disk() { }

        public IEnumerable<ComponentGroup> Retreive() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetInfo("Select Size, Model from Win32_DiskDrive where InterfaceType != 'USB'");
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
