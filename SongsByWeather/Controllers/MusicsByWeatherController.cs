using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SongsByWeather.Services;
using SongsByWeather.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace SongsByWeather.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MusicsByWeatherController : ControllerBase {

        private readonly MusicsByWeatherService musicsByWeatherService;
        private readonly IConfiguration configuration;

        public MusicsByWeatherController(MusicsByWeatherService musicsByWeatherService, IConfiguration configuration)
        {
            this.musicsByWeatherService = musicsByWeatherService;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<MusicInformation>>> GetMusicsInformation(Nullable<double> latitude = null , Nullable<double> longitude = null, String cityName=null)
        {
            List<MusicInformation> musics = new List<MusicInformation>();

            if(latitude != null && longitude!= null)
            {
                musics.AddRange(await musicsByWeatherService.FindPlaylistByWeatherInformingLatitudeAndLongitude((double)latitude, (double)longitude));
            } else if(cityName != null)
            {
                musics.AddRange(await musicsByWeatherService.FindPlaylistByWeatherInformingCity(cityName));
            }

            bool isEmpty = !musics.Any();

            if (isEmpty)
            {
                return BadRequest();
            }

            return musics;
        }

    }
}
