// script.js
document.addEventListener('DOMContentLoaded', () => {
    const cards = {
        authors: document.getElementById('authors-card'),
        books: document.getElementById('books-card'),
        publishers: document.getElementById('publishers-card'),
        categories: document.getElementById('categories-card')
    };

/*    // Handle card clicks
    Object.keys(cards).forEach(key => {
        cards[key].addEventListener('click', () => {
            // In a real app, this would navigate to the CRUD page
            alert(`Navigating to ${key.replace('-', ' ')} management page`);

            // You would typically use:
            // window.location.href = `${key}.html`;
        });
    });*/

    // Add keyboard navigation
    document.addEventListener('keydown', (e) => {
        const keyActions = {
            '1': 'authors',
            '2': 'books',
            '3': 'publishers',
            '4': 'categories'
        };

        if (keyActions[e.key]) {
            cards[keyActions[e.key]].click();
        }
    });
});