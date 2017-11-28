using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

#warning Ensure disk name is the same for Linux and Windows
#warning UUID?

namespace sizingservers.beholderv2.agent.linux {
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
                        new PayloadProperty("Size", mo["Size"], "GB"),
                        new PayloadProperty("Model", mo["Model"])
                    })
                );

            return cgs;
        }
    }
}
