using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;
using System.Diagnostics;
using NetflixClone.Data;
using NetflixClone.Services;
using Microsoft.AspNetCore.Antiforgery;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace NetflixClone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NetflixCloneDbContext _context;
        private readonly ProfileService _profileService;
        private readonly MovieService _movieService;
        private readonly IAntiforgery _antiforgery;

        public HomeController(ILogger<HomeController> logger, NetflixCloneDbContext context, ProfileService profileService, MovieService movieService, IAntiforgery antiforgery)
        {
            _logger = logger;
            _context = context;
            _profileService = profileService;
            _movieService = movieService;
            _antiforgery = antiforgery;
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

        public IActionResult Login()
        {
            ViewData["BodyClass"] = " ";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = false;
            ViewData["BodyClass"] = "lighter-background";
            return View();
        }

        [HttpPost]
        public ActionResult HomeNetflix(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && VerifyPassword(password, user.Password))
            {
                TempData["UserName"] = email; 
                return RedirectToAction("HomeNetflix");
            }

            ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
            return View("Login"); 
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var hash = Convert.ToBase64String(bytes);

                return hash == hashedPassword;
            }
        }

        [Route("Home/HomeNetflix")]
        public async Task<IActionResult> HomeNetflix()
        {
            ViewData["BodyClass"] = "black-background";
            ViewData["Page"] = "Movie";

            var profiles = _profileService.GetAllProfiles(); 
            ViewData["Profiles"] = profiles;

            var selectedProfileName = HttpContext.Session.GetString("SelectedProfileName") ?? "Guest";
            var selectedProfileIcon = HttpContext.Session.GetString("SelectedProfileIcon") ?? "#ffffff";

            ViewData["SelectedProfileName"] = selectedProfileName;
            ViewData["SelectedProfileIcon"] = selectedProfileIcon;

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

        [HttpGet]
        public IActionResult SignUp1()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult SignUp1(string email)
        {
            ViewBag.Email = email;
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        [HttpGet]
        public ActionResult CheckInbox()
        {
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult CheckInbox(string email)
        {
            ViewBag.Email = email;
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        public ActionResult CreatePassword(string email)
        {
            ViewBag.Email = email;
            ViewData["BodyClass"] = "white-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View(); 
        }

        [HttpPost]
        public ActionResult CreatePassword(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = new User
            {
                Email = email,
                Password = hashedPassword,
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("ChoosePlan");
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

        public ActionResult WhoIsWatching()
        {
            int userId = 1;

            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewData["__RequestVerificationToken"] = tokens.RequestToken;

            // Fetch profiles for the logged-in user
            var profiles = _profileService.GetProfilesByUserId(userId);

            profiles ??= new List<Profile>();
            ViewData["BodyClass"] = "black-background";
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = false;

            return View("~/Views/Home/WhoIsWatching.cshtml", profiles);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query cannot be null or empty.");
            }

            var searchResults = await _movieService.SearchMoviesAsync(query);
            return Json(searchResults); 
        }

            public IActionResult Logout()
            {
                HttpContext.Session.Clear();

                if (Request.Cookies["UserAuth"] != null)
                {
                    Response.Cookies.Delete("UserAuth");
                }

                return RedirectToAction("Login", "Home");
            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
