using System;

namespace Example.Covid19.WebUI.DTO.SummaryCases
{
    public class Summary
    {
        public Global Global { get; set; }

        public CountryInfo[] Countries { get; set; }

        public DateTime Date { get; set; }
    }
}
