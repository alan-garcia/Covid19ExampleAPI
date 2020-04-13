using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Example.Covid19.WebUI.Controllers
{
    public class DayOneLiveController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneLiveController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new DayOneLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
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
                dayOneLiveViewModel.StatusTypeList = GetStatusTypeList();
                dayOneLiveViewModel.DayOneLive = dayOneLiveByCountryListOrdered;
            }
            else
            {
                dayOneLiveViewModel.Countries = await GetCountries();
                dayOneLiveViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", dayOneLiveViewModel);
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