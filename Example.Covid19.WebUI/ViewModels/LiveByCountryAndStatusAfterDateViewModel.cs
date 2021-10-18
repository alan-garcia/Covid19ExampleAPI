using Example.Covid19.API.DTO.LiveByCountryCases;
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
        public LiveByCountryAndStatusAfterDateViewModel()
        {
            Date = new DateTime(DateTime.Now.Year, 1, 1);
        }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Date { get; set; }

        public IEnumerable<LiveByCountryAndStatusAfterDate> LiveByCountryAndStatusAfterDate { get; set; }
    }
}
