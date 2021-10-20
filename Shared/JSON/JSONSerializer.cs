using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Common.JSON
{
    public static class JSONSerializer
    {
        private static readonly JsonSerializer _serializer;
        private static readonly JsonSerializerSettings _settings;

        static JSONSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false
                    }
                }
            };

            _settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            _serializer = JsonSerializer.Create(_settings);
        }

        public static JObject Serialize(object @object)
        {
            return JObject.FromObject(@object, _serializer);
        }

        public static JObject Deserialize(string serializedJSON)
        {
            return JObject.Parse(serializedJSON);
        }

        public static T Deserialize<T>(JObject json)
        {
            return json.ToObject<T>();
        }

        public static object ToObject(Type type, JObject source)
        {
            return source.ToObject(type);
        }
    }
}
