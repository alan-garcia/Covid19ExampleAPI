using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.SummaryCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    public class SummaryController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        public SummaryController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        // GET: Summary
        public async Task<ActionResult<Summary>> GetSummary()
        {
            var summary = await _apiService.GetAsync<Summary>
            (
                _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.SUMMARY_KEY}")
            );

            return View("Summary", summary);
        }

        // GET: Summary/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

    }
}