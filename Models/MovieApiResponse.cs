namespace NetflixClone.Models
{
    public class MovieApiResponse
    {
        public List<MovieData>? Results { get; set; }
        public int Page { get; set; }
        public int TotalResults { get; set; } 
        public int TotalPages { get; set; }
    }

    public class MovieData
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Genre { get; set; }
    }
}
