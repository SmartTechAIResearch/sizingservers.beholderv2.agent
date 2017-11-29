using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;

namespace sizingservers.beholderv2.agent.linux {
    internal class BaseBoard : IPayloadRetriever {
        public static BaseBoard _instance = new BaseBoard();

        public static BaseBoard GetInstance() { return _instance; }

        private BaseBoard() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new ComponentGroup[] { new ComponentGroup("BaseBoard") };

            var properties = new HashSet<PayloadProperty>();

            Dictionary<string, string> col = RetrieverProxy.GetInxiInfo("M")["Machine"];
            properties.Add(new PayloadProperty("Manufacturer", col.GetValueOrDefault("Mobo")));
            properties.Add(new PayloadProperty("Model", col.GetValueOrDefault("Mobo model")));
            properties.Add(new PayloadProperty("SerialNumber", col.GetValueOrDefault("Mobo serial"), true));
            properties.Add(new PayloadProperty("BIOS", col.GetValueOrDefault("Bios v")));

            cgs[0].Properties = properties.ToArray();

            return cgs;
        }
    }
}
