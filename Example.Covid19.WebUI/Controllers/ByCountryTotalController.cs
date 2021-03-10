using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
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
    ///     Controlador que contienen las acciones sobre todos los casos por tipo para un país
    /// </summary>
    public class ByCountryTotalController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public ByCountryTotalController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            var byCountryViewModel = new ByCountryTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        /// <summary>
        ///     Obtiene la lista de todos los casos por tipo para un país
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(int? page)
        {
            string byCountryTotalListFilter = HttpContext.Session.GetString("ByCountryTotalListFilter");
            IEnumerable<ByCountryTotal> byCountryTotalListFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<ByCountryTotal>>(byCountryTotalListFilter);

            int pageNumber = page ?? 1;
            HttpContext.Session.SetString("ByCountryTotalListFilter", JsonConvert.SerializeObject(byCountryTotalListFilterDeserialized));

            ViewBag.ByCountryTotalFilterList = byCountryTotalListFilterDeserialized.ToPagedList(pageNumber, 15);

            ByCountryTotalViewModel byCountryTotalViewModel = new ByCountryTotalViewModel
            {
                ByCountryTotal = byCountryTotalListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", byCountryTotalViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de todos los casos por tipo para un país
        /// </summary>
        /// <param name="byCountryTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos del país indicado que cumpla el criterio del estado 
        /// y el rango de fechas</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(ByCountryTotalViewModel byCountryTotalViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string byCountryTotalUrl = ExtractPlaceholderUrlApi(byCountryTotalViewModel);
                IEnumerable<ByCountryTotal> byCountryTotalList = await _apiService.GetAsync<IEnumerable<ByCountryTotal>>(byCountryTotalUrl);
                IEnumerable<ByCountryTotal> byCountryTotalListFilter = ApplySearchFilter(byCountryTotalList, byCountryTotalViewModel);

                byCountryTotalViewModel.ByCountryTotal = byCountryTotalListFilter;

                int pageNumber = page ?? 1;
                HttpContext.Session.SetString("ByCountryTotalListFilter", JsonConvert.SerializeObject(byCountryTotalListFilter));

                ViewBag.byCountryTotalFilterList = byCountryTotalListFilter.ToPagedList(pageNumber, 15);
            }

            byCountryTotalViewModel.Countries = await GetCountries();
            byCountryTotalViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryTotalViewModel);
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
        /// <param name="byCountryTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "total/country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(ByCountryTotalViewModel byCountryTotalViewModel)
        {
            string byCountryTotalApiUrlPlaceHolder = _config.GetValue<string>(
                $"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_TOTAL_KEY}"
            );

            return new StringBuilder(byCountryTotalApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, byCountryTotalViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, byCountryTotalViewModel.StatusType)
                    .Replace(DATEFROM_PLACEHOLDER, byCountryTotalViewModel.DateFrom.ToString("dd/MM/yyyy"))
                    .Replace(DATETO_PLACEHOLDER, byCountryTotalViewModel.DateTo.ToString("dd/MM/yyyy"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país
        /// </summary>
        /// <param name="byCountryTotalList">La lista de países</param>
        /// <param name="byCountryTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país y el rango de fechas seleccionadas en la búsqueda, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<ByCountryTotal> ApplySearchFilter(IEnumerable<ByCountryTotal> byCountryTotalList,
                                                              ByCountryTotalViewModel byCountryTotalViewModel)
        {
            return byCountryTotalList
                    .Where(bc => bc.Country.Equals(byCountryTotalViewModel.Country) && bc.Status.Equals(byCountryTotalViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryTotalViewModel.DateFrom && bc.Date <= byCountryTotalViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }

    }
}
