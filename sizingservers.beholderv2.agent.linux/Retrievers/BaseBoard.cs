using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace sizingservers.beholder.agent.linux {
    internal class BaseBoard : IPayloadRetriever {
        public static BaseBoard _instance = new BaseBoard();

        public static BaseBoard GetInstance() { return _instance; }

        private BaseBoard() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new ComponentGroup[] { new ComponentGroup("BaseBoard") };

            var properties = new HashSet<PayloadProperty>();

            ManagementObjectCollection col = RetreiverProxy.GetInfo("Select Name from Win32_BaseBoard");
            foreach (ManagementObject mo in col) {
                properties.Add(new PayloadProperty("Manufacturer", mo["Manufacturer"]));
                properties.Add(new PayloadProperty("Model", mo["Model"]));
                properties.Add(new PayloadProperty("Product", mo["Product"]));
                properties.Add(new PayloadProperty("PartNumber", mo["PartNumber"]));
            }
            col = RetreiverProxy.GetInfo("Select Name from Win32_BIOS WHERE PrimaryBIOS='True'");
            foreach (ManagementObject mo in col)
                properties.Add(new PayloadProperty("BIOS", mo["Name"]));

            cgs[0].Properties = properties.ToArray();

            return cgs;
        }
    }
}
