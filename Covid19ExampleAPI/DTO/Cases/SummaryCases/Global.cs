namespace Covid19ExampleAPI.DTO.Cases.SummaryCases
{
    /// <summary>
    ///     Modelo que representa el resumen global de los casos del COVID-19
    /// </summary>
    public class Global
    {
        public int NewConfirmed { get; set; }
        public int TotalConfirmed { get; set; }
        public int NewDeaths { get; set; }
        public int TotalDeaths { get; set; }
        public int NewRecovered { get; set; }
        public int TotalRecovered { get; set; }
    }
}
