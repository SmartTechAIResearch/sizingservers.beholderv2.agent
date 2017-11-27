using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Groups ComponentGroups by matching properties. If all properties match, the count property is increased.
    /// </summary>
    public static class ComponentGroupConsolidator {
        /// <summary>
        /// Groups ComponentGroups by matching properties. If all properties match, the count property is increased.
        /// </summary>
        /// <param name="componentGroups">The component groups.</param>
        /// <returns></returns>
        public static ComponentGroup[] Do(IEnumerable<ComponentGroup> componentGroups) {
            var flatAndFullCGs = new Dictionary<string, ComponentGroup>();
            foreach (var cg in componentGroups) flatAndFullCGs.Add(cg.Flatten(), cg);

            var consolidated = new Dictionary<string, ComponentGroup>();
            foreach(var kpv in flatAndFullCGs) {
                if (consolidated.ContainsKey(kpv.Key))
                    consolidated[kpv.Key].Count++;
                else
                    consolidated.Add(kpv.Key, Clone(kpv.Value)); 
            }

            return consolidated.Values.ToArray();
        }

        private static ComponentGroup Clone(ComponentGroup componentGroup) {
            ComponentGroup clone = new ComponentGroup(componentGroup.Type, componentGroup.Count);
            clone.Properties = new PayloadProperty[componentGroup.Properties.LongLength];

            for (long i = 0; i != componentGroup.Properties.LongLength; i++) {
                var p = componentGroup.Properties[i];
                clone.Properties[i] = p.Clone();
            }

            return clone;
        }
    }
}
