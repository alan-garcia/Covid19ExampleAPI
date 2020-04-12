using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class ByCountryLiveViewModel
    {
        public string Country { get; set; }

        public string StatusType { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<ByCountryLive> ByCountryLive { get; set; }
    }
}
