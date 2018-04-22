using System;
using System.Globalization;
using Kotas.Utils.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kotas.Utils.Common.Json
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var substracted = ((DateTime)value).Subtract(DateTimeUtils.UnixBase);
            writer.WriteRawValue(substracted.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return DateTimeUtils.ConvertToUnixTime((long)reader.Value);
        }
    }
}
