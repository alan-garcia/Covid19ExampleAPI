using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
    /// </summary>
    public class DayOneLiveController : BaseController
    {
        private const int PAGE_SIZE = 15;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public DayOneLiveController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            return View(await GetCountriesViewModel<DayOneLiveViewModel>());
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos desde el primer caso de COVID (en directo), filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(int? page)
        {
            string dayOneLiveSearchFilter = HttpContext.Session.GetString("DayOneLiveListFilter");
            IEnumerable<DayOneLive> dayOneLiveSearchFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<DayOneLive>>(dayOneLiveSearchFilter);

            int pageNumber = page ?? 1;
            var dayOneLiveViewModel = await GetCountriesViewModel<DayOneLiveViewModel>();
            dayOneLiveViewModel.DayOneLive = dayOneLiveSearchFilterDeserialized.ToPagedList(pageNumber, PAGE_SIZE);

            return View("Index", dayOneLiveViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="dayOneLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(DayOneLiveViewModel dayOneLiveViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string dayOneLiveUrl = ExtractPlaceholderUrlApi(dayOneLiveViewModel);
                IEnumerable<DayOneLive> dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);
                IEnumerable<DayOneLive> dayOneLiveSearchFilter = ApplySearchFilter(dayOneLiveList, dayOneLiveViewModel);

                int pageNumber = page ?? 1;
                dayOneLiveViewModel.DayOneLive = dayOneLiveSearchFilter.ToPagedList(pageNumber, PAGE_SIZE);
                HttpContext.Session.SetString("DayOneLiveListFilter", JsonConvert.SerializeObject(dayOneLiveSearchFilter));
            }

            dayOneLiveViewModel.Countries = await GetCountries();
            dayOneLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneLiveViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="dayOneLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "dayone/country/status/live" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(DayOneLiveViewModel dayOneLiveViewModel)
        {
            string dayOneLiveApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.DAYONE_LIVE_KEY);

            return new StringBuilder(dayOneLiveApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, dayOneLiveViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, dayOneLiveViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país desde el 
        ///     primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="dayOneLiveByCountryList">La lista de países</param>
        /// <param name="dayOneLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país desde el primer caso de COVID, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<DayOneLive> ApplySearchFilter(IEnumerable<DayOneLive> dayOneLiveByCountryList,
                                                          DayOneLiveViewModel dayOneLiveViewModel)
        {
            return dayOneLiveByCountryList
                    .Where(day => day.Country.Equals(dayOneLiveViewModel.Country) && day.Status.Equals(dayOneLiveViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}