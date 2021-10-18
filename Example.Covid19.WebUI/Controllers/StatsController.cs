using Example.Covid19.API.DTO.StatsCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones sobre las estadísticas globales de los casos de COVID-19
    /// </summary>
    public class StatsController : BaseController
    {
        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public StatsController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        /// <summary>
        ///     Obtiene toda la información relacionada con las estadísticas globales de los casos de COVID-19
        /// </summary>
        /// <returns>La vista con las estadísticas globales de los casos de COVID-19</returns>
        public async Task<ActionResult<StatViewModel>> GetStats()
        {
            Stat stats = await GetRequestData<Stat>(AppSettingsConfig.STATS_KEY);
            StatViewModel statViewModel = new()
            {
                Stat = stats
            };

            return View("Index", statViewModel);
        }
    }
}
