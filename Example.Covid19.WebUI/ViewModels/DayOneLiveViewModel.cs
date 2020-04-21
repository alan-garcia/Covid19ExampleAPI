using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class DayOneLiveViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOneLive> DayOneLive { get; set; }
    }
}
