namespace Example.Covid19.API.Services
{
    public interface ICovid19MemoryCacheService
    {
        bool Get<T>(string key, out T entry) where T : class;
        void Set<T>(string key, T entry) where T : class;
    }
}
