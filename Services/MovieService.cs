using NetflixClone.Models;
using NetflixClone.Data;
using System.Text.Json;

namespace NetflixClone.Services
{
    public class MovieService
    {
        private readonly NetflixCloneDbContext _context;
        private readonly HttpClient _httpClient;

        public MovieService(NetflixCloneDbContext context, HttpClient httpClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Movie>(); 
            }

            // First, search in the local database
            var localMovies = _context.Movies
                .Where(m => m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // If there are results in the database, return them
            if (localMovies.Any())
            {
                return localMovies;
            }

            // Otherwise, fetch from the external API
            var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key=333c507e13a6708b1caa02ed821254c7&query={Uri.EscapeDataString(query)}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var movieData = JsonSerializer.Deserialize<MovieApiResponse>(content);

                if (movieData?.Results != null)
                {
                    return movieData.Results.Select(movie => new Movie
                    {
                        Id = movie.Id,
                        Title = movie.Title ?? "Unknown Title",
                        Description = movie.Description ?? "No description available",
                        PosterUrl = !string.IsNullOrEmpty(movie.PosterUrl) ? "https://image.tmdb.org/t/p/w500" + movie.PosterUrl : string.Empty,
                        ReleaseDate = movie.ReleaseDate ?? DateTime.Now,
                        Genre = movie.Genre ?? "Unknown Genre"
                    }).ToList();
                }
            }

            return new List<Movie>(); 
        }


        public async Task FetchAndAddMoviesAsync()
        {
            int page = 1;
            bool morePages = true;

            while (morePages)
            {
                var response = await _httpClient.GetAsync($"https://api.themoviedb.org/3/movie/popular?api_key=333c507e13a6708b1caa02ed821254c7&page={page}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var movieData = JsonSerializer.Deserialize<MovieApiResponse>(content);

                    // Debugging: Log the API response content to see what is being returned
                    Console.WriteLine($"API Response for page {page}: {content}");

                    if (movieData?.Results != null && movieData.Results.Any())
                    {
                        var newMovies = new List<Movie>(); // To collect new movies to save in bulk

                        foreach (var movie in movieData.Results)
                        {
                            if (!_context.Movies.Any(m => m.Id == movie.Id))
                            {
                                var newMovie = new Movie
                                {
                                    Id = movie.Id,
                                    Title = movie.Title ?? "Unknown Title",
                                    Description = movie.Description ?? "No description available",
                                    PosterUrl = !string.IsNullOrEmpty(movie.PosterUrl) ? "https://image.tmdb.org/t/p/w500" + movie.PosterUrl : string.Empty,
                                    ReleaseDate = movie.ReleaseDate ?? DateTime.Now,
                                    Genre = movie.Genre ?? "Unknown Genre"
                                };
                                newMovies.Add(newMovie); // Add to the list instead of saving immediately
                            }
                        }

                        if (newMovies.Any())
                        {
                            _context.Movies.AddRange(newMovies); // Add all new movies at once
                            await _context.SaveChangesAsync(); // Save all movies in a single transaction
                        }
                    }

                    // Debugging: Check if more pages are available by inspecting the 'total_pages' field
                    if (movieData?.TotalPages <= page)
                    {
                        morePages = false; // End the loop if we've reached the last page
                    }
                    else
                    {
                        page++; // Move to the next page
                    }
                }
                else
                {
                    morePages = false; // End the loop if the response fails
                    // Optionally log the error or retry here
                    Console.WriteLine("API request failed.");
                }
            }
        }
    }
}
