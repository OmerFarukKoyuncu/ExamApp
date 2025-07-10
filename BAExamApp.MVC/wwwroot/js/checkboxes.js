document.addEventListener("DOMContentLoaded", function () {
    // Get all checkboxes
    const checkboxes = document.querySelectorAll('.custom-checkbox');

    // Add event listener to each checkbox
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function (e) {
            if (this.disabled) return;

            // Create ripple effect
            const label = this.nextElementSibling;
            const ripple = document.createElement('span');
            ripple.classList.add('ripple');

            // Add the ripple to the label
            label.appendChild(ripple);

            // Remove the ripple after animation completes
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
});