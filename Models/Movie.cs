namespace NetflixClone.Models
{
    public class Movie
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }
        public double Rating { get; set; } 
        public bool IsExclusiveToNetflix { get; set; } 
        public int Views { get; set; } 
        public bool IsCurrentlyBeingWatched { get; set; } 
        public bool IsTop10 { get; set; }
    }
}
