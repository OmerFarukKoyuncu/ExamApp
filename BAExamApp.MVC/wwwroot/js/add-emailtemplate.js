document.addEventListener('DOMContentLoaded', function () {
    const cards = document.querySelectorAll('.dto-card');

    cards.forEach(card => {
        card.addEventListener('mouseover', function () {
            this.style.cursor = 'pointer';
        });

        card.addEventListener('mouseout', function () {
            this.style.cursor = 'default';
        });
    });
});