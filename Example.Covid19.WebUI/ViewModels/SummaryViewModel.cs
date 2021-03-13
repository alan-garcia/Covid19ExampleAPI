using Example.Covid19.WebUI.DTO.SummaryCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista del resumen global y los casos por países con la última fecha actualizada de los datos
    /// </summary>
    public class SummaryViewModel
    {
        public Summary Summary { get; set; }
        public IEnumerable<CountryInfo> CountriesSummary { get; set; }
    }
}
