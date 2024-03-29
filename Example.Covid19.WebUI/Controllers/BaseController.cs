﻿using Example.Covid19.API.DTO.CountriesCases;
using Example.Covid19.API.Services;
using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    /// <summary>
    ///     Controlador que contienen las acciones comunes para todos los controladores
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected IApiService _apiService;
        protected IConfiguration _config;
        protected ICovid19MemoryCacheService _cache;

        /// <summary>
        ///     Constructor que inyecta el servicio de la API y la configuración cargada en el fichero "appsettings.json"
        /// </summary>
        /// <param name="apiService">El servicio de la API de la cual va a consumir</param>
        /// <param name="config">El fichero de configuración "appsettings.json"</param>
        /// <param name="cache">La caché en memoria</param>
        protected BaseController(IApiService apiService, IConfiguration config, ICovid19MemoryCacheService cache)
        {
            _apiService = apiService;
            _config = config;
            _cache = cache;
        }

        /// <summary>
        ///     Obtiene el modelo de los paises y sus estados
        /// </summary>
        /// <typeparam name="T">La Vista-Modelo</typeparam>
        /// <returns>La Vista-Modelo con todos los países y sus posibles estados</returns>
        protected async Task<T> GetCountriesViewModel<T>() where T : CovidBaseViewModel, new()
        {
            T viewModel = new()
            {
                Countries = await GetCountriesSelect(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return viewModel;
        }

        /// <summary>
        ///     Obtiene todos los datos relacionados con el país
        /// </summary>
        /// <returns>La lista de países en un elemento HTML de tipo "desplegable" para ser mostrado en la vista</returns>
        protected async Task<IEnumerable<SelectListItem>> GetCountriesSelect()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);
            return CountriesList.GetCountriesSelectListItem(countries);
        }

        /// <summary>
        ///     Obtiene los datos de la API con base en una URL concreta
        /// </summary>
        /// <typeparam name="T">API concreta a consumir</typeparam>
        /// <param name="apiUrl">URL concreta de la API</param>
        /// <returns></returns>
        protected async Task<T> GetRequestData<T>(string apiUrl) where T : class
        {
            var requestData = await _apiService.GetAsync<T>
            (
                _config.GetValue<string>($"{ AppSettingsConfig.COVID19API_KEY }:{ apiUrl }")
            );

            return requestData;
        }

        /// <summary>
        ///     Obtiene la URL de la API del fichero "appsettings.json" especificado por el nombre de la clave
        /// </summary>
        /// <param name="key">Nombre de la clave a buscar dentro del fichero "appsettings.json"</param>
        /// <returns>La URL de la API de la clave especificada</returns>
        public string GetAppSettingsUrlApiByKey(string key)
        {
            return _config.GetValue<string>($"{ AppSettingsConfig.COVID19API_KEY }:{ key }");
        }
    }
}
