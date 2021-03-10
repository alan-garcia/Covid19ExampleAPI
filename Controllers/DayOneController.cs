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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

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
            var dayOneViewModel = new DayOneViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneViewModel);
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos desde el primer caso de COVID, filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(int? page)
        {
            string dayOneByCountryListFilter = HttpContext.Session.GetString("DayOneByCountryListFilter");
            IEnumerable<DayOne> dayOneByCountryListFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<DayOne>>(dayOneByCountryListFilter);

            int pageNumber = page ?? 1;
            HttpContext.Session.SetString("DayOneByCountryListFilter", JsonConvert.SerializeObject(dayOneByCountryListFilterDeserialized));

            ViewBag.DayOneByCountryListFilter = dayOneByCountryListFilterDeserialized.ToPagedList(pageNumber, 15);

            DayOneViewModel dayOneViewModel = new DayOneViewModel
            {
                DayOne = dayOneByCountryListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

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
                IEnumerable<DayOne> dayOneByCountryList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);
                IEnumerable<DayOne> dayOneByCountryListFilter = ApplySearchFilter(dayOneByCountryList, dayOneViewModel);

                dayOneViewModel.DayOne = dayOneByCountryListFilter;

                int pageNumber = page ?? 1;
                HttpContext.Session.SetString("DayOneByCountryListFilter", JsonConvert.SerializeObject(dayOneByCountryListFilter));

                ViewBag.DayOneByCountryListFilter = dayOneByCountryListFilter.ToPagedList(pageNumber, 15);
            }

            dayOneViewModel.Countries = await GetCountries();
            dayOneViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneViewModel);
        }

        /// <summary>
        ///     Obtiene todos los datos relacionados con el país
        /// </summary>
        /// <returns>La lista de países en un elemento HTML de tipo "desplegable" para ser mostrado en la vista</returns>
        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            IEnumerable<Countries> countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "dayone/country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(DayOneViewModel dayOneViewModel)
        {
            string dayOneApiUrlPlaceHolder = _config.GetValue<string>(
                $"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_KEY}"
            );

            return new StringBuilder(dayOneApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, dayOneViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, dayOneViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneByCountryList">La lista de países</param>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país desde el primer caso de COVID, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<DayOne> ApplySearchFilter(IEnumerable<DayOne> dayOneByCountryList,
                                                      DayOneViewModel dayOneViewModel)
        {
            return dayOneByCountryList
                    .Where(day => day.Country.Equals(dayOneViewModel.Country) && day.Status.Equals(dayOneViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}