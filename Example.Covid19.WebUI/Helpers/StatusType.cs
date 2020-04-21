using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Example.Covid19.WebUI.Helpers
{
    public static class StatusType
    {
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
