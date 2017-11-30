using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;

namespace sizingservers.beholderv2.agent.linux {
    internal class NIC : IPayloadRetriever {
        public static NIC _instance = new NIC();

        public static NIC GetInstance() { return _instance; }

        private NIC() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            Dictionary<string, string> col = RetrieverProxy.GetInxiInfo("-n")["Network"];
            HashSet<PayloadProperty> currentProperties = null;
            foreach (string key in col.Keys)
                if (key.StartsWith("Card") && !key.EndsWith(" driver")) {
                    currentProperties = new HashSet<PayloadProperty>();
                    currentProperties.Add(new PayloadProperty("Name", col.GetValueOrDefault(key)));
                }
                else if (key.StartsWith("IF") && key.EndsWith(" mac")) {
                    currentProperties.Add(new PayloadProperty("MAC address", ("" + col.GetValueOrDefault(key)).ToUpperInvariant()));
                    cgs.Add(new ComponentGroup("NIC", currentProperties.ToArray())); //Can be spoofed, do not run in a VM!
                }

            return cgs;
        }
    }
}
