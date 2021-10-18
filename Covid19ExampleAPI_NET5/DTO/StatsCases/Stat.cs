namespace Example.Covid19.API.DTO.StatsCases
{
    /// <summary>
    ///     Representan las estadísticas globales de los casos de COVID-19
    /// </summary>
    public class Stat
    {
        public int Total { get; set; }
        public int All { get; set; }
        public int Countries { get; set; }
        public int ByCountry { get; set; }
        public int ByCountryLive { get; set; }
        public int ByCountryTotal { get; set; }
        public int DayOne { get; set; }
        public int DayOneLive { get; set; }
        public int DayOneTotal { get; set; }
        public int LiveCountryStatus { get; set; }
        public int LiveCountryStatusAfterDate { get; set; }
        public int Stats { get; set; }
        public int Default { get; set; }
        public int Summary { get; set; }
    }
}
