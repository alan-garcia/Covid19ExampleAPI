using System;

namespace Covid19ExampleAPI.DTO.Cases.SummaryCases
{
    /// <summary>
    ///     Modelo que representan los casos de COVID-19 por países
    /// </summary>
    public class CountryInfo
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Slug { get; set; }
        public int NewConfirmed { get; set; }
        public int TotalConfirmed { get; set; }
        public int NewDeaths { get; set; }
        public int TotalDeaths { get; set; }
        public int NewRecovered { get; set; }
        public int TotalRecovered { get; set; }
        public DateTime Date { get; set; }
    }
}
