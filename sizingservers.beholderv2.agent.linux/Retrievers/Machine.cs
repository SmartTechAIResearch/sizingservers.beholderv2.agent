using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;

namespace sizingservers.beholderv2.agent.linux {
    internal class Machine : IPayloadRetriever {
        public static Machine _instance = new Machine();

        public static Machine GetInstance() { return _instance; }

        private Machine() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new ComponentGroup[] { new ComponentGroup("Machine") };

            var properties = new HashSet<PayloadProperty>();

            Dictionary<string, string> col = RetrieverHelper.GetInxiInfo("-S")["System"];
            properties.Add(new PayloadProperty("Hostname", (col.GetValueOrDefault("Host") + "." + RetrieverHelper.GetDnsDomainName()).ToLowerInvariant(), true));
            properties.Add(new PayloadProperty("OS", col.GetValueOrDefault("Distro") + " Kernel " + col.GetValueOrDefault("Kernel")));

            col = RetrieverHelper.GetInxiInfo("-xi")["Network"];
            
            var ips = new List<string>();
            foreach (string key in col.Keys)
                if (key.Contains("ip-v"))
                    ips.Add(col[key]);

            properties.Add(new PayloadProperty("IPs", ips));

            cgs[0].Properties = properties.ToArray();

            return cgs;
        }
    }
}
