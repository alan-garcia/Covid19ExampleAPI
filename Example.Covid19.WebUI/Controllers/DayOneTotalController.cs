using Example.Covid19.WebUI.Config;
using Example.Covid19.WebUI.DTO.Cases.CountriesCases;
using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
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
    public class DayOneTotalController : BaseController
    {
        private const string COUNTRYNAME_PLACEHOLDER = "{countryName}";
        private const string STATUS_PLACEHOLDER = "{status}";

        public DayOneTotalController(IApiService apiService, IConfiguration config) : base(apiService, config)
        {
            _apiService = apiService;
            _config = config;
        }

        public async Task<ActionResult<IEnumerable<Countries>>> Index()
        {
            var dayOneTotalViewModel = new DayOneTotalViewModel
            {
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View(dayOneTotalViewModel);
        }

        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(int? page)
        {
            var dayOneTotalByCountryListFilter = HttpContext.Session.GetString("DayOneTotalByCountryListFilter");
            var dayOneTotalByCountryListFilterDeserialized = JsonConvert.DeserializeObject<IEnumerable<DayOneTotal>>(dayOneTotalByCountryListFilter);

            var pageNumber = page ?? 1;
            HttpContext.Session.SetString("DayOneTotalByCountryListFilter", JsonConvert.SerializeObject(dayOneTotalByCountryListFilterDeserialized));

            ViewBag.DayOneTotalByCountryListFilter = dayOneTotalByCountryListFilterDeserialized.ToPagedList(pageNumber, 15);

            DayOneTotalViewModel dayOneTotalViewModel = new DayOneTotalViewModel
            {
                DayOneTotal = dayOneTotalByCountryListFilterDeserialized,
                Countries = await GetCountries(),
                StatusTypeList = StatusType.GetStatusTypeList()
            };

            return View("Index", dayOneTotalViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DayOneTotal>>> GetDayOneTotalByCountry(DayOneTotalViewModel dayOneTotalLiveViewModel, int? page)
        {
            if (ModelState.IsValid)
            {
                var dayOneTotalUrl = ExtractPlaceholderUrlApi(dayOneTotalLiveViewModel);

                var dayOneTotalByCountryList = await _apiService.GetAsync<IEnumerable<DayOneTotal>>(dayOneTotalUrl);

                var dayOneTotalByCountryListFilter = ApplySearchFilter(dayOneTotalByCountryList, dayOneTotalLiveViewModel);

                dayOneTotalLiveViewModel.DayOneTotal = dayOneTotalByCountryListFilter;

                var pageNumber = page ?? 1;
                HttpContext.Session.SetString("DayOneTotalByCountryListFilter", JsonConvert.SerializeObject(dayOneTotalByCountryListFilter));

                ViewBag.DayOneTotalByCountryListFilter = dayOneTotalByCountryListFilter.ToPagedList(pageNumber, 15);
            }

            dayOneTotalLiveViewModel.Countries = await GetCountries();
            dayOneTotalLiveViewModel.StatusTypeList = StatusType.GetStatusTypeList();

            return View("Index", dayOneTotalLiveViewModel);
        }

        private async Task<IEnumerable<SelectListItem>> GetCountries()
        {
            var countries = await GetRequestData<IEnumerable<Countries>>(AppSettingsConfig.COUNTRIES_KEY);

            return CountriesList.BuildAndGetCountriesSelectListItem(countries);
        }

        private string ExtractPlaceholderUrlApi(DayOneTotalViewModel dayOneTotalLiveViewModel)
        {
            return _config.GetValue<string>($"{AppSettingsConfig.COVID19API_KEY}:{AppSettingsConfig.DAYONE_TOTAL_KEY}")
                            .Replace(COUNTRYNAME_PLACEHOLDER, dayOneTotalLiveViewModel.Country)
                            .Replace(STATUS_PLACEHOLDER, dayOneTotalLiveViewModel.StatusType);
        }

        private IEnumerable<DayOneTotal> ApplySearchFilter
            (IEnumerable<DayOneTotal> dayOneTotalByCountryList, DayOneTotalViewModel dayOneTotalLiveViewModel)
        {
            return dayOneTotalByCountryList
                    .Where(day => day.Country.Equals(dayOneTotalLiveViewModel.Country) && day.Status.Equals(dayOneTotalLiveViewModel.StatusType))
                    .OrderByDescending(day => day.Date.Date);
        }

    }
}
