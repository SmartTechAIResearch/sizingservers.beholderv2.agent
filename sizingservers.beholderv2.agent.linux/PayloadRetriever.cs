using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

namespace sizingservers.beholderv2.agent.linux {
    internal class PayloadRetriever : IPayloadRetriever {
        public static PayloadRetriever _instance = new PayloadRetriever();

        public static PayloadRetriever GetInstance() { return _instance; }

        private PayloadRetriever() { }

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
