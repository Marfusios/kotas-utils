using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kotas.Utils.Common.Json
{
    public static class JsonSerializer
    {
        /// <summary>
        /// Custom json formatting - lower camel case, indented, enum to string
        /// </summary>
        public static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>() { new StringEnumConverter() { CamelCaseText = true} },
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static Newtonsoft.Json.JsonSerializer Serializer => Newtonsoft.Json.JsonSerializer.Create(Settings);
    }
}
