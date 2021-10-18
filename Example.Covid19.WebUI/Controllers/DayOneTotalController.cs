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
    ///     Controlador que contienen las acciones sobre todos los casos por tipo para un país desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneTotalController : BaseController
    {
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
            var dayOneTotalViewModel = await GetCountriesViewModel<DayOneTotalViewModel>();
            string dayOneTotalUrl = ExtractPlaceholderUrlApi(dayOneTotalViewModel);
            var dayOneTotalList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);
            var dayOneTotalSearchFilter = ApplySearchFilter(dayOneTotalList, dayOneTotalViewModel);
            dayOneTotalViewModel.DayOneTotal = dayOneTotalSearchFilter;

            return View(dayOneTotalViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de todos los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido
        /// </summary>
        /// <param name="dayOneTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de todos los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(DayOneTotalViewModel dayOneTotalViewModel)
        {
            if (ModelState.IsValid)
            {
                string dayOneTotalUrl = ExtractPlaceholderUrlApi(dayOneTotalViewModel);
                var dayOneTotalList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);
                var dayOneTotalSearchFilter = ApplySearchFilter(dayOneTotalList, dayOneTotalViewModel);

                dayOneTotalViewModel.DayOneTotal = dayOneTotalSearchFilter;
            }

            dayOneTotalViewModel.Countries = await GetCountries();
            dayOneTotalViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneTotalViewModel);
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
            string dayOneTotalApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.DAYONE_TOTAL_KEY);
            dayOneTotalViewModel.Country ??= "Spain";
            dayOneTotalViewModel.StatusType ??= "confirmed";

            return new StringBuilder(dayOneTotalApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, dayOneTotalViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, dayOneTotalViewModel.StatusType)
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
            if (dayOneTotalViewModel.Country == null)
            {
                return dayOneTotalByCountryList.OrderByDescending(bc => bc.Date.Date);
            }

            return dayOneTotalByCountryList
                    .Where(day => day.Country.Equals(dayOneTotalViewModel.Country) && day.Status.Equals(dayOneTotalViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}
