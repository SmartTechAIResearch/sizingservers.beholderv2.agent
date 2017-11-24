using System;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Made because we want to easily extend the hardware info we can get.
    /// </summary>
    public class PayloadProperty {
        public enum PayloadType { String, Long, Double, Boolean }
        
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public PayloadProperty(PayloadType type, string name, string value) {
            SetType(type);
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Sets the type safely. Stored as a string for easy serialisation.
        /// </summary>
        /// <param name="type">Type of the payload.</param>
        public void SetType(PayloadType type) {
            Type = Enum.GetName(typeof(PayloadType), type);
        }
    }
}
