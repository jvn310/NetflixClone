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

                    if (movieData?.Results != null && movieData.Results.Any())
                    {
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
                                _context.Movies.Add(newMovie);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Check if there are more pages
                    if (movieData?.Results == null || movieData.Results.Count < 20)
                    {
                        morePages = false; // End the loop if fewer than 20 movies returned
                    }
                    else
                    {
                        page++; // Move to the next page
                    }
                }
                else
                {
                    morePages = false; // End the loop if the response fails
                }
            }
        }
    }
}
