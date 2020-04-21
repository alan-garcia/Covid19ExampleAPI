using Example.Covid19.WebUI.DTO.Cases.LiveByCountryCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class LiveByCountryAndStatusViewModel : CovidBaseViewModel
    {
        public IEnumerable<LiveByCountryAndStatus> LiveByCountryAndStatus { get; set; }
    }
}
