"use strict";


function getCultureLanguage() {
    let match = document.cookie.match(/\.AspNetCore\.Culture=([^;]+)/);
    if (!match) return null;

    let cultureValue = decodeURIComponent(match[1]);
    let langMatch = cultureValue.match(/c=([a-zA-Z-]+)/);

    return langMatch ? langMatch[1] : null;
}

function getCurrentLanguage() {
    let language = getCultureLanguage();
    console.log("Mevcut Dil:", language);

    if (language && language.startsWith('tr')) {
        return 'tr';
    } else if (language && language.startsWith('en')) {
        return 'en';
    } else {
        return 'en'; // Varsayılan dil ayarı
    }
}


var language = getCurrentLanguage();
var localizedTexts = {};

function updateLanguageTexts() {
    language = getCurrentLanguage();

    if (language === "tr") {
        localizedTexts = {
            formSubmittedText: "Form başarıyla gönderildi!",
            okButtonText: "Tamam",
            unsavedChangesTitle: "Kaydedilmemiş değişiklikler var!",
            unsavedChangesText: "Değişiklikleriniz kaybolacak, devam etmek istiyor musunuz?",
            confirmButtonText: "Evet, devam et",
            cancelButtonText: "İptal",
            nameRequired: "İsim alanı zorunludur",
            technicalUnitRequired: "Teknik birimler alanı zorunludur",
            isActiveRequired: "Eğitim aktiflik alanı zorunludur"
        };
    } else if (language === "en") {
        localizedTexts = {
            formSubmittedText: "Form submitted successfully!",
            okButtonText: "OK",
            unsavedChangesTitle: "Unsaved changes!",
            unsavedChangesText: "Your changes will be lost. Do you want to proceed?",
            confirmButtonText: "Yes, continue",
            cancelButtonText: "Cancel",
            nameRequired: "Name is required",
            technicalUnitRequired: "Technical unit is required",
            isActiveRequired: "Activity status is required"
        };
    }

    console.log("Localized Texts:", localizedTexts);
}

// Sayfa yüklendiğinde dil güncellemesini yap
document.addEventListener('DOMContentLoaded', function () {
    updateLanguageTexts();
});


var KTUsersAddUser = function () {
    // Shared variables
    const element = document.getElementById('createProductModal');
    const form = element.querySelector('#kt_modal_create_product_form');
    const modal = new bootstrap.Modal(element);
    var validator;

    // Init add schedule modal
    var initAddUser = () => {
        element.addEventListener('show.bs.modal', function () {
            form.reset();
            removeSubjectIds();
        });


        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'Name': {
                        validators: {
                            notEmpty: {
                                message: localizedTexts.nameRequired
                            }
                        }
                    },
                    'TechnicalUnitId': {
                        validators: {
                            notEmpty: {
                                message: localizedTexts.technicalUnitRequired
                            }
                        }
                    },
                    'IsActive': {
                        validators: {
                            notEmpty: {
                                message: localizedTexts.isActiveRequired
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );

        element.addEventListener('hide.bs.modal', function () {
            if (validator) {
                validator.resetForm();
            }
        });

        // Submit button handler
        const submitButton = element.querySelector('[data-kt-users-modal-action="submit"]');
        submitButton.addEventListener('click', e => {
            e.preventDefault();

            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
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
                                }
                            });

                            form.submit();
                        }, 2000);
                    }
                });
            }
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
                cancelButtonColor: '#3085d6',
                confirmButtonColor: '#d33',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset();
                    modal.hide();
                    removeSubjectIds();
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
                cancelButtonColor: '#3085d6',
                confirmButtonColor: '#d33',
                confirmButtonText: localizedTexts.confirmButtonText,
                cancelButtonText: localizedTexts.cancelButtonText,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-active-light"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset();
                    modal.hide();
                    removeSubjectIds();
                }
            });
        });

        function removeSubjectIds() {
            const subjectIdsInput = form.querySelector('[name="SubjectIds"]');
            $(subjectIdsInput).val(null).trigger('change');
        }
    }
    return {
        init: function () {
            initAddUser();
        }
    };
}();

KTUtil.onDOMContentLoaded(function () {
    KTUsersAddUser.init();
});
