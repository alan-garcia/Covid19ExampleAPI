using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.CountriesCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Example.Covid19.WebUI.Controllers
{
    public class CountriesController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        public CountriesController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> GetCountries()
        {
            var countries = await _apiService.GetAsync<IEnumerable<Countries>>
            (
                _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.COUNTRIES_KEY}")
            );

            return View("Countries", countries);
        }
    }
}