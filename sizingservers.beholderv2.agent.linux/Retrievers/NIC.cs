using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.linux {
    internal class NIC : IPayloadRetriever {
        public static NIC _instance = new NIC();

        public static NIC GetInstance() { return _instance; }

        private NIC() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = 
                RetreiverProxy.GetInfo("SELECT Name, DriverDescription, MediaConnectState FROM MSFT_NetAdapter WHERE HardwareInterface = 'True' AND EndpointInterface = 'False'",
                "root\\StandardCimv2");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("NIC",
                    new PayloadProperty[] {
                        new PayloadProperty("Name", mo["Name"]),
                        new PayloadProperty("DriverDescription", mo["DriverDescription"]),
                        new PayloadProperty("MediaConnectState", mo["MediaConnectState"])
                    })
                );

            return cgs;
        }
    }
}
