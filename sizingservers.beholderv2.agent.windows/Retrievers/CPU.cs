using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholder.agent.windows {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retreive() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetInfo("Select Name from Win32_Processor");
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
