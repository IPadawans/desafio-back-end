using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongsByWeather.Domain.Models;
using SongsByWeather.Domain.Services;

namespace SongsByWeather.Services
{
    public class MusicsByWeatherService
    {
        private readonly IWeatherProviderService _weatherProviderService;
        private readonly IPlaylistProviderService _playlistProviderService;

        public MusicsByWeatherService(IWeatherProviderService weatherProviderService, IPlaylistProviderService playlistProviderService)
        {
            this._weatherProviderService = weatherProviderService;
            this._playlistProviderService = playlistProviderService;
        }

        public async Task<List<MusicInformation>> FindPlaylistByWeatherInformingCity(String city)
        {
            Nullable<double> temperatureInCelsius = await _weatherProviderService.GetTemperatureInCelsiusByCity(city);
            return await SuggestPlaylistByTemperature(temperatureInCelsius);
        }

        public async Task<List<MusicInformation>> FindPlaylistByWeatherInformingLatitudeAndLongitude(double latitude, double longitude)
        {
            Nullable<double> temperatureInCelsius = await _weatherProviderService.GetTemperaturaInCelsiusByLatitudeAndLongitude(latitude, longitude);
            return await SuggestPlaylistByTemperature(temperatureInCelsius);
        }

        public async Task<List<MusicInformation>> SuggestPlaylistByTemperature(Nullable<double> temperature)
        {
            if(temperature == null)
            {
                return new List<MusicInformation>();
            }
            String wishedPlaylist = null;
            if(temperature > 30)
            {
                wishedPlaylist = await _playlistProviderService.FindWishedPlaylist("party");
            } else if (temperature >= 15 && temperature <= 30)
            {
                wishedPlaylist= await _playlistProviderService.FindWishedPlaylist("pop");
            } else if (temperature >= 10 && temperature <= 14)
            {
                wishedPlaylist = await _playlistProviderService.FindWishedPlaylist("rock");
            } else
            {
                wishedPlaylist = await _playlistProviderService.FindWishedPlaylist("classic");
            }


            return await _playlistProviderService.MusicsInformations(wishedPlaylist);
        }
    }
}
