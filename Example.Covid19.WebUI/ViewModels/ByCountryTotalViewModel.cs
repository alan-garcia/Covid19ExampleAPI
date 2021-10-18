using Example.Covid19.API.DTO.CountriesCases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista de todos los casos por tipo para un país
    /// </summary>
    public class ByCountryTotalViewModel : CovidBaseViewModel
    {
        public ByCountryTotalViewModel()
        {
            DateFrom = new DateTime(DateTime.Now.Year, 1, 1);
            DateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        [Required(ErrorMessage = "La fecha de inicio es obligatorio")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatorio")]
        public DateTime DateTo { get; set; }

        public IEnumerable<ByCountryTotal> ByCountryTotal { get; set; }
    }
}
