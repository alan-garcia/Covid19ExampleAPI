using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.Helpers
{
    /// <summary>
    ///     Clase para representar los tipos de estados
    /// </summary>
    public static class StatusType
    {
        /// <summary>
        ///     Obtiene una lista de los tipos de estados
        /// </summary>
        /// <returns>Lista de los tipos de estados, para ser representado como un elemento HTML de tipo "option"
        /// en los formularios de tipo "lista desplegable"</returns>
        public static List<SelectListItem> GetStatusTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() { Text = "Confirmados", Value = "confirmed" },
                new SelectListItem() { Text = "Recuperados", Value = "recovered" },
                new SelectListItem() { Text = "Muertos", Value = "deaths" }
            };
        }
    }
}
