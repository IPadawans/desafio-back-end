using SongsByWeather.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongsByWeather.Domain.Services
{
    public interface IPlaylistProviderService
    {
        Task<string> FindWishedPlaylist(String filter);

        Task<List<MusicInformation>> MusicsInformations(String playlistUrl);
    }
}
