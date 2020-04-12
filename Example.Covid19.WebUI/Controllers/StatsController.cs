using System.Threading.Tasks;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.StatsCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Example.Covid19.WebUI.Controllers
{
    public class StatsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        public StatsController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<Stat>> GetStats()
        {
            var stats = await _apiService.GetAsync<Stat>
            (
                _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.STATS_KEY}")
            );

            return View("Stats", stats);
        }
    }
}