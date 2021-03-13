using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones sobre los datos en directo de los países y sus estados 
    ///     después de una fecha dada
    /// </summary>
    public class LiveByCountryAndStatusAfterDateController : BaseController
    {
        private const int PAGE_SIZE = 15;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public LiveByCountryAndStatusAfterDateController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            return View(await GetCountriesViewModel<LiveByCountryAndStatusAfterDateViewModel>());
        }

        /// <summary>
        ///     Obtiene la lista de todos los casos en directo de los países y sus estados después de una fecha dada
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos en directo de los países con sus estados, después de una fecha dada</returns>
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(int? page)
        {
            string liveByCountryAndStatusAfterDateSearchFilter = HttpContext.Session.GetString("LiveByCountryAndStatusAfterDateSearchFilter");
            IEnumerable<LiveByCountryAndStatusAfterDate> liveByCountryAndStatusAfterDateSearchFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateSearchFilter);

            int pageNumber = page ?? 1;
            var liveByCountryAndStatusAfterDateViewModel = await GetCountriesViewModel<LiveByCountryAndStatusAfterDateViewModel>();
            liveByCountryAndStatusAfterDateViewModel.LiveByCountryAndStatusAfterDate = 
                liveByCountryAndStatusAfterDateSearchFilterDeserialized.ToPagedList(pageNumber, PAGE_SIZE);

            return View("Index", liveByCountryAndStatusAfterDateViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos en directo de los países y sus estados
        ///     después de una fecha dada
        /// </summary>
        /// <param name="liveByCountryAndStatusAfterDateViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos en directo de los países que cumpla el criterio del estado 
        /// después de una fecha dada</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(
            LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string liveByCountryAndStatusAfterDateUrl = ExtractPlaceholderUrlApi(liveByCountryAndStatusAfterDateViewModel);

                IEnumerable<LiveByCountryAndStatusAfterDate> liveByCountryAndStatusAfterDateUrlList = 
                    await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateUrl);
                IEnumerable<LiveByCountryAndStatusAfterDate> liveByCountryAndStatusAfterDateSearchFilter = 
                    ApplySearchFilter(liveByCountryAndStatusAfterDateUrlList, liveByCountryAndStatusAfterDateViewModel);

                int pageNumber = page ?? 1;
                liveByCountryAndStatusAfterDateViewModel.LiveByCountryAndStatusAfterDate =
                    liveByCountryAndStatusAfterDateSearchFilter.ToPagedList(pageNumber, PAGE_SIZE);
                HttpContext.Session.SetString("LiveByCountryAndStatusAfterDateSearchFilter",
                                              JsonConvert.SerializeObject(liveByCountryAndStatusAfterDateSearchFilter));
            }

            liveByCountryAndStatusAfterDateViewModel.Countries = await GetCountries();
            liveByCountryAndStatusAfterDateViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", liveByCountryAndStatusAfterDateViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="liveByCountryAndStatusAfterDateViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "live/country/status/date" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel)
        {
            string liveByCountryAndStatusAfterDateApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_AFTERDATE_KEY);

            return new StringBuilder(liveByCountryAndStatusAfterDateApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.StatusType)
                    .Replace(AppSettingsConfig.DATE_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Date.ToString("yyyy-MM-ddThh:mm:ssZ"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los datos en directo de los países y sus estados 
        ///     después de una fecha dada
        /// </summary>
        /// <param name="liveByCountryAndStatusUrlList">La lista de países</param>
        /// <param name="liveByCountryAndStatusViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con los datos en directo de los países y sus estados después de una fecha dada,
        /// ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<LiveByCountryAndStatusAfterDate> ApplySearchFilter
            (IEnumerable<LiveByCountryAndStatusAfterDate> liveByCountryAndStatusAfterDateUrlList,
             LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel)
        {
            return liveByCountryAndStatusAfterDateUrlList
                    .Where(live => live.Country.Equals(liveByCountryAndStatusAfterDateViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
