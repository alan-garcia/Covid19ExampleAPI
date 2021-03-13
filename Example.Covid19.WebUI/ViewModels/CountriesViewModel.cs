using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista del menú de los países
    /// </summary>
    public class CountriesViewModel
    {
        public IEnumerable<Countries> Countries { get; set; }
    }
}
