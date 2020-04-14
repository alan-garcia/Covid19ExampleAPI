using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    public class LiveByCountryAndStatusAfterDateViewModel
    {
        [Required(ErrorMessage = "El pais es obligatorio")]
        public string Country { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string StatusType { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Date { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<LiveByCountryAndStatusAfterDate> LiveByCountryAndStatusAfterDate { get; set; }
    }
}
