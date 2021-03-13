using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneController : BaseController
    {
        private const int PAGE_SIZE = 15;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public DayOneController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            return View(await GetCountriesViewModel<DayOneViewModel>());
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos desde el primer caso de COVID, filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(int? page)
        {
            string dayOneSearchFilter = HttpContext.Session.GetString("DayOneSearchFilter");
            IEnumerable<DayOne> dayOneSearchFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<DayOne>>(dayOneSearchFilter);

            int pageNumber = page ?? 1;
            var dayOneViewModel = await GetCountriesViewModel<DayOneViewModel>();
            dayOneViewModel.DayOne = dayOneSearchFilterDeserialized.ToPagedList(pageNumber, PAGE_SIZE);

            return View("Index", dayOneViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(DayOneViewModel dayOneViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string dayOneUrl = ExtractPlaceholderUrlApi(dayOneViewModel);
                IEnumerable<DayOne> dayOneList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);
                IEnumerable<DayOne> dayOneSearchFilter = ApplySearchFilter(dayOneList, dayOneViewModel);

                int pageNumber = page ?? 1;
                dayOneViewModel.DayOne = dayOneSearchFilter.ToPagedList(pageNumber, PAGE_SIZE);
                HttpContext.Session.SetString("DayOneSearchFilter", JsonConvert.SerializeObject(dayOneSearchFilter));
            }

            dayOneViewModel.Countries = await GetCountries();
            dayOneViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "dayone/country/status" con los parámetros de búsqueda sustituídos</returns>
        public string ExtractPlaceholderUrlApi(DayOneViewModel dayOneViewModel)
        {
            string dayOneApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.DAYONE_KEY);

            return new StringBuilder(dayOneApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, dayOneViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, dayOneViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneByCountryList">La lista de países</param>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país desde el primer caso de COVID, ordenadas de fechas más recientes a más antiguas</returns>
        public IEnumerable<DayOne> ApplySearchFilter(IEnumerable<DayOne> dayOneByCountryList,
                                                      DayOneViewModel dayOneViewModel)
        {
            return dayOneByCountryList
                    .Where(day => day.Country.Equals(dayOneViewModel.Country) && day.Status.Equals(dayOneViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }
    }
}