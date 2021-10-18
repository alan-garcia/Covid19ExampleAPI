using Example.Covid19.API.DTO.SummaryCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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
        public async Task<ActionResult<SummaryViewModel>> GetSummary()
        {
            Summary summary = await GetRequestData<Summary>(AppSettingsConfig.SUMMARY_KEY);
            SummaryViewModel summaryViewModel = new()
            {
                Summary = summary,
                CountriesSummary = summary.Countries
            };

            return View("Index", summaryViewModel);
        }
    }
}
