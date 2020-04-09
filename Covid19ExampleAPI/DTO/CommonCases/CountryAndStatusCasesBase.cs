using System;

namespace Covid19ExampleAPI.DTO.CommonCases
{
    public class CountryAndStatusCasesBase
    {
        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public int Confirmed { get; set; }

        public int Deaths { get; set; }

        public int Recovered { get; set; }

        public int Active { get; set; }

        public DateTime Date { get; set; }

        public string LocationID { get; set; }
    }
}
