﻿using Example.Covid19.WebUI.Config;
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
    ///     Controlador que contienen las acciones sobre todos los casos por tipo para un país
    /// </summary>
    public class ByCountryTotalController : BaseController
    {
        private const int PAGE_SIZE = 15;

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
            return View(await GetCountriesViewModel<ByCountryTotalViewModel>());
        }

        /// <summary>
        ///     Obtiene la lista de todos los casos por tipo para un país
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los casos filtrado por países</returns>
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(int? page)
        {
            string byCountryTotalSearchFilter = HttpContext.Session.GetString("ByCountryTotalSearchFilter");
            IEnumerable<ByCountryTotal> byCountryTotalSearchFilterDeserialized = 
                JsonConvert.DeserializeObject<IEnumerable<ByCountryTotal>>(byCountryTotalSearchFilter);

            int pageNumber = page ?? 1;
            var byCountryTotalViewModel = await GetCountriesViewModel<ByCountryTotalViewModel>();
            byCountryTotalViewModel.ByCountryTotal = byCountryTotalSearchFilterDeserialized.ToPagedList(pageNumber, PAGE_SIZE);

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
                IEnumerable<ByCountryTotal> byCountryTotalSearchFilter = ApplySearchFilter(byCountryTotalList, byCountryTotalViewModel);

                int pageNumber = page ?? 1;
                byCountryTotalViewModel.ByCountryTotal = byCountryTotalSearchFilter.ToPagedList(pageNumber, PAGE_SIZE);
                HttpContext.Session.SetString("ByCountryTotalSearchFilter", JsonConvert.SerializeObject(byCountryTotalSearchFilter));
            }

            byCountryTotalViewModel.Countries = await GetCountries();
            byCountryTotalViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryTotalViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre corchetes "{" "}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="byCountryTotalViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La URL de la API "total/country/status" con los parámetros de búsqueda sustituídos</returns>
        public string ExtractPlaceholderUrlApi(ByCountryTotalViewModel byCountryTotalViewModel)
        {
            string byCountryTotalApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.BY_COUNTRY_TOTAL_KEY);

            return new StringBuilder(byCountryTotalApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, byCountryTotalViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, byCountryTotalViewModel.StatusType)
                    .Replace(AppSettingsConfig.DATEFROM_PLACEHOLDER, byCountryTotalViewModel.DateFrom.ToString("dd/MM/yyyy"))
                    .Replace(AppSettingsConfig.DATETO_PLACEHOLDER, byCountryTotalViewModel.DateTo.ToString("dd/MM/yyyy"))
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
