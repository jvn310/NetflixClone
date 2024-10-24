// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiKey = '333c507e13a6708b1caa02ed821254c7';
let movies = [];
let currentSlideIndex = 0;
const slidesToShow = 5; // Number of movie cards visible per slide
let genresMap = {};


// Function to handle region change
function handleRegionChange() {
    const region = document.getElementById('region').value;
    const contentTypeSelect = document.getElementById('contentType');

    contentTypeSelect.innerHTML = ''; // Clear previous options

    if (region === 'ZA') {
        // South Africa: Show Movies and TV Shows without language options
        contentTypeSelect.innerHTML = `
            <option value="movie-za">Movies</option>
            <option value="tv-za">TV Shows</option>
        `;
    } else if (region === 'global') {
        // Global: Show Movies and TV Shows with language options
        contentTypeSelect.innerHTML = `
            <option value="movie-english">Movies – English</option>
            <option value="movie-other">Movies – Other Languages</option>
            <option value="tv-english">TV Shows – English</option>
            <option value="tv-other">TV Shows – Other Languages</option>
        `;
    }

    getTrendingContent(); // Fetch content based on the new selection
}

async function getTrendingContent() {
    const region = document.getElementById('region').value;
    const contentType = document.getElementById('contentType').value;

    let language;
    if (contentType === 'movie-english' || contentType === 'tv-english') {
        language = 'en';
    } else if (contentType === 'movie-other' || contentType === 'tv-other') {
        language = null; // Show all languages
    }

    const isZA = region === 'ZA'; // Check if South Africa is selected
    const type = contentType.includes('movie') ? 'movie' : 'tv';

    const discoverURL = `https://api.themoviedb.org/3/discover/${type}?api_key=${apiKey}${language ? `&with_original_language=${language}` : ''
        }&region=${isZA ? 'ZA' : ''}`;

    try {
        const response = await fetch(discoverURL);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        movies = data.results.slice(0, 10);  // Limit to top 10 movies/TV shows
        displayTrendingMovies(currentSlideIndex);  // Limit to top 10
    } catch (error) {
        console.error('Error fetching data:', error);
    }
}

// Display top 12 trending movies/TV shows
function displayTrendingMovies(slideIndex) {
    const moviesGrid = document.getElementById('moviesGrid');
    moviesGrid.innerHTML = '';  // Clear the grid

    const fragment = document.createDocumentFragment();  // Create fragment for better performance

    // Calculate which movies to display on the current slide
    const start = slideIndex * 5;  // Show 5 movies per slide
    const end = start + 5;
    const currentMovies = movies.slice(start, end);

    currentMovies.forEach((movie, index) => {
        const movieCard = document.createElement('div');
        movieCard.className = 'movie-card';

        // Check if Netflix is listed as a production company
        const isNetflixOriginal = movie.production_companies && movie.production_companies.some(company => company.name === 'Netflix');

        // Add Netflix logo if it's a Netflix original
        const netflixLogo = isNetflixOriginal ? '<img src="/images/netflix_.png" alt="Netflix Original" class="netflix-logo">' : '';

        movieCard.innerHTML = `
    <div class="movie-number">${start + index + 1}</div>
    <img src="https://image.tmdb.org/t/p/w500${movie.poster_path}" alt="${movie.title || movie.name}">
    <div class="logo-container">
        ${netflixLogo}
    </div>
`;

        movieCard.addEventListener('click', () => showMovieDetails(movie)); // Click event for movie card
        fragment.appendChild(movieCard);  // Append the movie card to the fragment
    });

    moviesGrid.appendChild(fragment);  // Append the fragment to the #moviesGrid
}

// Function to go to the next slide
function nextSlide() {
    if ((currentSlideIndex + 1) * 5 < movies.length) {
        currentSlideIndex++;
        displayTrendingMovies(currentSlideIndex);
    }
}

// Function to go to the previous slide
function prevSlide() {
    if (currentSlideIndex > 0) {
        currentSlideIndex--;
        displayTrendingMovies(currentSlideIndex);
    }
}

// Function to display more movie details when a card is clicked
function showMovieDetails(movie) {
    const modal = document.getElementById('movieModal');
    const movieTitle = document.getElementById('movieTitle');
    const moviePoster = document.getElementById('moviePoster');
    const movieDescription = document.getElementById('movieDescription');
    const movieYear = document.getElementById('movieYear');
    const movieGenres = document.getElementById('movieGenres');

    // Set the movie details in the modal
    movieTitle.textContent = movie.title || movie.name;
    moviePoster.src = `https://image.tmdb.org/t/p/w500${movie.poster_path}`;
    movieDescription.textContent = movie.overview || 'No description available.';
    movieYear.textContent = movie.release_date ? new Date(movie.release_date).getFullYear() : 'Unknown';

    // Populate genres
    movieGenres.innerHTML = '';  // Clear previous genres
    movie.genre_ids.forEach(genreId => {
        const genreElement = document.createElement('span');
        genreElement.classList.add('genre');
        genreElement.textContent = getGenreName(genreId);  // Helper function to convert genre id to name
        movieGenres.appendChild(genreElement);
    });

    // Show the modal
    modal.style.display = 'block';
}

function fetchGenres() {
    const genreUrl = `https://api.themoviedb.org/3/genre/movie/list?api_key=${apiKey}&language=en-US`;

    fetch(genreUrl)
        .then(response => response.json())
        .then(data => {
            // Map of genre IDs to genre names
            data.genres.forEach(genre => {
                genresMap[genre.id] = genre.name;
            });
        })
        .catch(error => {
            console.error('Error fetching genres:', error);
        });
}

fetchGenres();

// Helper function to map genre ID to genre name
function getGenreName(genreId) {
    return genresMap[genreId] || 'Unknown';
}

// Close modal when the close button is clicked
const modal = document.getElementById('movieModal');
const closeButton = document.querySelector('.modal .close');

closeButton.addEventListener('click', () => {
    modal.style.display = 'none';
});

// Close modal when clicking outside of the modal content
window.addEventListener('click', (event) => {
    if (event.target === modal) {
        modal.style.display = 'none';
    }
});

//Faq questions interactivity
document.querySelectorAll('.faq-question').forEach(question => {
    question.addEventListener('click', () => {
        const parent = question.parentElement;

        // Toggle the active class
        parent.classList.toggle('active');

        // Close any other open FAQs
        document.querySelectorAll('.faq-item').forEach(item => {
            if (item !== parent) {
                item.classList.remove('active');
            }
        });
    });
});

function showTrailer(trailerUrl, title, description, posterUrl) {
    // Set movie details
    document.getElementById('movieTitle').textContent = title;
    document.getElementById('movieDescription').textContent = description;
    document.getElementById('moviePoster').src = posterUrl;

    // Set the trailer URL and display the iframe
    const trailerIframe = document.getElementById('trailerIframe');
    trailerIframe.src = trailerUrl;

    // Show the modal
    const modal = new bootstrap.Modal(document.getElementById('trailerModal'));
    modal.show();
}


// Initial call to populate content when the page loads
handleRegionChange();

