using System;
using System.Threading.Tasks;

namespace SongsByWeather.Domain.Services
{
    public interface IWeatherProviderService
    {
        Task<Nullable<double>> GetTemperatureInCelsiusByCity(String city);
        Task<Nullable<double>> GetTemperaturaInCelsiusByLatitudeAndLongitude(double latitude, double longitude);
    }
}
