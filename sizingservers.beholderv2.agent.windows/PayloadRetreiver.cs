using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

namespace sizingservers.beholder.agent.windows {
    internal class PayloadRetreiver : IPayloadRetriever {
        public static PayloadRetreiver _instance = new PayloadRetreiver();

        public static PayloadRetreiver GetInstance() { return _instance; }

        private PayloadRetreiver() { }

        public IEnumerable<ComponentGroup> Retreive() {
            var cgs = new List<ComponentGroup>();

            cgs.AddRange(Machine.GetInstance().Retreive());
            cgs.AddRange(BaseBoard.GetInstance().Retreive());
            cgs.AddRange(CPU.GetInstance().Retreive());
            cgs.AddRange(RAM.GetInstance().Retreive());
            cgs.AddRange(Disk.GetInstance().Retreive());
            cgs.AddRange(NIC.GetInstance().Retreive());

            return ComponentGroupConsolidator.Do(cgs);
        }
    }
}
