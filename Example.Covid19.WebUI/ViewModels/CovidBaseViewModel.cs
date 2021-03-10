﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Covid19.WebUI.ViewModels
{
    public class CovidBaseViewModel
    {
        [Required(ErrorMessage = "El pais es obligatorio")]
        public string Country { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string StatusType { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}