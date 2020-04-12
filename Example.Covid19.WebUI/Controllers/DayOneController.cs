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
    public class DayOneController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        // GET: DayOne
        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneViewModel = new DayOneViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
            };

            return View(dayOneViewModel);
        }

        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(DayOneViewModel dayOneViewModel)
        {
            if(!string.IsNullOrEmpty(dayOneViewModel.Country) && !string.IsNullOrEmpty(dayOneViewModel.StatusType))
            {
                var dayOneUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_KEY}")
                                .Replace(COUNTRYNAME_PLACEHOLDER, dayOneViewModel.Country)
                                .Replace(STATUS_PLACEHOLDER, dayOneViewModel.StatusType);

                var dayOneByCountryList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);
                var dayOneByCountryListOrdered = dayOneByCountryList.OrderByDescending(day => day.Date.Date);

                dayOneViewModel.Countries = await GetCountries();
                dayOneViewModel.StatusTypeList = GetStatusTypeList();
                dayOneViewModel.DayOne = dayOneByCountryListOrdered;
            }
            else
            {
                dayOneViewModel.Countries = await GetCountries();
                dayOneViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", dayOneViewModel);
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