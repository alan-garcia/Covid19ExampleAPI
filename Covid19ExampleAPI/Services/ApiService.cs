using Covid19ExampleAPI.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Covid19ExampleAPI
{
    public class ApiService
    {
        private readonly int _loginTimeOut;

        public ApiService() {
            _loginTimeOut = 20;
        }

        public async Task<T> GetAsync<T>(string urlApi) where T : class
        {
            T covidContentInfo = null;

            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(_loginTimeOut) })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync(urlApi);
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
