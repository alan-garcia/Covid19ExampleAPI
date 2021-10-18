using Example.Covid19.API.DTO.LiveByCountryCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista los datos en directo de los países y sus estados después de una fecha dada
    /// </summary>
    public class LiveByCountryAndStatusViewModel : CovidBaseViewModel
    {
        public IEnumerable<LiveByCountryAndStatus> LiveByCountryAndStatus { get; set; }
    }
}
