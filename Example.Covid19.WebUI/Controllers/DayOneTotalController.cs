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
    ///     Controlador que contienen las acciones sobre todos los casos por tipo para un país desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneTotalController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public DayOneTotalController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            var dayOneTotalViewModel = new DayOneTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneTotalViewModel);
        }

        /// <summary>
        ///     Obtiene la lista de todos los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos desde el primer caso de COVID, filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(int? page)
        {
            string dayOneTotalByCountryListFilter = HttpContext.Session.GetString("DayOneTotalByCountryListFilter");
            IEnumerable<DayOneTotal> dayOneTotalByCountryListFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<DayOneTotal>>(dayOneTotalByCountryListFilter);

            int pageNumber = page ?? 1;
            HttpContext.Session.SetString("DayOneTotalByCountryListFilter", JsonConvert.SerializeObject(dayOneTotalByCountryListFilterDeserialized));

            ViewBag.DayOneTotalByCountryListFilter = dayOneTotalByCountryListFilterDeserialized.ToPagedList(pageNumber, 15);

            DayOneTotalViewModel dayOneTotalViewModel = new DayOneTotalViewModel
            {
                DayOneTotal = dayOneTotalByCountryListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", dayOneTotalViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de todos los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneTotalLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(DayOneTotalViewModel dayOneTotalLiveViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string dayOneTotalUrl = ExtractPlaceholderUrlApi(dayOneTotalLiveViewModel);
                IEnumerable<DayOneTotal> dayOneTotalByCountryList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);
                IEnumerable<DayOneTotal> dayOneTotalByCountryListFilter = ApplySearchFilter(dayOneTotalByCountryList, dayOneTotalLiveViewModel);

                dayOneTotalLiveViewModel.DayOneTotal = dayOneTotalByCountryListFilter;

                int pageNumber = page ?? 1;
                HttpContext.Session.SetString("DayOneTotalByCountryListFilter", JsonConvert.SerializeObject(dayOneTotalByCountryListFilter));

                ViewBag.DayOneTotalByCountryListFilter = dayOneTotalByCountryListFilter.ToPagedList(pageNumber, 15);
            }

            dayOneTotalLiveViewModel.Countries = await GetCountries();
            dayOneTotalLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneTotalLiveViewModel);
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
        /// <param name="dayOneTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "total/dayone/country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(DayOneTotalViewModel dayOneTotalViewModel)
        {
            string dayOneTotalApiUrlPlaceHolder = _config.GetValue<string>(
                $"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_TOTAL_KEY}"
            );

            return new StringBuilder(dayOneTotalApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, dayOneTotalViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, dayOneTotalViewModel.StatusType)
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país desde el 
        ///     primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="dayOneTotalByCountryList">La lista de países</param>
        /// <param name="dayOneTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país desde el primer caso de COVID, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<DayOneTotal> ApplySearchFilter(IEnumerable<DayOneTotal> dayOneTotalByCountryList,
                                                           DayOneTotalViewModel dayOneTotalViewModel)
        {
            return dayOneTotalByCountryList
                    .Where(day => day.Country.Equals(dayOneTotalViewModel.Country) && day.Status.Equals(dayOneTotalViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}
