using sizingservers.beholderv2.agent.shared;
using System.Collections.Generic;

namespace sizingservers.beholderv2.agent.linux {
    internal class RAM : IPayloadRetriever {
        public static RAM _instance = new RAM();

        public static RAM GetInstance() { return _instance; }

        private RAM() { }

        public IEnumerable<ComponentGroup> Retrieve() {
            var cgs = new HashSet<ComponentGroup>();

            Dictionary<string, string> col = RetrieverHelper.GetInxiInfo("-mxx")["Memory"];
            HashSet<PayloadProperty> currentProperties = null;
            foreach (string key in col.Keys)
                if (key.StartsWith("Device")) {
                    string value = col.GetValueOrDefault(key);
                    if (value == null || value.StartsWith("No Module") || value == "N/A")
                        continue;

                    if (key.EndsWith(" size")) {
                        currentProperties = new HashSet<PayloadProperty>();

                        string[] split = value.Split(' ');
                        currentProperties.Add(new PayloadProperty("Size", long.Parse(split[0]), false, split[1]));
                    }
                    else if (key.EndsWith(" speed")) {
                        string[] split = value.Split(' ');
                        currentProperties.Add(new PayloadProperty("Speed", long.Parse(split[0]), false, split[1]));
                    }
                    else if (key.EndsWith(" type")) {
                        currentProperties.Add(new PayloadProperty("Type", value));
                    }
                    else if (key.EndsWith(" manufacturer")) {
                        currentProperties.Add(new PayloadProperty("Manufacturer", value));
                    }
                    else if (key.EndsWith(" part")) {
                        currentProperties.Add(new PayloadProperty("Part number", value));
                    }
                    else if (key.EndsWith(" serial")) {
                        currentProperties.Add(new PayloadProperty("Serial number", value, true));
                    }
                }

            return cgs;
        }
    }
}
