using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
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
    public class ByCountryLiveController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";
        private const string DATEFROM_PLACEHOLDER = "{dateFrom}";
        private const string DATETO_PLACEHOLDER = "{dateTo}";

        public ByCountryLiveController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var byCountryViewModel = new ByCountryLiveViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(byCountryViewModel);
        }

        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(int? page)
        {
            var byCountryLiveListFilter = HttpContext.Session.GetString("ByCountryLiveListFilter");
            var byCountryLiveListFilterDeserialized = JsonConvert.DeserializeObject<IEnumerable<ByCountryLive>>(byCountryLiveListFilter);

            var pageNumber = page ?? 1;
            HttpContext.Session.SetString("ByCountryLiveListFilter", JsonConvert.SerializeObject(byCountryLiveListFilterDeserialized));

            ViewBag.ByCountryLiveFilterList = byCountryLiveListFilterDeserialized.ToPagedList(pageNumber, 15);

            ByCountryLiveViewModel byCountryLiveViewModel = new ByCountryLiveViewModel
            {
                ByCountryLive = byCountryLiveListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", byCountryLiveViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ByCountryLive>>> GetByCountryLive(ByCountryLiveViewModel byCountryLiveViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                var byCountryLiveUrl = ExtractPlaceholderUrlApi(byCountryLiveViewModel);

                var byCountryLiveList = await _apiService.GetAsync<IEnumerable<ByCountryLive>>(byCountryLiveUrl);

                var byCountryLiveListFilter = ApplySearchFilter(byCountryLiveList, byCountryLiveViewModel);

                byCountryLiveViewModel.ByCountryLive = byCountryLiveListFilter;

                var pageNumber = page ?? 1;
                HttpContext.Session.SetString("ByCountryLiveListFilter", JsonConvert.SerializeObject(byCountryLiveListFilter));

                ViewBag.byCountryLiveFilterList = byCountryLiveListFilter.ToPagedList(pageNumber, 15);
            }

            byCountryLiveViewModel.Countries = await GetCountries();
            byCountryLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", byCountryLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(ByCountryLiveViewModel byCountryLiveViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.BY_COUNTRY_LIVE_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, byCountryLiveViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, byCountryLiveViewModel.StatusType)
                            .Replace(DATEFROM_PLACEHOLDER, byCountryLiveViewModel.DateFrom.ToString("yyyy-MM-dd"))
                            .Replace(DATETO_PLACEHOLDER, byCountryLiveViewModel.DateTo.ToString("yyyy-MM-dd"));
        }

        private IEnumerable<ByCountryLive> ApplySearchFilter
            (IEnumerable<ByCountryLive> byCountryLiveList, ByCountryLiveViewModel byCountryLiveViewModel)
        {
            return byCountryLiveList
                    .Where(bc => bc.Country.Equals(byCountryLiveViewModel.Country) && bc.Status.Equals(byCountryLiveViewModel.StatusType))
                    .Where(bc => bc.Date >= byCountryLiveViewModel.DateFrom && bc.Date <= byCountryLiveViewModel.DateTo)
                    .OrderByDescending(bc => bc.Date.Date);
        }

    }
}
