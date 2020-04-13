using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
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
    public class ByCountryLiveController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryLiveController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
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
                byCountryLiveViewModel.StatusTypeList = GetStatusTypeList();
                byCountryLiveViewModel.ByCountryLive = byCountryLiveListOrdered;
            }
            else
            {
                byCountryLiveViewModel.Countries = await GetCountries();
                byCountryLiveViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", byCountryLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await _apiService.GetAsync<IEnumerable<Countries>>
            (
                _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.COUNTRIES_KEY}")
            );

            var byCountryOrderedList = countries.OrderBy(c => c.Country).ToList();

            return byCountryOrderedList
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
