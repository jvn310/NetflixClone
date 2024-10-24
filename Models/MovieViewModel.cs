namespace NetflixClone.Models
{
    public class MovieViewModel
    {
        public Movie FeaturedMovie { get; set; } 
        public List<Movie> MovieList { get; set; }
        public List<Movie> Top10Movies { get; set; }
        public List<Movie> OnlyOnNetflixMovies { get; set; }
        public List<Movie> TrendingMovies { get; set; }
        public List<Movie> NewReleases { get; set; }
        public List<Movie> ContinueWatching { get; set; }
        public List<Movie> ActionMovies { get; set; }
        public List<Movie> ComedyMovies { get; set; }
        public List<Movie> DramaMovies { get; set; }
        public List<Movie> HorrorMovies { get; set; }
        public List<Movie> AnimationMovies { get; set; }



        public MovieViewModel()
        {
            FeaturedMovie = new Movie();
            MovieList = new List<Movie>();
            TrendingMovies = new List<Movie>();
            Top10Movies = new List<Movie>();
            OnlyOnNetflixMovies = new List<Movie>();
            NewReleases = new List<Movie>();
            ContinueWatching = new List<Movie>();
            ActionMovies = new List<Movie>();
            ComedyMovies = new List<Movie>();
            DramaMovies = new List<Movie>();
            HorrorMovies = new List<Movie>();
            AnimationMovies = new List<Movie>();

        }
    }
}
