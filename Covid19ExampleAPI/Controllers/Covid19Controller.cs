﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19ExampleAPI.Models;
using Covid19ExampleAPI.Models.Countries;
using Covid19ExampleAPI.Models.Stats;
using Covid19ExampleAPI.Models.Summary;
using Covid19ExampleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Covid19ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        private readonly IApiService _apiService;
        private readonly IOptions<CovidApisAppSettingsModel> _appSettings;

        public Covid19Controller(IApiService apiService, IOptions<CovidApisAppSettingsModel> appSettings)
        {
            _apiService = apiService;
            _appSettings = appSettings;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            return await _apiService.GetAsync<Summary>(_appSettings.Value.Summary);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            var allCountriesList = await _apiService.GetAsync<IEnumerable<Country>>(_appSettings.Value.Countries);

            return allCountriesList.ToList();
        }

        [HttpGet("dayone/{countryName}")]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneByCountry(string countryName)
        {
            var dayOneUrl = _appSettings.Value.DayOne.Replace("{countryName}", countryName);

            var dayOneList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneUrl);

            return dayOneList.ToList();
        }

        [HttpGet("dayone/{countryName}/live")]
        public async Task<ActionResult<IEnumerable<DayOneLive>>> GetDayOneLiveByCountry(string countryName)
        {
            var dayOneLiveUrl = _appSettings.Value.DayOneLive.Replace("{countryName}", countryName);

            var dayOneLiveList = await _apiService.GetAsync<IEnumerable<DayOneLive>>(dayOneLiveUrl);

            return dayOneLiveList.ToList();
        }

        [HttpGet("total/dayone/{countryName}")]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(string countryName)
        {
            var dayOneTotalUrl = _appSettings.Value.DayOneTotal.Replace("{countryName}", countryName);

            var dayOneTotalList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);

            return dayOneTotalList.ToList();
        }

        [HttpGet("country/{countryName}")]
        public async Task<ActionResult<IEnumerable<ByCountry>>> GetByCountry(string countryName)
        {
            var byCountryUrl = _appSettings.Value.ByCountry.Replace("{countryName}", countryName);

            var byCountryList = await _apiService.GetAsync<IEnumerable<ByCountry>>(byCountryUrl);

            return byCountryList.ToList();
        }

        [HttpGet("country/{countryName}/live")]
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(string countryName)
        {
            var byCountryLiveUrl = _appSettings.Value.ByCountryLive.Replace("{countryName}", countryName);

            var byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);

            return byCountryLiveList.ToList();
        }

        [HttpGet("total/country/{countryName}")]
        public async Task<ActionResult<IEnumerable<ByCountryTotal>>> GetByCountryTotal(string countryName)
        {
            var byCountryTotalUrl = _appSettings.Value.ByCountryTotal.Replace("{countryName}", countryName);

            var byCountryTotalList = await _apiService.GetAsync<IEnumerable<ByCountryTotal>>(byCountryTotalUrl);

            return byCountryTotalList.ToList();
        }

        [HttpGet("live/country/{countryName}/{status}")]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>>
            GetLiveByCountryAndStatus(string countryName, string status)
        {
            var countryAndStatusUrl = _appSettings.Value.LiveByCountryAndStatus
                                        .Replace("{countryName}", countryName)
                                        .Replace("{status}", status);

            var countryAndStatusList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(countryAndStatusUrl);

            return countryAndStatusList.ToList();
        }

        [HttpGet("live/country/{countryName}/{status}/{date}")]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>>
            GetLiveByCountryAndStatusAfterDate(string countryName, string status, DateTime date)
        {
            var countryAndStatusWithDateUrl = _appSettings.Value.LiveByCountryAndStatusAfterDate
                                        .Replace("{countryName}", countryName)
                                        .Replace("{status}", status)
                                        .Replace("{date}", date.ToString("yyyy-MM-ddThh:mm:ssZ"));

            var countryAndStatusWithDateList = await _apiService
                .GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(countryAndStatusWithDateUrl);

            return countryAndStatusWithDateList.ToList();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<Stat>> GetStats()
        {
            var stats = await _apiService.GetAsync<Stat>(_appSettings.Value.Stats);

            return stats;
        }
    }
}
