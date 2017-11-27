using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

namespace sizingservers.beholder.agent.linux {
    internal class PayloadRetreiver : IPayloadRetriever {
        public static PayloadRetreiver _instance = new PayloadRetreiver();

        public static PayloadRetreiver GetInstance() { return _instance; }

        private PayloadRetreiver() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new List<ComponentGroup>();

            cgs.AddRange(Machine.GetInstance().Retrieve());
            cgs.AddRange(BaseBoard.GetInstance().Retrieve());
            cgs.AddRange(CPU.GetInstance().Retrieve());
            cgs.AddRange(RAM.GetInstance().Retrieve());
            cgs.AddRange(Disk.GetInstance().Retrieve());
            cgs.AddRange(NIC.GetInstance().Retrieve());

            return ComponentGroupConsolidator.Do(cgs);
        }
    }
}
