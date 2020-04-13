using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IApiService _apiService;
        protected IConfiguration _config;

        protected BaseController(IApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        protected async Task<T> GetRequestData<T>(string urlApi) where T : class
        {
            var requestData = await _apiService.GetAsync<T>
            (
                _config.GetValue<string>($"{ AppSettingsConfig.COVID19API_KEY }:{ urlApi }")
            );

            return requestData;
        }
    }
}