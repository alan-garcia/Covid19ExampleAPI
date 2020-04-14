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
    public class ByCountryTotalController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryTotalController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(ByCountryTotalViewModel byCountryTotalViewModel)
        {
            if (ModelState.IsValid)
            {
                var byCountryTotalUrl = ExtractPlaceholderUrlApi(byCountryTotalViewModel);

                var byCountryTotalList = await _apiService.GetAsync<IEnumerable<ByCountryTotal>>(byCountryTotalUrl);

                var byCountryTotalListFilter = ApplySearchFilter(byCountryTotalList, byCountryTotalViewModel);

                byCountryTotalViewModel.ByCountryTotal = byCountryTotalListFilter;
            }

            byCountryTotalViewModel.Countries = await GetCountries();
            byCountryTotalViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryTotalViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(ByCountryTotalViewModel byCountryTotalViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_TOTAL_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, byCountryTotalViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, byCountryTotalViewModel.StatusType)
                            .Replace(DATEFROM_PLACEHOLDER, byCountryTotalViewModel.DateFrom.ToString("dd/MM/yyyy"))
                            .Replace(DATETO_PLACEHOLDER, byCountryTotalViewModel.DateTo.ToString("dd/MM/yyyy"));
        }

        private IEnumerable<ByCountryTotal> ApplySearchFilter
            (IEnumerable<ByCountryTotal> byCountryTotalList, ByCountryTotalViewModel byCountryTotalViewModel)
        {
            return byCountryTotalList
                    .Where(bc => bc.Country.Equals(byCountryTotalViewModel.Country) && bc.Status.Equals(byCountryTotalViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryTotalViewModel.DateFrom && bc.Date <= byCountryTotalViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }

    }
}
