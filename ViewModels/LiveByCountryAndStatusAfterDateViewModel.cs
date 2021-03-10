using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista los datos en directo de los países y sus estados después de una fecha dada
    /// </summary>
    public class LiveByCountryAndStatusAfterDateViewModel : CovidBaseViewModel
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Date { get; set; }

        public IEnumerable<LiveByCountryAndStatusAfterDate> LiveByCountryAndStatusAfterDate { get; set; }
    }
}
