document.addEventListener('DOMContentLoaded', function () {
    // Handle pagination clicks with AJAX
    document.querySelectorAll('.pagination-container .page-link').forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            var url = link.getAttribute('href');

            fetch(url)
                .then(function (response) {
                    return response.text();
                })
                .then(function (result) {
                    // Parse the result into a new document
                    var parser = new DOMParser();
                    var newDoc = parser.parseFromString(result, 'text/html');

                    // Update the table content
                    var newTableBody = newDoc.querySelector('.email-templates-table tbody');
                    var currentTableBody = document.querySelector('.email-templates-table tbody');
                    if (newTableBody && currentTableBody) {
                        currentTableBody.innerHTML = newTableBody.innerHTML;
                    }

                    // Update the pagination
                    var newPagination = newDoc.querySelector('.pagination');
                    var currentPagination = document.querySelector('.pagination');
                    if (newPagination && currentPagination) {
                        currentPagination.innerHTML = newPagination.innerHTML;
                    }

                    // Update URL without refreshing the page
                    history.pushState({}, '', url);
                })
                .catch(function () {
                    alert('An error occurred while loading the data.');
                });
        });
    });
});