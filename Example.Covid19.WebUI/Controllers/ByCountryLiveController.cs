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
    public class ByCountryLiveController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryLiveController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(ByCountryLiveViewModel byCountryLiveViewModel)
        {
            if (!string.IsNullOrEmpty(byCountryLiveViewModel.Country) && !string.IsNullOrEmpty(byCountryLiveViewModel.StatusType) &&
                !string.IsNullOrEmpty(byCountryLiveViewModel.DateFrom.ToString()) && !string.IsNullOrEmpty(byCountryLiveViewModel.DateTo.ToString()))
            {
                var byCountryLiveUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_LIVE_KEY}")
                                    .Replace(COUNTRYNAME_PLACEHOLDER, byCountryLiveViewModel.Country)
                                    .Replace(STATUS_PLACEHOLDER, byCountryLiveViewModel.StatusType)
                                    .Replace(DATEFROM_PLACEHOLDER, byCountryLiveViewModel.DateFrom.ToString("dd/MM/yyyy"))
                                    .Replace(DATETO_PLACEHOLDER, byCountryLiveViewModel.DateTo.ToString("dd/MM/yyyy"));

                var byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);

                var byCountryLiveListOrdered = byCountryLiveList.Where(day => day.Country.Equals(byCountryLiveViewModel.Country) &&
                        day.Status.Equals(byCountryLiveViewModel.StatusType) &&
                        day.Date >= byCountryLiveViewModel.DateFrom && day.Date <= byCountryLiveViewModel.DateTo)
                    .OrderByDescending(day => day.Date.Date);

                byCountryLiveViewModel.Countries = await GetCountries();
                byCountryLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
                byCountryLiveViewModel.ByCountryLive = byCountryLiveListOrdered;
            }
            else
            {
                byCountryLiveViewModel.Countries = await GetCountries();
                byCountryLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
            }

            return View("Index", byCountryLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

    }
}
