using Example.Covid19.WebUI.DTO.Cases.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista todos los casos totales por tipo para un país, desde el primer caso de COVID conocido
    /// </summary>
    public class DayOneTotalViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOneTotal> DayOneTotal { get; set; }
    }
}
