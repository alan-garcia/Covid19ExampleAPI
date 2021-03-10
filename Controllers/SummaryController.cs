using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.SummaryCases;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using X.PagedList;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones sobre el resumen global y los casos por países 
    ///     con la última fecha actualizada de los datos
    /// </summary>
    public class SummaryController : BaseController
    {
        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public SummaryController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        /// <summary>
        ///     Obtiene toda la información relacionada con el resumen global y los casos por países
        /// </summary>
        /// <returns>La vista con el resumen global de los casos por países</returns>
        public async Task<ActionResult<Summary>> GetSummary(int? page)
        {
            Summary summary = await GetRequestData<Summary>(AppSettingsConfig.SUMMARY_KEY);
            int pageNumber = page ?? 1;

            ViewBag.SummaryCountriesPagedList = summary.Countries.ToPagedList(pageNumber, 15);

            return View("Summary", summary);
        }

    }
}