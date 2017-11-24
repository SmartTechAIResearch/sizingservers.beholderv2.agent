using System;
using System.Collections.Generic;
using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    public class ComponentGroup {
        public PayloadProperty[] Properties { get; set; }

        public ComponentGroup(string type, string name) : this(type, new PayloadProperty(PayloadProperty.PayloadType.String, "Name", name)) { }
        public ComponentGroup(string type, params PayloadProperty[] properties) {
            Properties = new PayloadProperty[properties.Length + 1];
            Properties[0] = new PayloadProperty(PayloadProperty.PayloadType.String, "Type", type);
            properties.CopyTo(Properties, 1);
        }
    }
}
