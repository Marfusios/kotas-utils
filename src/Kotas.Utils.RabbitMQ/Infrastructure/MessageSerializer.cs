using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    public static class MessageSerializer
    {
        private static readonly JsonSerializerSettings SETTINGS = new JsonSerializerSettings();

        static MessageSerializer()
        {
            SETTINGS.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        // TODO: Optimize serialization - GZIP, etc..
        public static byte[] Serialize(object payload)
        {
            var serialized = JsonConvert.SerializeObject(payload, SETTINGS);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public static T Deserialize<T>(byte[] raw)
        {
            if (raw == null)
            {
                return default(T);
            }

            var serialized = Encoding.UTF8.GetString(raw);
            return JsonConvert.DeserializeObject<T>(serialized, SETTINGS);
        }
    }
}
