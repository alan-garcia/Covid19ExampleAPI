﻿using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.DTO.DayOneCases;
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
    ///     Controlador que contienen las acciones sobre los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
    /// </summary>
    public class DayOneLiveController : BaseController
    {
        private string dayOneLiveCacheKey = "dayOneLive";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        /// <param name="cache">La caché en memoria</param>
        public DayOneLiveController(IApiService apiService, IConfiguration config, ICovid19MemoryCacheService cache) : base(apiService, config, cache)
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
            if (!_cache.Get(dayOneLiveCacheKey, out DayOneLiveViewModel dayOneLiveVM))
            {
                dayOneLiveVM = await GetCountriesViewModel<DayOneLiveViewModel>();
                string dayOneLiveUrl = ExtractPlaceholderUrlApi(dayOneLiveVM);
                var dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);
                dayOneLiveVM.DayOneLive = ApplySearchFilter(dayOneLiveList, dayOneLiveVM);
            }

            return View(dayOneLiveVM);
        }

        /// <summary>
        ///     Aplica en el formulario los filtros de búsqueda de los casos por tipo para un país desde el 
        ///     primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="dayOneLiveViewModel">La vista-modelo que contienen las opciones seleccionadas en el 
        /// formulario de búsqueda</param>
        /// <returns>La vista con la lista de los casos del país indicado que cumpla el criterio del estado</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(DayOneLiveViewModel dayOneLiveViewModel)
        {
            if (ModelState.IsValid)
            {
                dayOneLiveCacheKey = $"{dayOneLiveCacheKey}_{dayOneLiveViewModel.Country}_{dayOneLiveViewModel.StatusType}";
                if (!_cache.Get(dayOneLiveCacheKey, out DayOneLiveViewModel dayOneLiveVM))
                {
                    dayOneLiveVM = await GetCountriesViewModel<DayOneLiveViewModel>();
                    string dayOneLiveUrl = ExtractPlaceholderUrlApi(dayOneLiveVM);
                    var dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);
                    dayOneLiveVM.DayOneLive = ApplySearchFilter(dayOneLiveList, dayOneLiveVM);

                    _cache.Set(dayOneLiveCacheKey, dayOneLiveVM);
                }

                dayOneLiveViewModel = dayOneLiveVM;
            }

            return View("Index", dayOneLiveViewModel);
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
            string dayOneLiveApiUrl = GetAppSettingsUrlApiByKey(AppSettingsConfig.DAYONE_LIVE_KEY);
            dayOneLiveViewModel.Country ??= "Spain";
            dayOneLiveViewModel.StatusType ??= "confirmed";

            return new StringBuilder(dayOneLiveApiUrl)
                    .Replace(AppSettingsConfig.COUNTRYNAME_PLACEHOLDER, dayOneLiveViewModel.Country)
                    .Replace(AppSettingsConfig.STATUS_PLACEHOLDER, dayOneLiveViewModel.StatusType)
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
            if (dayOneLiveViewModel.Country == null)
            {
                return dayOneLiveByCountryList.OrderByDescending(bc => bc.Date.Date);
            }

            return dayOneLiveByCountryList
                    .Where(day => day.Country.Equals(dayOneLiveViewModel.Country) && day.Status.Equals(dayOneLiveViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}
