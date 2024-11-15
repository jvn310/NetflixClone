using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using System.Diagnostics;
using NetflixClone.Data;

namespace NetflixClone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NetflixCloneDbContext _context;

        public HomeController(ILogger<HomeController> logger, NetflixCloneDbContext context)
        {
            _logger = logger;
            _context = context;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["BodyClass"] = " ";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            ViewData["BodyClass"] = " ";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = false;
            ViewData["BodyClass"] = "lighter-background";
            return View();
        }

        [Route("Home/HomeNetflix")]
        public async Task<IActionResult> HomeNetflix()
        {
            ViewData["BodyClass"] = "black-background";
            ViewData["Page"] = "Movie";

            // Fetch movies from the database
            var movies = await _context.Movies.ToListAsync();

            // Find the featured movie
            var featuredMovie = movies.FirstOrDefault(m => m.IsFeatured) ?? new Movie(); ;

            // Pass featured movie
            ViewBag.FeaturedMovie = featuredMovie;

            // View model assigned to movie list
            var model = new MovieViewModel
            {
                MovieList = movies, // "We Think You'll Love These" section
                Top10Movies = GetTop10Movies(),
                OnlyOnNetflixMovies = GetOnlyOnNetflixMovies(),
                TrendingMovies = GetTrendingMovies(),
                NewReleases = GetNewReleases(),
                ContinueWatching = GetContinueWatchingMovies(),
                FeaturedMovie = featuredMovie,

                // Genre filtering
                ActionMovies = GetMoviesByGenre("Action"),
                ComedyMovies = GetMoviesByGenre("Comedy"),
                DramaMovies = GetMoviesByGenre("Drama"),
                HorrorMovies = GetMoviesByGenre("Horror"),
                AnimationMovies = GetMoviesByGenre("Animation")
            };

            // Pass the view model to the view
            return View(model);
        }

        private List<Movie> GetTop10Movies()
        {
            // Fetch the top 10 movies
            return _context.Movies
                .OrderByDescending(m => m.Rating) 
                .Take(10)
                .ToList();
        }

        private List<Movie> GetOnlyOnNetflixMovies()
        {
            // Fetch movies that are only on Netflix 
            return _context.Movies
                .Where(m => m.IsExclusiveToNetflix == true) 
                .ToList();
        }

        private List<Movie> GetTrendingMovies()
        {
            // Fetch trending movies
            return _context.Movies
                .OrderByDescending(m => m.Views) 
                .Take(10)
                .ToList();
        }

        private List<Movie> GetNewReleases()
        {
            // Fetch new releases
            return _context.Movies
                .Where(m => m.ReleaseDate >= DateTime.Now.AddMonths(-1)) // Last month releases
                .ToList();
        }


        private List<Movie> GetContinueWatchingMovies()
        {
            return _context.Movies
                .Where(m => m.IsCurrentlyBeingWatched == true) 
                .ToList();
        }

        private List<Movie> GetMoviesByGenre(string genre)
        {
            // Fetch movies by genre
            return _context.Movies
                .Where(m => m.Genre == genre)
                .ToList();
        }

        public IActionResult SignUp1()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        public ActionResult CheckInbox()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        public ActionResult CreatePassword()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View(); 
        }

        public ActionResult ChoosePlan()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignOut"] = true;
            return View();
        }

        public ActionResult ChoosePlan2()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignOut"] = true;
            return View();
        }
        public ActionResult PaymentSelection()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignOut"] = true;
            return View();
        }

        public ActionResult CardSetup()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignOut"] = true;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
