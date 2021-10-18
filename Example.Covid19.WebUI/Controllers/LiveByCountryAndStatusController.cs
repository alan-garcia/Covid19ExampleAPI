using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.DTO.LiveByCountryCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.Helpers;
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
        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public LiveByCountryAndStatusController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        /// <summary>
        ///     Obtiene la lista de todos los países
        /// </summary>
        /// <returns>La vista con la lista de los países</returns>
        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var liveByCountryAndStatusViewModel = await GetCountriesViewModel<LiveByCountryAndStatusViewModel>();
            string liveByCountryAndStatusUrl = ExtractPlaceholderUrlApi(liveByCountryAndStatusViewModel);
            var liveByCountryAndStatusList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(liveByCountryAndStatusUrl);
            var liveByCountryAndStatusSearchFilter = ApplySearchFilter(liveByCountryAndStatusList, liveByCountryAndStatusViewModel);
            liveByCountryAndStatusViewModel.LiveByCountryAndStatus = liveByCountryAndStatusSearchFilter;

            return View(liveByCountryAndStatusViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos en directo de los países y sus estados
        /// </summary>
        /// <param name="liveByCountryAndStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de todos los casos en directo de los países que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(
            LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            if (ModelState.IsValid)
            {
                string liveByCountryAndStatusUrl = ExtractPlaceholderUrlApi(liveByCountryAndStatusViewModel);
                var liveByCountryAndStatusUrlList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(liveByCountryAndStatusUrl);
                var liveByCountryAndStatusSearchFilter = ApplySearchFilter(liveByCountryAndStatusUrlList, liveByCountryAndStatusViewModel);

                liveByCountryAndStatusViewModel.LiveByCountryAndStatus = liveByCountryAndStatusSearchFilter;
            }

            liveByCountryAndStatusViewModel.Countries = await GetCountries();
            liveByCountryAndStatusViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", liveByCountryAndStatusViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="liveByCountryAndStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "live/country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            string liveByCountryAndStatusApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_KEY);
            liveByCountryAndStatusViewModel.Country ??= "Spain";
            liveByCountryAndStatusViewModel.StatusType ??= "confirmed";

            return new StringBuilder(liveByCountryAndStatusApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, liveByCountryAndStatusViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los datos en directo de los países y sus estados
        /// </summary>
        /// <param name="liveByCountryAndStatusUrlList">La lista de países</param>
        /// <param name="liveByCountryAndStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con los datos en directo de los países, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<LiveByCountryAndStatus> ApplySearchFilter(IEnumerable<LiveByCountryAndStatus> liveByCountryAndStatusUrlList,
                                                                      LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            if (liveByCountryAndStatusViewModel.Country == null)
            {
                return liveByCountryAndStatusUrlList.OrderByDescending(bc => bc.Date.Date);
            }

            return liveByCountryAndStatusUrlList
                    .Where(live => live.Country.Equals(liveByCountryAndStatusViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
