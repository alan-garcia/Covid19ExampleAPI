using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Example.Covid19.API.Helpers
{
    public static class JsonConfig
    {
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            var serializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver };

            return serializerSettings;
        }
    }
}
