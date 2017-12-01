/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using System.Diagnostics;
using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// A component group descibes a type of hardware.
    /// Type and Count are obligated, the others are free to chose: ParyloadProperty[] properties. 
    /// One should always try to get as close ase possible to a collection of 1 piece of hardware. 
    /// Use a unique serial number if that's available. e.g. Disk: Name, Serial; not possible for CPUs. 
    /// That way it is easier to track every single piece of hardware.  
    /// A property can be labeled as unique --> this one will / should be used to compare hardware at the endpoint instead of combining all properties. 
    /// </summary>
    [DebuggerDisplay("Type = {Type}, Count = {Count}")]
    public class ComponentGroup {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; set; }
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public PayloadProperty[] Properties { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentGroup"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="properties">The properties.</param>
        public ComponentGroup(string type, params PayloadProperty[] properties) : this(type, 1, properties) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentGroup"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <param name="properties">The properties.</param>
        public ComponentGroup(string type, int count, params PayloadProperty[] properties) {
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
                sb.Append(property.UniqueId);
                sb.Append(property.Unit);
            }
            return sb.ToString();
        }
    }
}
