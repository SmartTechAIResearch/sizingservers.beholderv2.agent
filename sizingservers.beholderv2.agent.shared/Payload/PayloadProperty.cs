using SizingServers.Util;
using System;
using System.Collections;
using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Made because we want to easily extend the hardware info we can get.
    /// </summary>
    public class PayloadProperty {
        public enum PayloadType { Collection, String, Long, Double, Boolean }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadProperty"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value. The "ToString()" is stored. Or if it is an IEnumerable it will be serialized as { + \"x0\", ..., \"xn\" + } (only single level collections are handled correctly).</param>
        /// <param name="unit">%, GB, empty string,... cannot be null</param>
        public PayloadProperty(string name, object value, string unit = "") {
            Name = name;
            SetValue(value);
            Unit = unit;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadProperty"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value. The "ToString()" is stored. Or if it is an IEnumerable it will be serialized as { + \"x0\", ..., \"xn\" + } (only single level collections are handled correctly).</param>
        /// <param name="unit">%, GB, empty string,... cannot be null</param>
        private PayloadProperty(string type, string name, string value, string unit) {
            Type = type;
            Name = name;
            Value = value;
            Unit = unit;
        }
        /// <summary>
        /// Sets the type safely. Stored as a string for easy serialisation.
        /// </summary>
        /// <param name="type">Type of the payload.</param>
        public void SetType(PayloadType type) { Type = Enum.GetName(typeof(PayloadType), type); }
        /// <summary>
        /// Sets the value. The "ToString()" is stored. Or if it is an IEnumerable it will be serialized as { + \"x0\", ..., \"xn\" + } (only single level collections are handled correctly).
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value) {
            if (value is string) {
                SetType(PayloadType.String);
                Value = value.ToString();
            }
            else if (value is IEnumerable) {
                SetType(PayloadType.Collection);
                var enumerator = (Value as IEnumerable).GetEnumerator();
                enumerator.Reset();

                var sb = new StringBuilder();
                sb.Append("{");
                long i = 0;
                while (enumerator.MoveNext()) {
                    if (i++ != 0) sb.Append(",");
                    sb.Append("\"");
                    sb.Append(enumerator.Current.ToString().Replace("\"", "\\\""));
                    sb.Append("\"");
                }
                sb.Append("}");
                Value = sb.ToString();
            }
            else if (value is float) {
                SetType(PayloadType.Double);
                StringUtil.FloatToLongString((float)value);
            }
            else if (value is double) {
                SetType(PayloadType.Double);
                StringUtil.DoubleToLongString((double)value);
            }
            else if (value is decimal) {
                SetType(PayloadType.Double);
                StringUtil.DecimalToLongString((decimal)value);
            }
            else if (value is short || value is ushort || value is int || value is uint || value is long || value is ulong) {
                SetType(PayloadType.Long);
                Value = value.ToString();
            }
            else if (value is bool) {
                SetType(PayloadType.Boolean);
                Value = value.ToString();
            }
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PayloadProperty Clone() { return new PayloadProperty(Type, Name, Value, Unit); }
    }
}
