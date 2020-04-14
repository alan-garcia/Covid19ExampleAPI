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
    public class DayOneController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneViewModel = new DayOneViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(DayOneViewModel dayOneViewModel)
        {
            if (ModelState.IsValid)
            {
                var dayOneUrl = ExtractPlaceholderUrlApi(dayOneViewModel);

                var dayOneByCountryList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);

                var dayOneByCountryListFilter = ApplySearchFilter(dayOneByCountryList, dayOneViewModel);

                dayOneViewModel.DayOne = dayOneByCountryListFilter;
            }

            dayOneViewModel.Countries = await GetCountries();
            dayOneViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(DayOneViewModel dayOneViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, dayOneViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, dayOneViewModel.StatusType);
        }

        private IEnumerable<DayOne> ApplySearchFilter
            (IEnumerable<DayOne> dayOneByCountryList, DayOneViewModel dayOneViewModel)
        {
            return dayOneByCountryList
                    .Where(day => day.Country.Equals(dayOneViewModel.Country) && day.Status.Equals(dayOneViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}