using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
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
    public class DayOneLiveController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneLiveController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new DayOneLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(DayOneLiveViewModel dayOneLiveViewModel)
        {
            if (!string.IsNullOrEmpty(dayOneLiveViewModel.Country))
            {
                var dayOneLiveUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_LIVE_KEY}")
                                    .Replace(COUNTRYNAME_PLACEHOLDER, dayOneLiveViewModel.Country)
                                    .Replace(STATUS_PLACEHOLDER, dayOneLiveViewModel.StatusType);

                var dayOneLiveByCountryList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);
                var dayOneLiveByCountryListOrdered = dayOneLiveByCountryList.OrderByDescending(day => day.Date.Date);

                dayOneLiveViewModel.Countries = await GetCountries();
                dayOneLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
                dayOneLiveViewModel.DayOneLive = dayOneLiveByCountryListOrdered;
            }
            else
            {
                dayOneLiveViewModel.Countries = await GetCountries();
                dayOneLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
            }

            return View("Index", dayOneLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

    }
}