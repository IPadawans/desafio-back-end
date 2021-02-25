using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SongsByWeather.Domain.Services;
namespace SongsByWeather.Services
{
    public class OpenWeatherMapsWeatherProviderService : IWeatherProviderService
    {
        static HttpClient client = new HttpClient();
        static readonly String baseURL = "https://api.openweathermap.org/data/2.5/weather";
        static String TOKEN_APP_ID;

        private readonly IConfiguration configuration;

        public OpenWeatherMapsWeatherProviderService(IConfiguration configuration)
        {
            this.configuration = configuration;
            TOKEN_APP_ID = configuration.GetSection("Credentials").GetSection("OpenWeather").GetSection("Token").Value;
        }

        public async Task<Nullable<double>> GetTemperaturaInCelsiusByLatitudeAndLongitude(double latitude, double longitude)
        {
            UriBuilder builder = new UriBuilder(baseURL);
            builder.Query = "lat=" + latitude + "&lon=" + longitude + "&appid=" + TOKEN_APP_ID;
            HttpResponseMessage response = client.GetAsync(builder.Uri).Result;
            return await DealWithApiResponse(response);
        }

        public async Task<Nullable<double>> GetTemperatureInCelsiusByCity(string city)
        {
            UriBuilder builder = new UriBuilder(baseURL);
            String encodedCity = Uri.EscapeDataString(city);
            builder.Query = "q=" + encodedCity + "&appid=" + TOKEN_APP_ID;
            HttpResponseMessage response = client.GetAsync(builder.Uri).Result;
            return await DealWithApiResponse(response);
        }

        public async Task<Nullable<double>> DealWithApiResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            String responseJson = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(responseJson);
            JToken temperature = jsonObject["main"]["temp"];
            double temperatureInKelvin = temperature.ToObject<double>();
            return GetTemperatureAsCelsius(temperatureInKelvin);
        }

        public double GetTemperatureAsCelsius(double temperatureInKelvin)
        {
            return temperatureInKelvin - 273.15;
        }
    }
}
