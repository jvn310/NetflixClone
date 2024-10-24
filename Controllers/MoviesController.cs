using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;
using NetflixClone.Services;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Controllers
{
    public class MoviesController : Controller
    {
        private readonly NetflixCloneDbContext _context;
        private readonly TmdbService _tmdbService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(NetflixCloneDbContext context, TmdbService tmdbService, ILogger<MoviesController> logger)
        {
            _context = context;
            _tmdbService = tmdbService;
            _logger = logger;
        }

        // Fetch and store popular movies
        public async Task<IActionResult> FetchPopularMovies()
        {
            _logger.LogInformation("Fetching popular movies...");

            var popularMoviesData = await _tmdbService.GetPopularMoviesAsync();
            _logger.LogInformation("Popular movies data: {data}", popularMoviesData.ToString());

            if (popularMoviesData == null)
            {
                _logger.LogError("Failed to fetch popular movies from TMDB.");
                return BadRequest("Failed to fetch popular movies.");
            }

            foreach (var movie in popularMoviesData["results"] ?? Enumerable.Empty<dynamic>())
            {
                var movieId = movie["id"] != null ? (int)movie["id"] : 0;
                if (movieId == 0) continue;

                _logger.LogInformation("Fetching movie details for movie ID: {movieId}", movieId);

                var movieDetails = await _tmdbService.GetMovieDetailsAsync(movieId);
                var movieTrailer = await _tmdbService.GetMovieTrailerAsync(movieId);
                string trailerUrl = ExtractTrailerUrl(movieTrailer);
                string posterUrl = $"https://image.tmdb.org/t/p/w500{movieDetails["backdrop_path"]}";

                _logger.LogInformation("Saving movie: {title}", movieDetails["title"]);

                if (!string.IsNullOrEmpty(posterUrl))
                {
                    var newMovie = new Movie
                    {
                        Title = movieDetails["title"]?.ToString() ?? "Unknown Title",
                        Description = movieDetails["overview"]?.ToString() ?? "No Description Available",
                        PosterUrl = posterUrl,
                        TrailerUrl = trailerUrl,
                        ReleaseDate = DateTime.TryParse(movieDetails["release_date"]?.ToString(), out var releaseDate) ? releaseDate : DateTime.MinValue,
                        Genre = movieDetails["genres"]?[0]?["name"]?.ToString() ?? "Unknown Genre",
                        Category = "Popular",
                        IsFeatured = false
                    };

                    _context.Movies.Add(newMovie);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Movies fetched and saved successfully.");
            return RedirectToAction("HomeNetflix");
        }

        public async Task<IActionResult> FetchOnlyOnNetflixMovies()
        {
            _logger.LogInformation("Fetching Netflix original movies...");

            // Call the TMDB API for Netflix original movies
            var netflixMoviesData = await _tmdbService.GetNetflixOriginalMoviesAsync();
            _logger.LogInformation("Netflix original movies data: {data}", netflixMoviesData.ToString());

            if (netflixMoviesData == null)
            {
                _logger.LogError("Failed to fetch Netflix original movies from TMDB.");
                return BadRequest("Failed to fetch Netflix original movies.");
            }

            foreach (var movie in netflixMoviesData["results"] ?? Enumerable.Empty<dynamic>())
            {
                var movieId = movie["id"] != null ? (int)movie["id"] : 0;
                if (movieId == 0) continue;

                _logger.LogInformation("Fetching movie details for movie ID: {movieId}", movieId);

                var movieDetails = await _tmdbService.GetMovieDetailsAsync(movieId);
                var movieTrailer = await _tmdbService.GetMovieTrailerAsync(movieId);
                string trailerUrl = ExtractTrailerUrl(movieTrailer);
                string posterUrl = $"https://image.tmdb.org/t/p/w500{movieDetails["backdrop_path"]}";

                _logger.LogInformation("Saving Netflix original movie: {title}", movieDetails["title"]);

                if (!string.IsNullOrEmpty(posterUrl))
                {
                    var newMovie = new Movie
                    {
                        Title = movieDetails["title"]?.ToString() ?? "Unknown Title",
                        Description = movieDetails["overview"]?.ToString() ?? "No Description Available",
                        PosterUrl = posterUrl,
                        TrailerUrl = trailerUrl,
                        ReleaseDate = DateTime.TryParse(movieDetails["release_date"]?.ToString(), out var releaseDate) ? releaseDate : DateTime.MinValue,
                        Genre = movieDetails["genres"]?[0]?["name"]?.ToString() ?? "Unknown Genre",
                        Category = "Only on Netflix", // Mark this as Netflix exclusive
                        IsExclusiveToNetflix = true,   // Flag for exclusive content
                        IsFeatured = false
                    };

                    // Check if movie is already saved in the database to avoid duplicates
                    var existingMovie = _context.Movies.FirstOrDefault(m => m.Title == newMovie.Title);
                    if (existingMovie == null)
                    {
                        _context.Movies.Add(newMovie);
                    }
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Netflix original movies fetched and saved successfully.");
            return RedirectToAction("HomeNetflix");
        }



        // Helper method to extract trailer URL
        private string ExtractTrailerUrl(JObject movieTrailer)
        {
            if (movieTrailer != null && movieTrailer.ContainsKey("results"))
            {
                var results = movieTrailer["results"];
                if (results != null && results.Count() > 0)
                {
                    var trailerKey = results[0]?["key"]?.ToString();
                    if (!string.IsNullOrEmpty(trailerKey))
                    {
                        return $"https://www.youtube.com/watch?v={trailerKey}";
                    }
                }
            }
            return string.Empty;
        }       
    }
}