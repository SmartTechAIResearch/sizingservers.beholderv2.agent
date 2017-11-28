using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;
using System.Linq;

#warning Fetch stuff from CPUID? Family, Model, Stepping, Ext. Family, Ext. Model, Revision.
#warning Ensure cpu name is the same for Linux and Window

namespace sizingservers.beholderv2.agent.linux {
    internal class CPU : IPayloadRetriever {
        public static CPU _instance = new CPU();

        public static CPU GetInstance() { return _instance; }

        private CPU() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            Dictionary<string, string> col = RetrieverProxy.GetInxiInfo("C").ElementAt(0).Value; //Can be CPU or CPU(s)

            //CPU name can be anything, but always ends on core
            foreach (string key in col.Keys)
                if (key.EndsWith(" core")) {
                    string name = col.GetValueOrDefault(key); //Format the name so it looks like the wmi output.
                    RetrieverProxy.GetStringBetween(name, " (-", "-)", out name);
                    cgs.Add(new ComponentGroup("CPU", new PayloadProperty("Name", name)));
                }


            return cgs;
        }
    }
}
