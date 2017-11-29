using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    internal class Machine : IPayloadRetriever {
        public static Machine _instance = new Machine();

        public static Machine GetInstance() { return _instance; }

        private Machine() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new ComponentGroup[] { new ComponentGroup("Machine") };

            var properties = new HashSet<PayloadProperty>();

            ManagementObjectCollection col = RetrieverProxy.GetWmiInfo("Select CSName, Domain, Manufacturer, Model from Win32_ComputerSystem");
            foreach (ManagementObject mo in col) {
                properties.Add(new PayloadProperty("Hostname", mo["CSName"] + "." + mo["Domain"], true));
            }

            col = RetrieverProxy.GetWmiInfo("Select IPAddress from Win32_NetworkAdapterConfiguration where IPEnabled='True'");
            var ips = new List<string>();
            foreach (ManagementObject mo in col)
                foreach (string ip in mo["IPAddress"] as string[]) ips.Add(ip);

            properties.Add(new PayloadProperty("IPs", ips));

            col = RetrieverProxy.GetWmiInfo("Select Version, Name, BuildNumber from Win32_OperatingSystem");
            foreach (ManagementObject mo in col) {
                properties.Add(new PayloadProperty("OS",
                    string.Format("{0} {1} Build {2}", mo["Name"].ToString().Split("|".ToCharArray())[0], mo["Version"], mo["BuildNumber"])));
            }
            cgs[0].Properties = properties.ToArray();

            return cgs;
        }
    }
}
