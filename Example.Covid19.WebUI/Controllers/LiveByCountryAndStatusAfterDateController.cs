using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    public class LiveByCountryAndStatusAfterDateController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATE_PLACEHOLDER = "{date}";

        public LiveByCountryAndStatusAfterDateController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new LiveByCountryAndStatusAfterDateViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(
            LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel)
        {
            if (!string.IsNullOrEmpty(liveByCountryAndStatusAfterDateViewModel.Country))
            {
                var liveByCountryAndStatusAfterDateUrl = _config
                    .GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_AFTERDATE_KEY}")
                    .Replace(COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.StatusType)
                    .Replace(DATE_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Date.ToString("yyyy-MM-ddThh:mm:ssZ"));

                var liveByCountryAndStatusAfterDateUrlList = await _apiService
                    .GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateUrl);

                var liveByCountryAndStatusAfterDateFilter = liveByCountryAndStatusAfterDateUrlList
                    .Where(day => day.Country.Equals(liveByCountryAndStatusAfterDateViewModel.Country))
                    .OrderByDescending(day => day.Date.Date);

                liveByCountryAndStatusAfterDateViewModel.Countries = await GetCountries();
                liveByCountryAndStatusAfterDateViewModel.StatusTypeList = GetStatusTypeList();
                liveByCountryAndStatusAfterDateViewModel.LiveByCountryAndStatusAfterDate = liveByCountryAndStatusAfterDateFilter;
            }
            else
            {
                liveByCountryAndStatusAfterDateViewModel.Countries = await GetCountries();
                liveByCountryAndStatusAfterDateViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", liveByCountryAndStatusAfterDateViewModel);
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
