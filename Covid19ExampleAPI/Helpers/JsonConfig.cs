using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19ExampleAPI.Models
{
    public class JsonConfig
    {
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            var serializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver };

            return serializerSettings;
        }
    }
}
