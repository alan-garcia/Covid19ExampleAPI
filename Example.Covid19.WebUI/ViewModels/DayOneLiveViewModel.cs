using Example.Covid19.API.DTO.DayOneCases;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.ViewModels
{
    /// <summary>
    ///     Representan los campos a mostrar en la vista de los casos por tipo para un país, desde el primer caso de COVID conocido 
    ///     con el último registro siendo el conteo en directo.
    /// </summary>
    public class DayOneLiveViewModel : CovidBaseViewModel
    {
        public IEnumerable<DayOneLive> DayOneLive { get; set; }
    }
}
