using System.Threading.Tasks;
using Covid19ExampleAPI.Models;
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

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            var apiService = new ApiService();
            var summaryInfo = await apiService.GetAsync<Summary>(_appSettings.Value.Summary);

            return summaryInfo;
        }
    }
}
