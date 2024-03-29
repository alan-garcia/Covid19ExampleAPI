﻿namespace Example.Covid19.API.Models
{
    /// <summary>
    ///     Representa el modelo de las opciones a cargar para el fichero "appsettings.json"
    /// </summary>
    public class CovidApiAppSettingsModel
    {
        public string UrlBase { get; set; }
        public string Summary { get; set; }
        public string Countries { get; set; }
        public string DayOne { get; set; }
        public string DayOneLive { get; set; }
        public string DayOneTotal { get; set; }
        public string ByCountry { get; set; }
        public string ByCountryLive { get; set; }
        public string ByCountryTotal { get; set; }
        public string LiveByCountryAndStatus { get; set; }
        public string LiveByCountryAndStatusAfterDate { get; set; }
        public string Stats { get; set; }
    }
}
