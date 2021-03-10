using System;

namespace Covid19ExampleAPI.DTO.Cases.CommonCases
{
    /// <summary>
    ///     Modelo que representan los campos comunes para el apartado de los países y los estados
    /// </summary>
    public class CountryAndStatusCasesBase
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public DateTime Date { get; set; }
    }
}
