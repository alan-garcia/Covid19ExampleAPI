using Example.Covid19.API.Helpers;
using Example.Covid19.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Example.Covid19.API.Services
{
    /// <summary>
    ///     Implementación del servicio para describir los servicios de la API a consumir
    /// </summary>
    public class ApiService : IApiService
    {
        /// <summary>
        ///     URL base de la API, indicada en la propiedad "UrlBase" del fichero "appsettings.json".
        /// </summary>
        private readonly string _baseApiUrl;

        /// <summary>
        ///     Tiempo límite asignado para conectarse a la API (en segundos)
        /// </summary>
        private readonly int _loginTimeOut;

        //// <summary>
        ///     Constructor que inyecta la configuración cargada en "appsetings.json" (la cual contiene todas las URLs de la API)
        ///     a partir de la propiedad "Covid19Api" de dicho fichero.
        /// </summary>
        /// <param name="appSettings">Fichero de configuración "appsettings.json"</param>
        public ApiService(IOptions<CovidApiAppSettingsModel> appSettings)
        {
            _loginTimeOut = 20;
            _baseApiUrl = appSettings.Value.UrlBase;
        }

        /// <summary>
        ///     Obtiene los datos de la API con base en una URL concreta
        /// </summary>
        /// <typeparam name="T">API concreta a consumir</typeparam>
        /// <param name="apiUrl">URL concreta de la API</param>
        /// <returns>Toda la información relacionada con la API especificada en la URL</returns>
        public async Task<T> GetAsync<T>(string apiUrl) where T : class
        {
            T covidContentInfo = null;

            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(_loginTimeOut) })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(_baseApiUrl + apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string httpContent = await response.Content.ReadAsStringAsync();
                    covidContentInfo = JsonConvert.DeserializeObject<T>(httpContent, JsonConfig.GetJsonSerializerSettings());
                }

                return covidContentInfo;
            }
        }
    }
}
