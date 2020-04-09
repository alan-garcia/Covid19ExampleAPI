using System.Threading.Tasks;

namespace Covid19ExampleAPI.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string urlApi) where T: class;
    }
}
