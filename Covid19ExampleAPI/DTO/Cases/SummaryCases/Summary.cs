using System;

namespace Covid19ExampleAPI.DTO.Cases.SummaryCases
{
    /// <summary>
    ///     Modelo que representa el resumen global y los casos por países con la última fecha actualizada de los datos
    /// </summary>
    public class Summary
    {
        public Global Global { get; set; }
        public CountryInfo[] Countries { get; set; }
        public DateTime Date { get; set; }
    }
}
