using System.Threading.Tasks;

namespace Example.Covid19.WebUI.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string urlApi) where T: class;
    }
}
