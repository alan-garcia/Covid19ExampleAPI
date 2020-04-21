using Example.Covid19.WebUI.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Services
{
    public class ApiService : IApiService
    {
        private readonly string _urlApiBase;
        private readonly int _loginTimeOut;

        public ApiService(IConfiguration config)
        {
            _loginTimeOut = 20;
            _urlApiBase = config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.API_URLBASE_KEY}");
        }

        public async Task<T> GetAsync<T>(string urlApi) where T : class
        {
            T covidContentInfo = null;

            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(_loginTimeOut) })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync(_urlApiBase + urlApi);
                if (response.IsSuccessStatusCode)
                {
                    var httpContent = await response.Content.ReadAsStringAsync();
                    covidContentInfo = JsonConvert.DeserializeObject<T>(httpContent, JsonConfig.GetJsonSerializerSettings());
                }

                return covidContentInfo;
            }
        }

    }
}
