using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    public class ByCountryLiveViewModel
    {
        [Required(ErrorMessage = "El pais es obligatorio")]
        public string Country { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string StatusType { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatorio")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatorio")]
        public DateTime DateTo { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<ByCountryLive> ByCountryLive { get; set; }
    }
}
