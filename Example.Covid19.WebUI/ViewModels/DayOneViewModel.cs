using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    public class DayOneViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOne> DayOne { get; set; }
    }
}
