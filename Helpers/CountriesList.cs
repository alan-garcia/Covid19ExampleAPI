using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Example.Covid19.WebUI.Helpers
{
    /// <summary>
    ///     Clase relacionada con la lista de países
    /// </summary>
    public static class CountriesList
    {
        /// <summary>
        ///     Convierte una lista de países en un elemento HTML de tipo "desplegable" para ser mostrado en la vista
        /// </summary>
        /// <param name="countries">Lista de países</param>
        /// <returns>La lista de los nombres de los países ordenadas alfabéticamente, para ser representado como un elemento HTML de tipo "option"
        /// en los formularios de tipo "lista desplegable"</returns>
        public static IEnumerable<SelectListItem> BuildAndGetCountriesSelectListItem(IEnumerable<Countries> countries)
        {
            List<Countries> byCountryOrderedList = countries.OrderBy(c => c.Country).ToList();

            return byCountryOrderedList
                .Select(c => new SelectListItem() { Text = c.Country, Value = c.Country })
                .OrderBy(c => c.Text);
        }
    }
}
