// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiKey = '333c507e13a6708b1caa02ed821254c7';

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
        displayTrendingMovies(data.results.slice(0, 10));  // Limit to top 10
    } catch (error) {
        console.error('Error fetching data:', error);
    }
}

// Display top 10 trending movies/TV shows
function displayTrendingMovies(movies) {
    const moviesGrid = document.getElementById('moviesGrid');
    moviesGrid.innerHTML = '';  // Clear the grid

    const fragment = document.createDocumentFragment();  // Create fragment for better performance

    movies.forEach((movie, index) => {
        const movieCard = document.createElement('div');
        movieCard.className = 'movie-card';
        movieCard.innerHTML = `
            <div class="movie-number">${index + 1}</div>
            <img src="https://image.tmdb.org/t/p/w500${movie.poster_path}" alt="${movie.title || movie.name}">
        `;
        fragment.appendChild(movieCard);  // Append the movie card to the fragment
    });

    moviesGrid.appendChild(fragment);  // Append the fragment to the #moviesGrid
}

// Initial call to populate content when the page loads
handleRegionChange();

