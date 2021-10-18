using Example.Covid19.API.DTO.CountriesCases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Example.Covid19.WebUI.Helpers
{
    public static class CountriesList
    {
        public static IEnumerable<SelectListItem> BuildAndGetCountriesSelectListItem(IEnumerable<Countries> countries)
        {
            List<Countries> byCountryOrderedList = countries.OrderBy(c => c.Country).ToList();

            return byCountryOrderedList
                .Select(c => new SelectListItem() { Text = c.Country, Value = c.Country })
                .OrderBy(c => c.Text);
        }
    }
}
