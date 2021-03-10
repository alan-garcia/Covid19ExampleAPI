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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país
    /// </summary>
    public class ByCountryController : BaseController
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
        public ByCountryController(IApiService apiService, IConfiguration config) : base(apiService, config)
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
            var byCountryViewModel = new ByCountryViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        /// <summary>
        ///     Obtiene la lista de los casos por tipo para un país
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(int? page)
        {
            string byCountryListFilter = HttpContext.Session.GetString("ByCountryListFilter");
            IEnumerable<ByCountry> byCountryListFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<ByCountry>>(byCountryListFilter);

            int pageNumber = page ?? 1;
            HttpContext.Session.SetString("ByCountryListFilter", JsonConvert.SerializeObject(byCountryListFilterDeserialized));

            ViewBag.ByCountryFilterList = byCountryListFilterDeserialized.ToPagedList(pageNumber, 15);

            ByCountryViewModel byCountryViewModel = new ByCountryViewModel
            {
                ByCountry = byCountryListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", byCountryViewModel);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país
        /// </summary>
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de los casos del país indicado que cumpla el criterio del estado 
        /// y el rango de fechas</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(ByCountryViewModel byCountryViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                string byCountryUrl = ExtractPlaceholderUrlApi(byCountryViewModel);
                IEnumerable<ByCountry> byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);
                IEnumerable<ByCountry> byCountryListFilter = ApplySearchFilter(byCountryList, byCountryViewModel);
                
                byCountryViewModel.ByCountry = byCountryListFilter;

                int pageNumber = page ?? 1;
                HttpContext.Session.SetString("ByCountryListFilter", JsonConvert.SerializeObject(byCountryListFilter));

                ViewBag.ByCountryFilterList = byCountryListFilter.ToPagedList(pageNumber, 15);
            }

            byCountryViewModel.Countries = await GetCountries();
            byCountryViewModel.StatusTypeList = StatusType.GetStatusTypeList();
            
            return View("Index", byCountryViewModel);
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
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "country/status" con los parámetros de búsqueda sustituídos</returns>
        private string ExtractPlaceholderUrlApi(ByCountryViewModel byCountryViewModel)
        {
            string byCountryApiUrlPlaceHolder = _config.GetValue<string>(
                $"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_KEY}"
            );

            return new StringBuilder(byCountryApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, byCountryViewModel.Country)
                    .Replace(STATUS_PLACEHOLDER, byCountryViewModel.StatusType)
                    .Replace(DATEFROM_PLACEHOLDER, byCountryViewModel.DateFrom.ToString("yyyy-MM-dd"))
                    .Replace(DATETO_PLACEHOLDER, byCountryViewModel.DateTo.ToString("yyyy-MM-dd"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país
        /// </summary>
        /// <param name="byCountryList">La lista de países</param>
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país y el rango de fechas seleccionadas en la búsqueda, ordenadas de fechas más recientes a más antiguas</returns>
        private IEnumerable<ByCountry> ApplySearchFilter(IEnumerable<ByCountry> byCountryList,
                                                         ByCountryViewModel byCountryViewModel)
        {
            return byCountryList
                    .Where(bc => bc.Country.Equals(byCountryViewModel.Country) && bc.Status.Equals(byCountryViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryViewModel.DateFrom && bc.Date <= byCountryViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }

    }
}
