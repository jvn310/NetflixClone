using Newtonsoft.Json.Linq;

namespace NetflixClone.Services
{
    public class TmdbService
    {
        private readonly string _apiKey = "333c507e13a6708b1caa02ed821254c7";
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public TmdbService(ILogger<TmdbService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<JObject> GetPopularMoviesAsync()
        {
            _logger.LogInformation("Fetching popular movies from TMDB...");
            var response = await _httpClient.GetStringAsync($"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&language=en-US&page=1");
            _logger.LogInformation("Received response from TMDB. Parsing JSON...");
            var result = JObject.Parse(response);
            _logger.LogInformation("Parsed JSON response from TMDB.");
            return result;
        }

        public async Task<JObject> GetMovieDetailsAsync(int movieId)
        {
            _logger.LogInformation($"Fetching movie details for movie ID {movieId} from TMDB...");
            var response = await _httpClient.GetStringAsync($"https://api.themoviedb.org/3/movie/{movieId}?api_key={_apiKey}&language=en-US");
            _logger.LogInformation($"Received response from TMDB for movie ID {movieId}. Parsing JSON...");
            var result = JObject.Parse(response);
            _logger.LogInformation($"Parsed JSON response from TMDB for movie ID {movieId}.");
            return result;
        }

        public async Task<JObject> GetNetflixOriginalMoviesAsync()
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={_apiKey}&with_networks=213"; // 213 is the ID for Netflix
            var response = await _httpClient.GetStringAsync(url);
            return JObject.Parse(response);
        }


        public async Task<JObject> GetMovieTrailerAsync(int movieId)
        {
            _logger.LogInformation($"Fetching movie trailer for movie ID {movieId} from TMDB...");
            var response = await _httpClient.GetStringAsync($"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key={_apiKey}&language=en-US");
            _logger.LogInformation($"Received response from TMDB for movie ID {movieId}. Parsing JSON...");
            var result = JObject.Parse(response);
            _logger.LogInformation($"Parsed JSON response from TMDB for movie ID {movieId}.");
            return result;
        }
    }
}
