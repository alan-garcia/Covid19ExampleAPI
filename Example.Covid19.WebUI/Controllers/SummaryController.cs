using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.SummaryCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using X.PagedList;

namespace Example.Covid19.WebUI.Controllers
{
    public class SummaryController : BaseController
    {
        public SummaryController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<Summary>> GetSummary(int? page)
        {
            var summary = await GetRequestData<Summary>(AppSettingsConfig.SUMMARY_KEY);
            var pageNumber = page ?? 1;

            ViewBag.SummaryCountries = summary.Countries.ToPagedList(pageNumber, 15);

            return View("Summary", summary);
        }

    }
}