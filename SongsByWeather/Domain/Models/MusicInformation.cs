using System;
namespace SongsByWeather.Domain.Models
{
    public class MusicInformation
    {
        public MusicInformation(String artist, String name)
        {
            this.Artist = artist;
            this.Name = name;
        }
        public String Artist { get; set; }
        public String Name { get; set; }
    }
}
