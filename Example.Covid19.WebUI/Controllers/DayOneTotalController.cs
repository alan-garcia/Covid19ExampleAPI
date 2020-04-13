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
    public class DayOneTotalController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneTotalController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneTotalViewModel = new DayOneTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneTotalViewModel);
        }

        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(DayOneTotalViewModel dayOneTotalLiveViewModel)
        {
            if (!string.IsNullOrEmpty(dayOneTotalLiveViewModel.Country))
            {
                var dayOneTotalUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_TOTAL_KEY}")
                                    .Replace(COUNTRYNAME_PLACEHOLDER, dayOneTotalLiveViewModel.Country)
                                    .Replace(STATUS_PLACEHOLDER, dayOneTotalLiveViewModel.StatusType);

                var dayOneTotalByCountryList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);
                var dayOneTotalByCountryListOrdered = dayOneTotalByCountryList.OrderByDescending(day => day.Date.Date);

                dayOneTotalLiveViewModel.Countries = await GetCountries();
                dayOneTotalLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
                dayOneTotalLiveViewModel.DayOneTotal = dayOneTotalByCountryListOrdered;
            }
            else
            {
                dayOneTotalLiveViewModel.Countries = await GetCountries();
                dayOneTotalLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();
            }

            return View("Index", dayOneTotalLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

    }
}
