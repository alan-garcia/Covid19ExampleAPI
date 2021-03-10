using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista de los casos por tipo para un país
    /// </summary>
    public class ByCountryViewModel : CovidBaseViewModel
    {
        [Required(ErrorMessage = "La fecha de inicio es obligatorio")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatorio")]
        public DateTime DateTo { get; set; }

        public IEnumerable<ByCountry> ByCountry { get; set; }
    }
}
