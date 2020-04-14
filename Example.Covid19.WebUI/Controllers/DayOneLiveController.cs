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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(DayOneLiveViewModel dayOneLiveViewModel)
        {
            if (ModelState.IsValid)
            {
                var dayOneLiveUrl = ExtractPlaceholderUrlApi(dayOneLiveViewModel);

                var dayOneLiveByCountryList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);

                var dayOneLiveByCountryListFilter = ApplySearchFilter(dayOneLiveByCountryList, dayOneLiveViewModel);

                dayOneLiveViewModel.DayOneLive = dayOneLiveByCountryListFilter;
            }

            dayOneLiveViewModel.Countries = await GetCountries();
            dayOneLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(DayOneLiveViewModel dayOneLiveViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_LIVE_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, dayOneLiveViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, dayOneLiveViewModel.StatusType);
        }

        private IEnumerable<DayOneLive> ApplySearchFilter
            (IEnumerable<DayOneLive> dayOneLiveByCountryList, DayOneLiveViewModel dayOneLiveViewModel)
        {
            return dayOneLiveByCountryList
                    .Where(day => day.Country.Equals(dayOneLiveViewModel.Country) && day.Status.Equals(dayOneLiveViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}