using System.ComponentModel;
using System.Diagnostics;

namespace ActionContainers
{
    /// <summary>
    /// Represents an <see cref="IParameter"/> that can be freely mutated.
    /// </summary>
    /// <remarks>
    /// With JSON (de-) serialization in mind, this object deliberately uses concrete types over interfaces.
    /// </remarks>
    [DebuggerDisplay($"{{{nameof(Id)},nq}}")]
    internal class MutableParameter(ParameterNode parameterNode, MutableAction parent) : IParameter
    {
        private const char separatorChar = ';';

        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Value { get; set; }

        /// <inheritdoc/>
        public Unit Unit { get; set; }

        /// <inheritdoc cref="IParameter.ParentAction"/>
        public MutableAction ParentAction { get; } = parent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IAction IParameter.ParentAction => ParentAction;

        /// <inheritdoc cref="IParameter.SubAction"/>
        public MutableAction SubAction
        {
            get
            {
                return SubActions.FirstOrDefault();
            }
            set
            {
                SubActions = [value];
            }
        }

        /// <inheritdoc cref="IParameter.SubActions"/>
        public List<MutableAction> SubActions { get; set; }

        /// <inheritdoc/>
        IAction IParameter.SubAction => SubAction;

        /// <inheritdoc/>
        List<IAction> IParameter.SubActions => [.. SubActions];

        /// <inheritdoc/>
        public bool TryGetValue<TEnum>(out TEnum value) where TEnum : struct, Enum
        {
            string intermediateValue = Value ?? "";
            return Enum.TryParse(intermediateValue, out value);
        }

        /// <inheritdoc/>
        public bool TryGetValue(out bool value)
        {
            //null-safe value parsing
            if (TryGetValue(out int intValue))
            {
                value = intValue is 1;
                return true;
            }

            //not found
            value = default;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(out double value)
        {
            //null-safe value parsing
            if (TryGetValue(out string strValue))
                return double.TryParse(strValue, out value);

            //not found
            value = default;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(out int value)
        {
            //null-safe value parsing
            if (TryGetValue(out string strValue))
                return int.TryParse(strValue, out value);

            //not found
            value = default;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(out string value)
        {
            value = Value ?? "";
            return true;
        }

        /// <inheritdoc/>
        public bool TryGetValue(out IList<double> value)
        {
            //null-safe value parsing
            if (TryGetValue(out string strValue))
            {
                var stringValues = strValue.Split(separatorChar);

                //parse strings to target type
                value = new List<double>();
                foreach (var stringValue in stringValues)
                {
                    if (double.TryParse(stringValue, out var doubleValue))
                        value.Add(doubleValue);
                }

                return true;
            }

            //not found
            value = Array.Empty<double>();
            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(out IList<string> value)
        {
            //null-safe value parsing
            if (TryGetValue(out string strValue))
            {
                value = strValue.Split(separatorChar);
                return true;
            }

            //not found
            value = Array.Empty<string>();
            return false;
        }
    }
}
