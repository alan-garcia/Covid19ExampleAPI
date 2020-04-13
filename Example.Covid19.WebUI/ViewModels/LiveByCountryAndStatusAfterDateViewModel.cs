using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class LiveByCountryAndStatusAfterDateViewModel
    {
        public string Country { get; set; }

        public string StatusType { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<LiveByCountryAndStatusAfterDate> LiveByCountryAndStatusAfterDate { get; set; }
    }
}
