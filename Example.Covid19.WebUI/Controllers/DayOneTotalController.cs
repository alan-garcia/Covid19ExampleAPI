using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
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
    public class DayOneTotalController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneTotalController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneTotalViewModel = new DayOneTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
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
                dayOneTotalLiveViewModel.StatusTypeList = GetStatusTypeList();
                dayOneTotalLiveViewModel.DayOneTotal = dayOneTotalByCountryListOrdered;
            }
            else
            {
                dayOneTotalLiveViewModel.Countries = await GetCountries();
                dayOneTotalLiveViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", dayOneTotalLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await _apiService.GetAsync<IEnumerable<Countries>>
            (
                _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.COUNTRIES_KEY}")
            );

            var dayOneByCountryOrderedList = countries.OrderBy(c => c.Country).ToList();

            return countries
                .Select(c => new SelectListItem() { Text = c.Country, Value = c.Country })
                .OrderBy(c => c.Text);
        }

        private List<SelectListItem> GetStatusTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() { Text = "Confirmados", Value = "confirmed" },
                new SelectListItem() { Text = "Recuperados", Value = "recovered" },
                new SelectListItem() { Text = "Muertos", Value = "deaths" }
            };
        }
    }
}
