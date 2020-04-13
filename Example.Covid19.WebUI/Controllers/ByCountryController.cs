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
    public class ByCountryController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(ByCountryViewModel byCountryViewModel)
        {
            if (!string.IsNullOrEmpty(byCountryViewModel.Country) && !string.IsNullOrEmpty(byCountryViewModel.StatusType) &&
                !string.IsNullOrEmpty(byCountryViewModel.DateFrom.ToString()) && !string.IsNullOrEmpty(byCountryViewModel.DateTo.ToString()))
            {
                var byCountryUrl = _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_KEY}")
                                    .Replace(COUNTRYNAME_PLACEHOLDER, byCountryViewModel.Country)
                                    .Replace(STATUS_PLACEHOLDER, byCountryViewModel.StatusType)
                                    .Replace(DATEFROM_PLACEHOLDER, byCountryViewModel.DateFrom.ToString("dd/MM/yyyy"))
                                    .Replace(DATETO_PLACEHOLDER, byCountryViewModel.DateTo.ToString("dd/MM/yyyy"));

                var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);

                var byCountryListOrdered = byCountryList.Where(day => day.Country.Equals(byCountryViewModel.Country) && 
                        day.Status.Equals(byCountryViewModel.StatusType) && 
                        day.Date >= byCountryViewModel.DateFrom && day.Date <= byCountryViewModel.DateTo)
                    .OrderByDescending(day => day.Date.Date);

                byCountryViewModel.Countries = await GetCountries();
                byCountryViewModel.StatusTypeList = GetStatusTypeList();
                byCountryViewModel.ByCountry = byCountryListOrdered;
            }
            else
            {
                byCountryViewModel.Countries = await GetCountries();
                byCountryViewModel.StatusTypeList = GetStatusTypeList();
            }

            return View("Index", byCountryViewModel);
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
