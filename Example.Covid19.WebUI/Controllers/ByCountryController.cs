using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    public class ByCountryController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(ByCountryViewModel byCountryViewModel)
        {
            if (ModelState.IsValid)
            {
                var byCountryUrl = ExtractPlaceholderUrlApi(byCountryViewModel);

                var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);

                var byCountryListFilter = ApplySearchFilter(byCountryList, byCountryViewModel);
                
                byCountryViewModel.ByCountry = byCountryListFilter;
            }

            byCountryViewModel.Countries = await GetCountries();
            byCountryViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(ByCountryViewModel byCountryViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, byCountryViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, byCountryViewModel.StatusType)
                            .Replace(DATEFROM_PLACEHOLDER, byCountryViewModel.DateFrom.ToString("yyyy-MM-dd"))
                            .Replace(DATETO_PLACEHOLDER, byCountryViewModel.DateTo.ToString("yyyy-MM-dd"));
        }

        private IEnumerable<ByCountry> ApplySearchFilter
            (IEnumerable<ByCountry> byCountryList, ByCountryViewModel byCountryViewModel)
        {
            return byCountryList
                    .Where(bc => bc.Country.Equals(byCountryViewModel.Country) && bc.Status.Equals(byCountryViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryViewModel.DateFrom && bc.Date <= byCountryViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }

    }
}
