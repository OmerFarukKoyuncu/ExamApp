"use strict";

// Class definition
var KTUsersAddUser = function () {
    const element = document.getElementById('kt_modal_add_user');
    const form = document.getElementById('kt_modal_add_user_form');
    const modal = new bootstrap.Modal(element);

    var initAddUser = () => {
        const submitButton = element.querySelector('[data-kt-users-modal-action="submit"]');
        const closeButton = element.querySelector('[data-kt-users-modal-action="close"]');

        // jQuery validator'ı yeniden bağla
        $.validator.unobtrusive.parse(form);

        // Submit Butonu
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            // jQuery Validation aktif mi kontrol et
            const isValid = $(form).valid();
            if (!isValid) {
                // HTML5 hataları varsa göster
                form.reportValidity();
                return;
            }

            // Yükleniyor simgesi göster
            submitButton.setAttribute('data-kt-indicator', 'on');
            submitButton.disabled = true;

            setTimeout(function () {
                submitButton.removeAttribute('data-kt-indicator');
                submitButton.disabled = false;

                Swal.fire({
                    text: localizedTexts.formSubmittedText,
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: localizedTexts.okButtonText,
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        modal.hide();
                        form.requestSubmit(); // Doğal submit
                    }
                });
            }, 1000);
        });

        // Kapat Butonu
        closeButton.addEventListener('click', function (e) {
            e.preventDefault();

            Swal.fire({
                title: localizedTexts.unsavedChangesTitle,
                text: localizedTexts.unsavedChangesText,
                icon: 'warning',
                showCancelButton: true,
                cancelButtonColor: '#3085d6',
                confirmButtonColor: '#d33',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText
            }).then((result) => {
                if (result.isConfirmed) {
                    closeAllModals();
                    form.reset();
                    $(form).validate().resetForm();
                }
            });
        });
    };

    return {
        init: function () {
            initAddUser();
        }
    };
}();

// Sayfa yüklendiğinde başlat
KTUtil.onDOMContentLoaded(function () {
    KTUsersAddUser.init();
});

// Açık tüm modalları kapatır
function closeAllModals() {
    document.querySelectorAll('.modal.show').forEach((modalElement) => {
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        }
    });
}
