using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SongsByWeather.Domain.Models;
using SongsByWeather.Domain.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SongsByWeather.Services
{
    public class SpotifyPlaylistProviderService : IPlaylistProviderService
    {
        private readonly HttpClient client = new HttpClient();
        private String SPOTIFY_TOKEN;

        private readonly IConfiguration configuration;

        public SpotifyPlaylistProviderService(IConfiguration configuration)
        {
            this.configuration = configuration;
            GetClientCredentialsAuthToken();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + SPOTIFY_TOKEN);
        }

        public async Task<string> FindWishedPlaylist(string filter)
        {
            UriBuilder builder = new UriBuilder("https://api.spotify.com/v1/search");
            builder.Query = "q=" + filter + "&type=playlist";
            HttpResponseMessage responseMessage = client.GetAsync(builder.Uri).Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }
            
            String textResponse = await responseMessage.Content.ReadAsStringAsync();
            JObject textAsJson = JObject.Parse(textResponse);
            JToken playlistLink = textAsJson["playlists"]["items"][0]["href"];
            return playlistLink.ToObject<string>();
        }

        public async Task<List<MusicInformation>> MusicsInformations(String playlistUrl)
        {
            if(playlistUrl == null)
            {
                return new List<MusicInformation>();
            }

            UriBuilder builder = new UriBuilder(playlistUrl);
            HttpResponseMessage responseMessage = client.GetAsync(builder.Uri).Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                return new List<MusicInformation>();
            }

            String textResponse = await responseMessage.Content.ReadAsStringAsync();
            JObject textAsJson = JObject.Parse(textResponse);
            List<MusicInformation> musicsInformations = new List<MusicInformation>();

            foreach(JToken actual in textAsJson["tracks"]["items"])
            {
                String musicName = actual["track"]["name"].ToObject<String>();
                String artistName = actual["track"]["artists"][0]["name"].ToObject<String>();
                musicsInformations.Add(new MusicInformation(artistName, musicName));
            }
            
            return musicsInformations;
        }

        public void GetClientCredentialsAuthToken()
        {
            var spotifyClient = configuration.GetSection("Credentials").GetSection("Spotify").GetSection("Client_id").Value;
            var spotifySecret = configuration.GetSection("Credentials").GetSection("Spotify").GetSection("SpotifySecret").Value;

            var webClient = new WebClient();

            var postparams = new NameValueCollection();
            postparams.Add("grant_type", "client_credentials");

            var authHeader = Convert.ToBase64String(Encoding.Default.GetBytes($"{spotifyClient}:{spotifySecret}"));
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authHeader);
            try
            {
                var tokenResponse = webClient.UploadValues("https://accounts.spotify.com/api/token", postparams);

                var textResponse = Encoding.UTF8.GetString(tokenResponse);
                JObject jsonObject = JObject.Parse(textResponse);
                JToken accessToken = jsonObject["access_token"];
                SPOTIFY_TOKEN = accessToken.ToObject<string>();
            }catch(Exception ignored)
            {
                return;
            }
        }
    }
}
