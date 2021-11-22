using System.Threading.Tasks;

namespace Example.Covid19.API.Services
{
    /// <summary>
    ///     Contracto para describir los servicios de la API a consumir
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        ///     Obtiene los datos de la API con base en una URL concreta
        /// </summary>
        /// <typeparam name="T">API concreta a consumir</typeparam>
        /// <param name="apiUrl">URL concreta de la API</param>
        /// <returns>Toda la información relacionada con la API especificada en la URL</returns>
        Task<T> GetAsync<T>(string apiUrl) where T : class;
    }
}
