using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.DTO.DayOneCases;
using Example.Covid19.API.DTO.LiveByCountryCases;
using Example.Covid19.API.DTO.StatsCases;
using Example.Covid19.API.DTO.SummaryCases;
using Example.Covid19.API.Models;
using Example.Covid19.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Covid19.API.Controllers
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
        /// <param name="appSettings">El fichero de configuración "appsettings.json"</param>
        public Covid19Controller(IApiService apiService, IOptions<CovidApiAppSettingsModel> appSettings)
        {
            _apiService = apiService;
            _appSettings = appSettings;
        }

        /// <summary>
        ///     Obtiene el resumen global y los casos por países con la última fecha actualizada de los datos
        /// </summary>
        /// <returns>Resumen global y los casos por países</returns>
        /// <response code="200">Resumen global y los casos por países</response>
        [HttpGet("summary")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            return await _apiService.GetAsync<Summary>(_appSettings.Value.Summary);
        }

        /// <summary>
        ///     Obtiene todos los datos relacionados con el país
        /// </summary>
        /// <returns>Lista de todos los países</returns>
        /// <response code="200">Listado de paises</response>
        [HttpGet("countries")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Countries>>> GetCountries()
        {
            var allCountriesList = await _apiService.GetAsync<IEnumerable<Countries>>(_appSettings.Value.Countries);

            return allCountriesList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de los casos del país indicado</returns>
        /// <response code="200">Lista de los casos del país indicado</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("dayone/{countryName}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DayOne>>> GetDayOneByCountry(string countryName)
        {
            string dayOneUrl = _appSettings.Value.DayOne.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var dayOneList = await _apiService.GetAsync<IEnumerable<DayOne>>(dayOneUrl);

            return dayOneList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país desde el primer caso de COVID conocido (en directo)
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de los casos del país indicado</returns>
        /// <response code="200">Lista de los casos del país indicado (en directo)</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("dayone/{countryName}/live")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(string countryName)
        {
            string dayOneLiveUrl = _appSettings.Value.DayOneLive.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);

            return dayOneLiveList.ToList();
        }

        /// <summary>
        ///     Obtiene todos los casos por tipo para un país desde el primer caso de COVID conocido
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        /// <response code="200">Lista de los casos del país indicado (desde el primer caso)</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("total/dayone/{countryName}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(string countryName)
        {
            string dayOneTotalUrl = _appSettings.Value.DayOneTotal.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var dayOneTotalList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);

            return dayOneTotalList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        /// <response code="200">Lista de los casos del país indicado</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("country/{countryName}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(string countryName)
        {
            string byCountryUrl = _appSettings.Value.ByCountry.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);

            return byCountryList.ToList();
        }

        /// <summary>
        ///     Obtiene los casos por tipo para un país (en directo)
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        /// <response code="200">Lista de los casos del país indicado (en directo)</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("country/{countryName}/live")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(string countryName)
        {
            string byCountryLiveUrl = _appSettings.Value.ByCountryLive.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);

            return byCountryLiveList.ToList();
        }

        /// <summary>
        ///     Obtiene todos los casos por tipo para un país
        /// </summary>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <returns>Lista de todos los casos del país indicado</returns>
        /// <response code="200">Lista de taodos los casos del país indicado</response>
        /// <response code="500">Si el nombre del país indicado no existe</response>
        [HttpGet("total/country/{countryName}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(string countryName)
        {
            string byCountryTotalUrl = _appSettings.Value.ByCountryTotal.Replace(COUNTRYNAME_PLACEHOLDER, countryName);
            var byCountryTotalList = await _apiService.GetAsync<IEnumerable<ByCountryTotal>>(byCountryTotalUrl);

            return byCountryTotalList.ToList();
        }

        /// <summary>
        ///     Obtiene los datos en directo de los países y sus estados
        /// </summary>
        /// <remarks>
        ///     Valores disponibles para "status": confirmed, deaths, recovered
        /// </remarks>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <param name="status">Tipo de estado a buscar</param>
        /// <returns>Lista de todos los casos en directo de los países que cumpla el criterio del estado</returns>
        /// <response code="200">Lista de los casos del país indicado (en directo) con un estado específico</response>
        /// <response code="500">Si el nombre del país o el tipo de estado indicado no existe</response>
        [HttpGet("live/country/{countryName}/{status}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(string countryName,
                                                                                                       string status)
        {
            string liveByCountryAndStatusUrl = _appSettings.Value.LiveByCountryAndStatus;
            liveByCountryAndStatusUrl = new StringBuilder(liveByCountryAndStatusUrl)
                    .Replace(COUNTRYNAME_PLACEHOLDER, countryName)
                    .Replace(STATUS_PLACEHOLDER, status)
                    .ToString();

            var countryAndStatusList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(liveByCountryAndStatusUrl);

            return countryAndStatusList.ToList();
        }

        /// <summary>
        ///     Obtiene los datos en directo de los países y sus estados después de una fecha dada
        /// </summary>
        /// <remarks>
        ///     Valores disponibles para "status": confirmed, deaths, recovered
        /// </remarks>
        /// <param name="countryName">Nombre del país a buscar</param>
        /// <param name="status">Tipo de estado a buscar</param>
        /// <param name="date">Casos a buscar después de una fecha especificada</param>
        /// <returns>Lista de todos los casos en directo de los países que cumpla el criterio del estado, dado una fecha</returns>
        /// <response code="200">Lista de los casos del país indicado (en directo) a partir de una fecha dada, con un estado específico</response>
        /// <response code="500">Si el nombre del país, tipo de estado o fecha son erróneas</response>
        [HttpGet("live/country/{countryName}/{status}/{date}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>>
            GetLiveByCountryAndStatusAfterDate(string countryName, string status, DateTime date)
        {
            string liveByCountryAndStatusAfterDateUrl = _appSettings.Value.LiveByCountryAndStatus;
            liveByCountryAndStatusAfterDateUrl = new StringBuilder(liveByCountryAndStatusAfterDateUrl)
                    .Replace(COUNTRYNAME_PLACEHOLDER, countryName)
                    .Replace(STATUS_PLACEHOLDER, status)
                    .Replace(DATE_PLACEHOLDER, date.ToString("yyyy-MM-ddThh:mm:ssZ"))
                    .ToString();

            var countryAndStatusAfterDateList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateUrl);

            return countryAndStatusAfterDateList.ToList();
        }

        /// <summary>
        ///     Obtiene toda la información relacionada con las estadísticas globales de los casos de COVID-19
        /// </summary>
        /// <returns>Estadísticas globales de los casos de COVID-19</returns>
        /// <response code="200">Estadísticas globales de los casos</response>
        [HttpGet("stats")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Stat>> GetStats()
        {
            return await _apiService.GetAsync<Stat>(_appSettings.Value.Stats);
        }
    }
}
