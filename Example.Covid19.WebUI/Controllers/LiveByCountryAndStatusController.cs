using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
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
    public class LiveByCountryAndStatusController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public LiveByCountryAndStatusController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new LiveByCountryAndStatusViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(
            LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            if (!string.IsNullOrEmpty(liveByCountryAndStatusViewModel.Country))
            {
                var liveByCountryAndStatusUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_KEY}")
                    .Replace(COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, liveByCountryAndStatusViewModel.StatusType);

                var liveByCountryAndStatusUrlList = await _apiService
                    .GetAsync<IEnumerable<LiveByCountryAndStatus>>(liveByCountryAndStatusUrl);

                var liveByCountryAndStatusFilter = liveByCountryAndStatusUrlList
                    .Where(day => day.Country.Equals(liveByCountryAndStatusViewModel.Country))
                    .OrderByDescending(day => day.Date.Date);

                liveByCountryAndStatusViewModel.Countries = await GetCountries();
                liveByCountryAndStatusViewModel.StatusTypeList = GetStatusTypeList();
                liveByCountryAndStatusViewModel.LiveByCountryAndStatus = liveByCountryAndStatusFilter;
            }
            else
            {
                liveByCountryAndStatusViewModel.Countries = await GetCountries();
                liveByCountryAndStatusViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", liveByCountryAndStatusViewModel);
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
