using sizingservers.beholderv2.agent.shared;
using System;
using System.Collections.Generic;
using System.Management;

#warning Fetch stuff from CPUID? Family, Model, Stepping, Ext. Family, Ext. Model, Revision.
#warning Ensure cpu name is the same for Linux and Windows

namespace sizingservers.beholderv2.agent.windows {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            ManagementObjectCollection col = RetrieverProxy.GetInfo("Select Name from Win32_Processor");
            foreach (ManagementObject mo in col) {
                string name = mo["Name"].ToString(); //Format the name so it looks like the inxi output.
                name = name.Split(new string[] { " CPU @" }, StringSplitOptions.None)[0];
                name = name.Replace("(R) ", "").Replace("(TM)", "").Replace("(tm)", "");                

                cgs.Add(new ComponentGroup("CPU",
                    new PayloadProperty[] {
                        new PayloadProperty("Name", name)
                    })
                );
            }

            return cgs;
        }
    }
}
