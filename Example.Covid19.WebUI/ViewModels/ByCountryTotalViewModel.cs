using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    public class ByCountryTotalViewModel : CovidBaseViewModel
    {
        [Required(ErrorMessage = "La fecha de inicio es obligatorio")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatorio")]
        public DateTime DateTo { get; set; }

        public IEnumerable<ByCountryTotal> ByCountryTotal { get; set; }
    }
}
