using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholder.agent.linux {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetreiverProxy.GetInfo("Select Name from Win32_Processor");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("CPU",
                    new PayloadProperty[] {
                        new PayloadProperty("Name", mo["Name"])
                    })
                );

            return cgs;
        }
    }
}
