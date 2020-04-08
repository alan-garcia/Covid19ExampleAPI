using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19ExampleAPI.Models;
using Covid19ExampleAPI.Models.Countries;
using Covid19ExampleAPI.Models.Stats;
using Covid19ExampleAPI.Models.Summary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covid19ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        private readonly IOptions<CovidApisAppSettingsModel> _appSettings;

        public Covid19Controller(IOptions<CovidApisAppSettingsModel> appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            var apiService = new ApiService();
            var summaryInfo = await apiService.GetAsync<Summary>(_appSettings.Value.Summary);

            return summaryInfo;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            var apiService = new ApiService();
            var allCountriesList = await apiService.GetAsync<IEnumerable<Country>>(_appSettings.Value.Countries);

            return allCountriesList.ToList();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<Stat>> GetStats()
        {
            var apiService = new ApiService();
            var stats = await apiService.GetAsync<Stat>(_appSettings.Value.Stats);

            return stats;
        }
    }
}
