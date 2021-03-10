using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Covid19ExampleAPI.DTO.Cases.CountriesCases;
using Covid19ExampleAPI.DTO.Cases.DayOneCases;
using Covid19ExampleAPI.DTO.Cases.StatsCases;
using Covid19ExampleAPI.DTO.Cases.SummaryCases;
using Covid19ExampleAPI.Models;
using Covid19ExampleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covid19ExampleAPI.Controllers
{
    /// <summary>
    ///     Controlador que contienen todos los accesos a la API de COVID-19
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        private readonly IApiService _apiService;
        private readonly IOptions<CovidApiAppSettingsModel> _appSettings;

        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATE_PLACEHOLDER = "{date}";

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        public Covid19Controller(IApiService apiService, IOptions<CovidApiAppSettingsModel> appSettings)
        {
            _apiService = apiService;
            _appSettings = appSettings;
        }

        /// <summary>
        ///     Obtiene el resumen global y los casos por países con la última fecha actualizada de los datos
        /// </summary>
        /// <returns></returns>
        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            return await _apiService.GetAsync<Summary>(_appSettings.Value.Summary);
        }

        /// <summary>
        ///     Obtiene todos los datos relacionados con el país
        /// </summary>
        /// <param name="page">Número de página actual de la paginación</param>
        /// <returns>La vista con la lista de todos los países</returns>
        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Countries>>> GetCountries()
        {
            IEnumerable<Countries> allCountriesList = await _apiService.GetAsync<IEnumerable<Countries>>(_appSettings.Value.Countries);

            return allCountriesList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de los casos del país indicado</returns>
        [HttpGet("dayone/{countryName}")]
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(string countryName)
        {
            string dayOneUrl = _appSettings.Value.DayOne.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<DayOne> dayOneList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);

            return dayOneList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de los casos del país indicado</returns>
        [HttpGet("dayone/{countryName}/live")]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(string countryName)
        {
            string dayOneLiveUrl = _appSettings.Value.DayOneLive.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<DayOneLive> dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);

            return dayOneLiveList.ToList();
        }

        /// <summary>
        ///     Obtiene todos los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        [HttpGet("total/dayone/{countryName}")]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(string countryName)
        {
            string dayOneTotalUrl = _appSettings.Value.DayOneTotal.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<DayOneTotal> dayOneTotalList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);

            return dayOneTotalList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        [HttpGet("country/{countryName}")]
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(string countryName)
        {
            string byCountryUrl = _appSettings.Value.ByCountry.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<ByCountry> byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);

            return byCountryList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país (en directo)
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        [HttpGet("country/{countryName}/live")]
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(string countryName)
        {
            string byCountryLiveUrl = _appSettings.Value.ByCountryLive.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<ByCountryLive> byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);

            return byCountryLiveList.ToList();
        }

        /// <summary>
        ///     Obtiene todos los casos por tipo para un país
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        [HttpGet("total/country/{countryName}")]
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(string countryName)
        {
            string byCountryTotalUrl = _appSettings.Value.ByCountryTotal.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            IEnumerable<ByCountryTotal> byCountryTotalList = await _apiService.GetAsync<IEnumerable<ByCountryTotal>>(byCountryTotalUrl);

            return byCountryTotalList.ToList();
        }

        /// <summary>
        ///     Obtiene los datos en directo de los países y sus estados
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <param name="status">Tipo de estado a buscar</param>
        /// <returns>Lista de todos los casos en directo de los países que cumpla el criterio del estad</returns>
        [HttpGet("live/country/{countryName}/{status}")]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(string countryName,
                                                                                                       string status)
        {
            string countryAndStatusApiUrlPlaceHolder = _appSettings.Value.LiveByCountryAndStatus;
            countryAndStatusApiUrlPlaceHolder = new StringBuilder(countryAndStatusApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, countryName)
                    .Replace(STATUS_PLACEHOLDER, status)
                    .ToString();

            IEnumerable<LiveByCountryAndStatus> countryAndStatusList = 
                await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(countryAndStatusApiUrlPlaceHolder);

            return countryAndStatusList.ToList();
        }

        /// <summary>
        ///     Obtiene los datos en directo de los países y sus estados después de una fecha dada
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <param name="status">Tipo de estado a buscar</param>
        /// <param name="date">Casos a buscar después de una fecha especificada</param>
        /// <returns>Lista de todos los casos en directo de los países que cumpla el criterio del estado, dado una fecha</returns>
        [HttpGet("live/country/{countryName}/{status}/{date}")]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>>
            GetLiveByCountryAndStatusAfterDate(string countryName, string status, DateTime date)
        {
            string countryAndStatusAfterDateApiUrlPlaceHolder = _appSettings.Value.LiveByCountryAndStatus;
            countryAndStatusAfterDateApiUrlPlaceHolder = new StringBuilder(countryAndStatusAfterDateApiUrlPlaceHolder)
                    .Replace(COUNTRYNAME_PLACEHOLDER, countryName)
                    .Replace(STATUS_PLACEHOLDER, status)
                    .Replace(DATE_PLACEHOLDER, date.ToString("yyyy-MM-ddThh:mm:ssZ"))
                    .ToString();

            IEnumerable<LiveByCountryAndStatusAfterDate> countryAndStatusAfterDateList = await _apiService
                .GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(countryAndStatusAfterDateApiUrlPlaceHolder);

            return countryAndStatusAfterDateList.ToList();
        }

        /// <summary>
        ///     Obtiene toda la información relacionada con las estadísticas globales de los casos de COVID-19
        /// </summary>
        /// <returns>La vista con las estadísticas globales de los casos de COVID-19</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<Stat>> GetStats()
        {
            return await _apiService.GetAsync<Stat>(_appSettings.Value.Stats);
        }
    }
}
