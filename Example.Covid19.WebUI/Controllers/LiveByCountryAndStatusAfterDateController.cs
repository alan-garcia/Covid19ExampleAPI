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
    ///     después de una fecha dada
    /// </summary>
    public class LiveByCountryAndStatusAfterDateController : BaseController
    {
        private string byCountryStatusAfterDateCacheKey = "byCountryStatusAfterDate";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        /// <param name="cache">La caché en memoria</param>
        public LiveByCountryAndStatusAfterDateController(IApiService apiService, IConfiguration config, ICovid19MemoryCacheService cache) : base(apiService, config, cache)
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
            if (!_cache.Get(byCountryStatusAfterDateCacheKey, out LiveByCountryAndStatusAfterDateViewModel byCountryStatusAfterDateVM))
            {
                byCountryStatusAfterDateVM = await GetCountriesViewModel<LiveByCountryAndStatusAfterDateViewModel>();
                string byCountryStatusAfterDateUrl = ExtractPlaceholderUrlApi(byCountryStatusAfterDateVM);
                var byCountryStatusAfterDateList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(byCountryStatusAfterDateUrl);
                byCountryStatusAfterDateVM.LiveByCountryAndStatusAfterDate = ApplySearchFilter(byCountryStatusAfterDateList, byCountryStatusAfterDateVM);
            }

            return View(byCountryStatusAfterDateVM);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos en directo de los países y sus estados
        ///     después de una fecha dada
        /// </summary>
        /// <param name="byCountryStatusAfterDateViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de todos los casos en directo de los países que cumpla el criterio del estado 
        /// después de una fecha dada</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(
            LiveByCountryAndStatusAfterDateViewModel byCountryStatusAfterDateViewModel)
        {
            if (ModelState.IsValid)
            {
                byCountryStatusAfterDateCacheKey = $"{byCountryStatusAfterDateCacheKey}_{byCountryStatusAfterDateViewModel.Country}_{byCountryStatusAfterDateViewModel.StatusType}_{byCountryStatusAfterDateViewModel.Date.ToShortDateString()}";
                if (!_cache.Get(byCountryStatusAfterDateCacheKey, out LiveByCountryAndStatusAfterDateViewModel byCountryStatusAfterDateVM))
                {
                    byCountryStatusAfterDateVM = await GetCountriesViewModel<LiveByCountryAndStatusAfterDateViewModel>();
                    string byCountryStatusAfterDateUrl = ExtractPlaceholderUrlApi(byCountryStatusAfterDateVM);

                    var byCountryStatusAfterDateUrlList =
                        await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(byCountryStatusAfterDateUrl);
                    byCountryStatusAfterDateVM.LiveByCountryAndStatusAfterDate =
                        ApplySearchFilter(byCountryStatusAfterDateUrlList, byCountryStatusAfterDateVM);

                    _cache.Set(byCountryStatusAfterDateCacheKey, byCountryStatusAfterDateVM);
                }

                byCountryStatusAfterDateViewModel = byCountryStatusAfterDateVM;
            }

            return View("Index", byCountryStatusAfterDateViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="byCountryStatusAfterDateViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "live/country/status/date" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusAfterDateViewModel byCountryStatusAfterDateViewModel)
        {
            string byCountryStatusAfterDateApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_AFTERDATE_KEY);
            byCountryStatusAfterDateViewModel.Country ??= "Spain";
            byCountryStatusAfterDateViewModel.StatusType ??= "confirmed";

            return new StringBuilder(byCountryStatusAfterDateApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, byCountryStatusAfterDateViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, byCountryStatusAfterDateViewModel.StatusType)
                    .Replace(AppSettingsConfig.DATE_PLACEHOLDER, byCountryStatusAfterDateViewModel.Date.ToString("yyyy-MM-ddThh:mm:ssZ"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los datos en directo de los países y sus estados 
        ///     después de una fecha dada
        /// </summary>
        /// <param name="byCountryStatusAfterDateUrlList">La lista de países</param>
        /// <param name="byCountryStatusAfterDateViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con los datos en directo de los países y sus estados después de una fecha dada,
        /// ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<LiveByCountryAndStatusAfterDate> ApplySearchFilter
            (IEnumerable<LiveByCountryAndStatusAfterDate> byCountryStatusAfterDateUrlList,
             LiveByCountryAndStatusAfterDateViewModel byCountryStatusAfterDateViewModel)
        {
            if (byCountryStatusAfterDateViewModel.Country == null)
            {
                return byCountryStatusAfterDateUrlList.OrderByDescending(bc => bc.Date.Date);
            }

            return byCountryStatusAfterDateUrlList
                    .Where(live => live.Country.Equals(byCountryStatusAfterDateViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
