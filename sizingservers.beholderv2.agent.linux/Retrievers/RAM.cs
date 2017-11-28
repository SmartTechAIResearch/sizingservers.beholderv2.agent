﻿using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Management;

namespace sizingservers.beholderv2.agent.linux {
    internal class RAM : IPayloadRetriever {
        public static RAM _instance = new RAM();

        public static RAM GetInstance() { return _instance; }

        private RAM() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetreiverProxy.GetInfo("Select * from Win32_PhysicalMemory");
            foreach (ManagementObject mo in col)
                cgs.Add(new ComponentGroup("RAM",
                    new PayloadProperty[] {
                        new PayloadProperty("Capacity", mo["Name"], "bytes"),
                        new PayloadProperty("Manufacturer", mo["Manufacturer"]),
                        new PayloadProperty("Model", mo["Model"]),
                        new PayloadProperty("PartNumber", mo["PartNumber"]),
                        new PayloadProperty("Speed", mo["Speed"], "Mhz"),
                    })
                );

            return cgs;
        }
    }
}
