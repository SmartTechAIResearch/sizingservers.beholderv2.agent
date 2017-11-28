using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

#warning Ensure cpu name is the same for Linux and Windows
#warning UUID?

namespace sizingservers.beholderv2.agent.windows {
    internal class Disk : IPayloadRetriever {
        public static Disk _instance = new Disk();

        public static Disk GetInstance() { return _instance; }

        private Disk() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetInfo("Select Size, Model from Win32_DiskDrive where InterfaceType != 'USB'");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("Disk",
                    new PayloadProperty[] {
                        new PayloadProperty("Model", mo["Model"]),
                        new PayloadProperty("Size", long.Parse(mo["Size"].ToString()) / (1024 * 1024 * 1024), "GB")                        
                    })
                );

            return cgs;
        }
    }
}
