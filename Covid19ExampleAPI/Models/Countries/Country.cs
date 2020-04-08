using Newtonsoft.Json;

namespace Covid19ExampleAPI.Models.Countries
{
    public class Country
    {
        [JsonProperty("Country")]
        public string Name { get; set; }

        public string Slug { get; set; }

        public string ISO2 { get; set; }
    }
}
