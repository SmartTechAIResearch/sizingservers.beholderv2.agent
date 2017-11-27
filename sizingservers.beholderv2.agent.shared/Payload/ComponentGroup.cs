using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    public class ComponentGroup {
        public string Type { get; set; }
        public long Count { get; set; }
        public PayloadProperty[] Properties { get; set; }

        public ComponentGroup(string type, params PayloadProperty[] properties)
            : this(type, 1, properties) { }
        public ComponentGroup(string type, long count, params PayloadProperty[] properties) {
            Type = type;
            Count = count;
            Properties = properties;
        }
        /// <summary>
        /// Puts type and properties one after another in a string, for comparing and consolidating component groups. Count is ommited.
        /// </summary>
        /// <returns></returns>
        public string Flatten() {
            var sb = new StringBuilder();
            sb.Append(Type);
            foreach (var property in Properties) {
                sb.Append(property.Name);
                sb.Append(property.Type);
                sb.Append(property.Value);
            }
            return sb.ToString();
        }
    }
}
