using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.DTO.LiveByCountryCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones sobre los datos en directo de los países y sus estados
    /// </summary>
    public class LiveByCountryAndStatusController : BaseController
    {
        private string byCountryStatusCacheKey = "byCountryStatus";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        /// <param name="cache">La caché en memoria</param>
        public LiveByCountryAndStatusController(IApiService apiService, IConfiguration config, ICovid19MemoryCacheService cache) : base(apiService, config, cache)
        {
            _apiService = apiService;
            _config = config;
            _cache = cache;
        }

        /// <summary>
        ///     Obtiene la lista de todos los países
        /// </summary>
        /// <returns>La vista con la lista de los países</returns>
        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            if (!_cache.Get(byCountryStatusCacheKey, out LiveByCountryAndStatusViewModel byCountryStatusVM))
            {
                byCountryStatusVM = await GetCountriesViewModel<LiveByCountryAndStatusViewModel>();
                string byCountryStatusUrl = ExtractPlaceholderUrlApi(byCountryStatusVM);
                var liveByCountryAndStatusList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(byCountryStatusUrl);
                byCountryStatusVM.LiveByCountryAndStatus = ApplySearchFilter(liveByCountryAndStatusList, byCountryStatusVM);
            }

            return View(byCountryStatusVM);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos en directo de los países y sus estados
        /// </summary>
        /// <param name="byCountryStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de todos los casos en directo de los países que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(
            LiveByCountryAndStatusViewModel byCountryStatusViewModel)
        {
            if (ModelState.IsValid)
            {
                byCountryStatusCacheKey = $"{byCountryStatusCacheKey}_{byCountryStatusViewModel.Country}_{byCountryStatusViewModel.StatusType}";
                if (!_cache.Get(byCountryStatusCacheKey, out LiveByCountryAndStatusViewModel byCountryStatusVM))
                {
                    byCountryStatusVM = await GetCountriesViewModel<LiveByCountryAndStatusViewModel>();
                    string byCountryStatusUrl = ExtractPlaceholderUrlApi(byCountryStatusVM);
                    var byCountryStatusUrlList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(byCountryStatusUrl);
                    byCountryStatusVM.LiveByCountryAndStatus = ApplySearchFilter(byCountryStatusUrlList, byCountryStatusVM);

                    _cache.Set(byCountryStatusCacheKey, byCountryStatusVM);
                }

                byCountryStatusViewModel = byCountryStatusVM;
            }

            return View("Index", byCountryStatusViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="byCountryStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "live/country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusViewModel byCountryStatusViewModel)
        {
            string byCountryStatusApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_KEY);
            byCountryStatusViewModel.Country ??= "Spain";
            byCountryStatusViewModel.StatusType ??= "confirmed";

            return new StringBuilder(byCountryStatusApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, byCountryStatusViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, byCountryStatusViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los datos en directo de los países y sus estados
        /// </summary>
        /// <param name="byCountryStatusUrlList">La lista de países</param>
        /// <param name="byCountryStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con los datos en directo de los países, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<LiveByCountryAndStatus> ApplySearchFilter(IEnumerable<LiveByCountryAndStatus> byCountryStatusUrlList,
                                                                      LiveByCountryAndStatusViewModel byCountryStatusViewModel)
        {
            if (byCountryStatusViewModel.Country == null)
            {
                return byCountryStatusUrlList.OrderByDescending(bc => bc.Date.Date);
            }

            return byCountryStatusUrlList
                    .Where(live => live.Country.Equals(byCountryStatusViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
