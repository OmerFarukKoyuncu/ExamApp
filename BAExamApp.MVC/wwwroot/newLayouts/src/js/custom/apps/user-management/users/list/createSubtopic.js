"use strict";
// Class definition

var KTUsersAddUser = function () {

    // Shared variables
    const element = document.getElementById('createSubtopicModal');
    const form = element.querySelector('#kt_modal_create_subtopic_form');
    const modal = new bootstrap.Modal(element);
   

    // Init add schedule modal
    var initAddUser = () => {
        element.addEventListener('show.bs.modal', function () {
            // Clear input fields when modal is shown
            form.reset();
        });

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
       
        element.addEventListener('hide.bs.modal', function () {
            //Hide error messages
            $('.text-danger').text('');
            form.reset();
           
        });

       
        // Cancel button handler
        const cancelButton = element.querySelector('[data-kt-users-modal-action="cancel"]');
        cancelButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                title: localizedTexts.unsavedChangesTitle,
                text: localizedTexts.unsavedChangesText,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();
                }
            });
        });

        // Close button handler
        const closeButton = element.querySelector('[data-kt-users-modal-action="close"]');
        closeButton.addEventListener('click', e => {
            e.preventDefault();

            Swal.fire({
                title: localizedTexts.unsavedChangesTitle,
                text: localizedTexts.unsavedChangesText,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form			
                    modal.hide();
                } 
            });
        });
    }
    return {
        // Public functions
        init: function () {
            initAddUser();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsersAddUser.init();
});
