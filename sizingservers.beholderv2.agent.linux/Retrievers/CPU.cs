using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

namespace sizingservers.beholderv2.agent.linux {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            RetrieverProxy.CpuInfo cpuInfo = RetrieverProxy.GetCpuInfo();

            var properties = new PayloadProperty[cpuInfo.Count];
            for (int i = 0; i != cpuInfo.Count; i++) properties[i] = new PayloadProperty("Name", cpuInfo.Name);

            cgs.Add(new ComponentGroup("CPU", properties));
            
            return cgs;
        }
    }
}
