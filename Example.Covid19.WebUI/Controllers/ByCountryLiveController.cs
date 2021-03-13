using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país (en directo)
    /// </summary>
    public class ByCountryLiveController : BaseController
    {
        private const int PAGE_SIZE = 15;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public ByCountryLiveController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            return View(await GetCountriesViewModel<ByCountryLiveViewModel>());
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país (en directo)
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(int? page)
        {
            string byCountryLiveSearchFilter = HttpContext.Session.GetString("ByCountryLiveSearchFilter");
            IEnumerable<ByCountryLive> byCountryLiveSearchFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<ByCountryLive>>(byCountryLiveSearchFilter);

            int pageNumber = page ?? 1;
            var byCountryLiveViewModel = await GetCountriesViewModel<ByCountryLiveViewModel>();
            byCountryLiveViewModel.ByCountryLive = byCountryLiveSearchFilterDeserialized.ToPagedList(pageNumber, PAGE_SIZE);

            return View("Index", byCountryLiveViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país (en directo)
        /// </summary>
        /// <param name="byCountryLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos del país indicado que cumpla el criterio del estado 
        /// y el rango de fechas</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(ByCountryLiveViewModel byCountryLiveViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string byCountryLiveUrl = ExtractPlaceholderUrlApi(byCountryLiveViewModel);
                IEnumerable<ByCountryLive> byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);
                IEnumerable<ByCountryLive> byCountryLiveSearchFilter = ApplySearchFilter(byCountryLiveList, byCountryLiveViewModel);

                var pageNumber = page ?? 1;
                byCountryLiveViewModel.ByCountryLive = byCountryLiveSearchFilter.ToPagedList(pageNumber, PAGE_SIZE);
                HttpContext.Session.SetString("ByCountryLiveSearchFilter", JsonConvert.SerializeObject(byCountryLiveSearchFilter));
            }

            byCountryLiveViewModel.Countries = await GetCountries();
            byCountryLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryLiveViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="byCountryLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "country/status/live" con los parámetros de búsqueda sustituídos</returns>
        public string ExtractPlaceholderUrlApi(ByCountryLiveViewModel byCountryLiveViewModel)
        {
            string byCountryLiveApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.BY_COUNTRY_LIVE_KEY);

            return new StringBuilder(byCountryLiveApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, byCountryLiveViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, byCountryLiveViewModel.StatusType)
                    .Replace(AppSettingsConfig.DATEFROM_PLACEHOLDER, byCountryLiveViewModel.DateFrom.ToString("yyyy-MM-dd"))
                    .Replace(AppSettingsConfig.DATETO_PLACEHOLDER, byCountryLiveViewModel.DateTo.ToString("yyyy-MM-dd"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país (en directo)
        /// </summary>
        /// <param name="byCountryLiveList">La lista de países (en directo)</param>
        /// <param name="byCountryLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país y el rango de fechas seleccionadas en la búsqueda, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<ByCountryLive> ApplySearchFilter(IEnumerable<ByCountryLive> byCountryLiveList,
                                                             ByCountryLiveViewModel byCountryLiveViewModel)
        {
            return byCountryLiveList
                    .Where(bc => bc.Country.Equals(byCountryLiveViewModel.Country) && bc.Status.Equals(byCountryLiveViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryLiveViewModel.DateFrom && bc.Date <= byCountryLiveViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }
    }
}
