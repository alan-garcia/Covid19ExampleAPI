using Example.Covid19.API.DTO.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista de los casos por tipo para un país, desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOne> DayOne { get; set; }
    }
}
