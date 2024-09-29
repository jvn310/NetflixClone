// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiKey = '333c507e13a6708b1caa02ed821254c7';
async function getTrendingContent() {
    const region = document.getElementById('region').value;
    const contentType = document.getElementById('contentType').value;

    let language;
    if (contentType === 'movie-english' || contentType === 'tv-english') {
        language = 'en';
    } else if (contentType === 'movie-other' || contentType === 'tv-other') {
        language = null;
    }

    const discoverURL = `https://api.themoviedb.org/3/discover/${contentType.includes('movie') ? 'movie' : 'tv'}?api_key=${apiKey}${language ? `&with_original_language=${language}` : ''}&region=${region !== 'global' ? region : ''}`;

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
            <img src="https://image.tmdb.org/t/p/w500${movie.poster_path}" alt="${movie.title}">
        `;
        fragment.appendChild(movieCard);  // Append the movie card to the fragment
    });

    moviesGrid.appendChild(fragment);  // Append the fragment to the #moviesGrid
}

getTrendingContent();

