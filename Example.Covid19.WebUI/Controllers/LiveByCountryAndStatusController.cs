using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Controllers
{
    public class LiveByCountryAndStatusController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public LiveByCountryAndStatusController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new LiveByCountryAndStatusViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatus>>> GetLiveByCountryAndStatus(
            LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            if (ModelState.IsValid)
            {
                var liveByCountryAndStatusUrl = ExtractPlaceholderUrlApi(liveByCountryAndStatusViewModel);

                var liveByCountryAndStatusUrlList = await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatus>>(liveByCountryAndStatusUrl);

                var liveByCountryAndStatusFilter = ApplySearchFilter(liveByCountryAndStatusUrlList, liveByCountryAndStatusViewModel);

                liveByCountryAndStatusViewModel.LiveByCountryAndStatus = liveByCountryAndStatusFilter;
            }

            liveByCountryAndStatusViewModel.Countries = await GetCountries();
            liveByCountryAndStatusViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", liveByCountryAndStatusViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, liveByCountryAndStatusViewModel.StatusType);
        }

        private IEnumerable<LiveByCountryAndStatus> ApplySearchFilter
            (IEnumerable<LiveByCountryAndStatus> liveByCountryAndStatusUrlList,
             LiveByCountryAndStatusViewModel liveByCountryAndStatusViewModel)
        {
            return liveByCountryAndStatusUrlList
                    .Where(live => live.Country.Equals(liveByCountryAndStatusViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
