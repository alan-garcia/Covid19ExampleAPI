using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using Example.Covid19.WebUI.Helpers;
using Example.Covid19.WebUI.Services;
using Example.Covid19.WebUI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Example.Covid19.WebUI.Controllers
{
    public class LiveByCountryAndStatusAfterDateController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATE_PLACEHOLDER = "{date}";

        public LiveByCountryAndStatusAfterDateController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneLiveViewModel = new LiveByCountryAndStatusAfterDateViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneLiveViewModel);
        }

        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(int? page)
        {
            var liveByCountryAndStatusAfterDateListFilter = HttpContext.Session.GetString("LiveByCountryAndStatusAfterDateListFilter");
            var liveByCountryAndStatusAfterDateListFilterDeserialized = JsonConvert.DeserializeObject<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateListFilter);

            var pageNumber = page ?? 1;
            HttpContext.Session.SetString("LiveByCountryAndStatusAfterDateListFilter", JsonConvert.SerializeObject(liveByCountryAndStatusAfterDateListFilterDeserialized));

            ViewBag.LiveByCountryAndStatusAfterDateListFilter = liveByCountryAndStatusAfterDateListFilterDeserialized.ToPagedList(pageNumber, 15);

            LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel = new LiveByCountryAndStatusAfterDateViewModel
            {
                LiveByCountryAndStatusAfterDate = liveByCountryAndStatusAfterDateListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", liveByCountryAndStatusAfterDateViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<LiveByCountryAndStatusAfterDate>>> GetLiveByCountryAndStatusAfterDate(
            LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                var liveByCountryAndStatusAfterDateUrl = ExtractPlaceholderUrlApi(liveByCountryAndStatusAfterDateViewModel);

                var liveByCountryAndStatusAfterDateUrlList = 
                    await _apiService.GetAsync<IEnumerable<LiveByCountryAndStatusAfterDate>>(liveByCountryAndStatusAfterDateUrl);

                var liveByCountryAndStatusAfterDateFilter = 
                    ApplySearchFilter(liveByCountryAndStatusAfterDateUrlList, liveByCountryAndStatusAfterDateViewModel);

                liveByCountryAndStatusAfterDateViewModel.LiveByCountryAndStatusAfterDate = liveByCountryAndStatusAfterDateFilter;

                var pageNumber = page ?? 1;
                HttpContext.Session.SetString("LiveByCountryAndStatusAfterDateListFilter", JsonConvert.SerializeObject(liveByCountryAndStatusAfterDateFilter));

                ViewBag.LiveByCountryAndStatusAfterDateListFilter = liveByCountryAndStatusAfterDateFilter.ToPagedList(pageNumber, 15);
            }

            liveByCountryAndStatusAfterDateViewModel.Countries = await GetCountries();
            liveByCountryAndStatusAfterDateViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", liveByCountryAndStatusAfterDateViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.LIVE_BY_CONTRY_AND_STATUS_AFTERDATE_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.StatusType)
                            .Replace(DATE_PLACEHOLDER, liveByCountryAndStatusAfterDateViewModel.Date.ToString("yyyy-MM-ddThh:mm:ssZ"));
        }

        private IEnumerable<LiveByCountryAndStatusAfterDate> ApplySearchFilter
            (IEnumerable<LiveByCountryAndStatusAfterDate> liveByCountryAndStatusAfterDateUrlList,
             LiveByCountryAndStatusAfterDateViewModel liveByCountryAndStatusAfterDateViewModel)
        {
            return liveByCountryAndStatusAfterDateUrlList
                    .Where(live => live.Country.Equals(liveByCountryAndStatusAfterDateViewModel.Country))
                    .OrderByDescending(live => live.Date.Date);
        }

    }
}
