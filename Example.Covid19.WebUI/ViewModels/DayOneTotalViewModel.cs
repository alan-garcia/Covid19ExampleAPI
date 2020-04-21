using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class DayOneTotalViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOneTotal> DayOneTotal { get; set; }
    }
}
