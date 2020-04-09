﻿using System;

namespace Covid19ExampleAPI.DTO.SummaryCases
{
    public class Summary
    {
        public Global Global { get; set; }

        public CountryInfo[] Countries { get; set; }

        public DateTime Date { get; set; }
    }
}
