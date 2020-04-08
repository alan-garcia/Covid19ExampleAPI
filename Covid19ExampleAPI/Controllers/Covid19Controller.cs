using System.Threading.Tasks;
using Covid19ExampleAPI.Models.Summary;
using Microsoft.AspNetCore.Mvc;

namespace Covid19ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            var apiService = new ApiService();
            var summaryInfo = await apiService.GetAsync<Summary>(@"https://api.covid19api.com/summary");

            return summaryInfo;
        }
    }
}
