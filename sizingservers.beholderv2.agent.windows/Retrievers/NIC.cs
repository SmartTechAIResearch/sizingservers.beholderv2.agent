/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.windows {
    internal class NIC : IPayloadRetriever {
        public static NIC _instance = new NIC();

        public static NIC GetInstance() { return _instance; }

        private NIC() { }


        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col =
                RetrieverHelper.GetWmiInfo("SELECT InterfaceGuid, DriverDescription FROM MSFT_NetAdapter WHERE HardwareInterface = 'True' AND EndpointInterface = 'False'",
                "root\\StandardCimv2");
            foreach (ManagementObject mo in col) {
                string macAddress = "";

                var col2 = RetrieverHelper.GetWmiInfo("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE SettingID = '" + mo["InterfaceGuid"] + "'");
                foreach (var mo2 in col2) macAddress = mo2["MACAddress"].ToString().ToUpperInvariant();

                cgs.Add(new ComponentGroup("NIC",
                    new PayloadProperty[] {
                        new PayloadProperty("Name", mo["DriverDescription"].ToString().Replace("(R) ", "").Replace("(TM)", "").Replace("(tm)", "")),
                       new PayloadProperty("MAC address", macAddress, true) //Can be spoofed, do not run in a VM!
                    })
                );
            }

            return cgs;
        }
    }
}
