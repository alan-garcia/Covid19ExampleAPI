﻿using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país
    /// </summary>
    public class ByCountryController : BaseController
    {
        private string getCountriesCacheKey = "getCountries";
        private string byCountryCacheKey = "byCountry";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        /// <param name="cache">La caché en memoria</param>
        public ByCountryController(IApiService apiService, IConfiguration config, ICovid19MemoryCacheService cache) : base(apiService, config, cache)
        {
            _apiService = apiService;
            _config = config;
            _cache = cache;
        }

        /// <summary>
        ///     Obtiene la lista de todos los países
        /// </summary>
        /// <returns>La vista con la lista de los países</returns>
        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            if (!_cache.Get(getCountriesCacheKey, out ByCountryViewModel byCountryVM))
            {
                byCountryVM = await GetCountriesViewModel<ByCountryViewModel>();
                string byCountryUrl = ExtractPlaceholderUrlApi(byCountryVM);
                var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);
                byCountryVM.ByCountry = ApplySearchFilter(byCountryList, byCountryVM);

                _cache.Set(getCountriesCacheKey, byCountryVM);
            }

            return View(byCountryVM);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país
        /// </summary>
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de los casos del país indicado que cumpla el criterio del estado 
        /// y el rango de fechas</returns>
        [HttpPost]
        public async Task<ActionResult> GetByCountry(ByCountryViewModel byCountryViewModel)
        {
            if (ModelState.IsValid)
            {
                byCountryCacheKey = $"{byCountryCacheKey}_{byCountryViewModel.Country}_{byCountryViewModel.StatusType}_{byCountryViewModel.DateFrom.ToShortDateString()}_{byCountryViewModel.DateTo.ToShortDateString()}";
                if (!_cache.Get(byCountryCacheKey, out ByCountryViewModel byCountryVM))
                {
                    byCountryVM = await GetCountriesViewModel<ByCountryViewModel>();
                    string byCountryUrl = ExtractPlaceholderUrlApi(byCountryVM);
                    var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);
                    byCountryVM.ByCountry = ApplySearchFilter(byCountryList, byCountryVM);

                    _cache.Set(byCountryCacheKey, byCountryVM);
                }

                byCountryViewModel = byCountryVM;
            }

            return View("Index", byCountryViewModel);
        }

        /// <summary>
        ///     Sustituye los placeholders marcados entre llaves "{}" especificados en el fichero "appsettings.json" 
        ///     en el apartado "Covid19Api" por los datos filtrados en la vista-modelo recogidas en el formulario de búsqueda
        /// </summary>
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>URL de la API "country/status" con los parámetros de búsqueda sustituídos</returns>
        public string ExtractPlaceholderUrlApi(ByCountryViewModel byCountryViewModel)
        {
            string byCountryApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.BY_COUNTRY_KEY);
            byCountryViewModel.Country ??= "Spain";
            byCountryViewModel.StatusType ??= "confirmed";

            return new StringBuilder(byCountryApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, byCountryViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, byCountryViewModel.StatusType)
                    .Replace(AppSettingsConfig.DATEFROM_PLACEHOLDER, byCountryViewModel.DateFrom.ToString("yyyy-MM-dd"))
                    .Replace(AppSettingsConfig.DATETO_PLACEHOLDER, byCountryViewModel.DateTo.ToString("yyyy-MM-dd"))
                    .ToString();
        }

        /// <summary>
        ///     Aplica el filtro de búsqueda para los casos de un país
        /// </summary>
        /// <param name="byCountryList">La lista de países</param>
        /// <param name="byCountryViewModel">La vista-modelo que contienen las opciones seleccionadas en el formulario de búsqueda</param>
        /// <returns>Lista con el país y el rango de fechas seleccionadas en la búsqueda, ordenadas de fechas más recientes a más antiguas</returns>
        public IEnumerable<ByCountry> ApplySearchFilter(IEnumerable<ByCountry> byCountryList,
                                                        ByCountryViewModel byCountryViewModel)
        {
            if (byCountryViewModel.Country == null)
            {
                return byCountryList.OrderByDescending(bc => bc.Date.Date);
            }

            return byCountryList
                    .Where(bc => bc.Country.Equals(byCountryViewModel.Country) && bc.Status.Equals(byCountryViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryViewModel.DateFrom && bc.Date <= byCountryViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }
    }
}
