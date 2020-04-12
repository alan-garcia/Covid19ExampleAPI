using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class DayOneTotalViewModel
    {
        public string Country { get; set; }

        public string StatusType { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<DayOneTotal> DayOneTotal { get; set; }
    }
}
