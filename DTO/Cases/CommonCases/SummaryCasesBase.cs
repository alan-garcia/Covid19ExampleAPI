using System;

namespace Example.Covid19.WebUI.DTO.Cases.CommonCases
{
    /// <summary>
    ///     Modelo que representan los campos comunes para el resumen de los casos de COVID-19
    /// </summary>
    public class SummaryCasesBase
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public int Cases { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
