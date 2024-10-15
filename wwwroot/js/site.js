// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiKey = '333c507e13a6708b1caa02ed821254c7';
let movies = [];
let currentSlideIndex = 0;
const slidesToShow = 6; // Number of movie cards visible per slide

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
        movies = data.results.slice(0, 12);  // Limit to top 12 movies/TV shows
        displayTrendingMovies(currentSlideIndex);  // Limit to top 12
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
    const start = slideIndex * 6;  // Show 6 movies per slide
    const end = start + 6;
    const currentMovies = movies.slice(start, end);

    currentMovies.forEach((movie, index) => {
        const movieCard = document.createElement('div');
        movieCard.className = 'movie-card';
        movieCard.innerHTML = `
            <div class="movie-number">${start + index + 1}</div>
            <img src="https://image.tmdb.org/t/p/w500${movie.poster_path}" alt="${movie.title || movie.name}">
        `;
        fragment.appendChild(movieCard);  // Append the movie card to the fragment
    });

    moviesGrid.appendChild(fragment);  // Append the fragment to the #moviesGrid
}

// Function to go to the next slide
function nextSlide() {
    if ((currentSlideIndex + 1) * 6 < movies.length) {
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


// Initial call to populate content when the page loads
handleRegionChange();

