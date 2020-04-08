using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19ExampleAPI.Models;
using Covid19ExampleAPI.Models.Countries;
using Covid19ExampleAPI.Models.Stats;
using Covid19ExampleAPI.Models.Summary;
using Covid19ExampleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covid19ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        private readonly IApiService _apiService;
        private readonly IOptions<CovidApisAppSettingsModel> _appSettings;

        public Covid19Controller(IApiService apiService, IOptions<CovidApisAppSettingsModel> appSettings)
        {
            _apiService = apiService;
            _appSettings = appSettings;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            return await _apiService.GetAsync<Summary>(_appSettings.Value.Summary);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            var allCountriesList = await _apiService.GetAsync<IEnumerable<Country>>(_appSettings.Value.Countries);

            return allCountriesList.ToList();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<Stat>> GetStats()
        {
            var stats = await _apiService.GetAsync<Stat>(_appSettings.Value.Stats);

            return stats;
        }
    }
}
