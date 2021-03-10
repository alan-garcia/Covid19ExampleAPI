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
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

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
            var dayOneLiveViewModel = new DayOneLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos desde el primer caso de COVID (en directo), filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(int? page)
        {
            string dayOneLiveByCountryListFilter = HttpContext.Session.GetString("DayOneLiveByCountryListFilter");
            IEnumerable<DayOneLive> dayOneLiveByCountryListFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<DayOneLive>>(dayOneLiveByCountryListFilter);

            int pageNumber = page ?? 1;
            HttpContext.Session.SetString("DayOneLiveByCountryListFilter", JsonConvert.SerializeObject(dayOneLiveByCountryListFilterDeserialized));

            ViewBag.DayOneLiveByCountryListFilter = dayOneLiveByCountryListFilterDeserialized.ToPagedList(pageNumber, 15);

            DayOneLiveViewModel dayOneLiveViewModel = new DayOneLiveViewModel
            {
                DayOneLive = dayOneLiveByCountryListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

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
                IEnumerable<DayOneLive> dayOneLiveByCountryList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);
                IEnumerable<DayOneLive> dayOneLiveByCountryListFilter = ApplySearchFilter(dayOneLiveByCountryList, dayOneLiveViewModel);

                dayOneLiveViewModel.DayOneLive = dayOneLiveByCountryListFilter;

                int pageNumber = page ?? 1;
                HttpContext.Session.SetString("DayOneLiveByCountryListFilter", JsonConvert.SerializeObject(dayOneLiveByCountryListFilter));

                ViewBag.DayOneLiveByCountryListFilter = dayOneLiveByCountryListFilter.ToPagedList(pageNumber, 15);
            }

            dayOneLiveViewModel.Countries = await GetCountries();
            dayOneLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneLiveViewModel);
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
        /// <param name="dayOneLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "dayone/country/status/live" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(DayOneLiveViewModel dayOneLiveViewModel)
        {
            string dayOneLiveApiUrlPlaceHolder = _config.GetValue<string>(
                $"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_LIVE_KEY}"
            );

            return new StringBuilder(dayOneLiveApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, dayOneLiveViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, dayOneLiveViewModel.StatusType)
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