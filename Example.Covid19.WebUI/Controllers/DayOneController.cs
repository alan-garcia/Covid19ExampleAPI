using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.DTO.DayOneCases;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneController : BaseController
    {
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
            var dayOneViewModel = await GetCountriesViewModel<DayOneViewModel>();
            string dayOneUrl = ExtractPlaceholderUrlApi(dayOneViewModel);
            var dayOneList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);
            var dayOneSearchFilter = ApplySearchFilter(dayOneList, dayOneViewModel);
            dayOneViewModel.DayOne = dayOneSearchFilter;

            return View(dayOneViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de todos los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(DayOneViewModel dayOneViewModel)
        {
            if (ModelState.IsValid)
            {
                string dayOneUrl = ExtractPlaceholderUrlApi(dayOneViewModel);
                var dayOneList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);
                var dayOneSearchFilter = ApplySearchFilter(dayOneList, dayOneViewModel);

                dayOneViewModel.DayOne = dayOneSearchFilter;
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
            dayOneViewModel.Country ??= "Spain";
            dayOneViewModel.StatusType ??= "confirmed";

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
            if (dayOneViewModel.Country == null)
            {
                return dayOneByCountryList.OrderByDescending(bc => bc.Date.Date);
            }

            return dayOneByCountryList
                    .Where(day => day.Country.Equals(dayOneViewModel.Country) && day.Status.Equals(dayOneViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }
    }
}
