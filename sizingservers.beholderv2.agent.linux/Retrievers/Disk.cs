using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;

namespace sizingservers.beholderv2.agent.linux {
    internal class Disk : IPayloadRetriever {
        public static Disk _instance = new Disk();

        public static Disk GetInstance() { return _instance; }

        private Disk() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            Dictionary<string, string> col = RetrieverProxy.GetInxiInfo("-D -xx")["Drives"];
            HashSet<PayloadProperty> currentProperties = null;
            foreach (string key in col.Keys)
                if (key.StartsWith("ID")) {
                    if (key.EndsWith(" model")) {
                        currentProperties = new HashSet<PayloadProperty>();
                        currentProperties.Add(new PayloadProperty("Model", col.GetValueOrDefault(key)));
                    }
                    else if (key.EndsWith(" serial")) {
                        currentProperties.Add(new PayloadProperty("SerialNumber", col.GetValueOrDefault(key), true));

                        cgs.Add(new ComponentGroup("Disk", currentProperties.ToArray()));
                    }
                }

            return cgs;
        }
    }
}
