using System;

namespace Kotas.Utils.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; protected set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }
}