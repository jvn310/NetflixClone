namespace NetflixClone.Services.BackgroundServices
{
    public class MovieFetchBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MovieFetchBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var movieService = scope.ServiceProvider.GetRequiredService<MovieService>();
                    await movieService.FetchAndAddMoviesAsync();
                }

                // Wait for a specific time interval before fetching more movies again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
            }
        }
    }
}
