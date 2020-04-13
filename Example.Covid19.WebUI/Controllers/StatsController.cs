using System.Threading.Tasks;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.StatsCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Example.Covid19.WebUI.Controllers
{
    public class StatsController : BaseController
    {
        public StatsController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<Stat>> GetStats()
        {
            var stats = await GetRequestData<Stat>(AppSettingsConfig.STATS_KEY);

            return View("Stats", stats);
        }

    }
}