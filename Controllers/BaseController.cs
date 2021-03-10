using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones comunes para todos los controladores
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected IApiService _apiService;
        protected IConfiguration _config;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        protected BaseController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        /// <summary>
        ///     Obtiene los datos de la API con base en una URL concreta
        /// </summary>
        /// <typeparam name="T">API concreta a consumir</typeparam>
        /// <param name="apiUrl">URL concreta de la API</param>
        /// <returns></returns>
        protected async Task<T> GetRequestData<T>(string apiUrl) where T : class
        {
            var requestData = await _apiService.GetAsync<T>
            (
                _config.GetValue<string>($"{ AppSettingsConfig.COVID19API_KEY }:{ apiUrl }")
            );

            return requestData;
        }
    }
}