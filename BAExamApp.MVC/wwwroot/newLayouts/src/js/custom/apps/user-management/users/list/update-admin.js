"use strict";

let admin = null;

// **Cookie'den .AspNetCore.Culture değerini doğru parse eden fonksiyon**
function getCultureLanguage() {
    let match = document.cookie.match(/\.AspNetCore\.Culture=([^;]+)/);
    if (!match) return null; // Cookie bulunamazsa null döndür

    let cultureValue = decodeURIComponent(match[1]); // URL Encoded içeriği çöz
    let langMatch = cultureValue.match(/c=([a-zA-Z-]+)/); // `c=tr` veya `c=en` değerini al

    return langMatch ? langMatch[1] : null; // Eğer dil bulunduysa döndür, yoksa null
}

// **Sayfa dilini belirleme (Cookie veya document.documentElement.lang üzerinden)**
function getCurrentLanguage() {
    return getCultureLanguage() || document.documentElement.lang || null;
}

// **Dil metinlerini saklayan değişken**
var language = getCurrentLanguage();
var localizedTexts = {};

// **Dil metinlerini güncelleyen fonksiyon**
function updateLanguageTexts() {
    language = getCurrentLanguage(); // Güncel dili oku

    if (!language) {
        console.warn("Dil belirlenemedi. Lütfen sistemde bir dil ayarı yapın.");
        return; // Eğer dil belirlenemezse güncelleme yapma
    }

    if (language.startsWith("tr")) {
        localizedTexts = {
            formSubmittedText: "Form başarıyla güncellendi!",
            okButtonText: "Tamam",
            unsavedChangesTitle: "Kaydedilmemiş değişiklikler var!",
            unsavedChangesText: "Değişiklikleriniz kaybolacak, devam etmek istiyor musunuz?",
            confirmButtonText: "Evet, devam et",
            cancelButtonText: "İptal",
            firstNameRequired: "İsim alanı zorunludur",
            lastNameRequired: "Soyisim alanı zorunludur",
            emailRequired: "Mail Adresi alanı zorunludur",
            emailInvalid: "Lütfen geçerli bir mail adresi giriniz",
            genderRequired: "Cinsiyet alanı zorunludur",
            imageValidation: "Lütfen maksimum 512 kb boyutunda ve uygun dosya biçiminde bir fotoğraf seçiniz (png, jpg, jpeg)"
        };
    } else if (language.startsWith("en")) {
        localizedTexts = {
            formSubmittedText: "Form updated successfully!",
            okButtonText: "OK",
            unsavedChangesTitle: "Unsaved changes!",
            unsavedChangesText: "Your changes will be lost. Do you want to proceed?",
            confirmButtonText: "Yes, continue",
            cancelButtonText: "Cancel",
            firstNameRequired: "First name is required",
            lastNameRequired: "Last name is required",
            emailRequired: "Email is required",
            emailInvalid: "Please enter a valid email address",
            genderRequired: "Gender is required",
            imageValidation: "Please select an image with a maximum size of 512 kb (png, jpg, jpeg)"
        };
    } else {
        console.warn("Tanımlanmamış bir dil algılandı:", language);
    }

    console.log("Updated Language:", language);
}

// **Class definition**
var KTUsersUpdateDetails = function () {
    const element = document.getElementById('kt_modal_update_user');
    const form = document.getElementById('kt_modal_update_user_form');
    const modal = new bootstrap.Modal(element);
    let validator;

    var initUpdateDetails = () => {
        updateLanguageTexts();

        if (validator) {
            validator.destroy(); // Eğer validator zaten varsa tekrar oluşturmamak için temizle
        }

        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'NewImage': {
                        validators: {
                            file: {
                                extension: 'png,jpg,jpeg',
                                type: 'image/png,image/jpg,image/jpeg',
                                maxSize: 524288,
                                message: localizedTexts.imageValidation
                            }
                        }
                    },
                    'FirstName': { validators: { notEmpty: { message: localizedTexts.firstNameRequired } } },
                    'LastName': { validators: { notEmpty: { message: localizedTexts.lastNameRequired } } },
                    'Email': {
                        validators: {
                            notEmpty: { message: localizedTexts.emailRequired },
                            emailAddress: { message: localizedTexts.emailInvalid }
                        }
                    },
                    'Gender': { validators: { notEmpty: { message: localizedTexts.genderRequired } } }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: '.fv-row' })
                }
            }
        );

        // **Submit button handler**
        const submitButton = element.querySelector('[data-kt-users-modal-action="submit"]');
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
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
                            customClass: { confirmButton: "btn btn-primary" }
                        }).then(() => modal.hide());

                        form.submit();
                    }, 2000);
                }
            });
        });

        // **Cancel & Close button handler**
        const cancelButton = element.querySelector('[data-kt-users-modal-action="cancel"]');
        const closeButton = element.querySelector('[data-kt-users-modal-action="close"]');

        [cancelButton, closeButton].forEach(button => {
            button.addEventListener('click', e => {
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
                        validator.resetForm();
                        modal.hide();
                    }
                });
            });
        });
    }

    return { init: function () { initUpdateDetails(); } };
}();

document.addEventListener("DOMContentLoaded", function () {
    KTUsersUpdateDetails.init();
});

// **DİL DEĞİŞTİĞİNDE VALIDATOR'I YENİDEN BAŞLAT**
const observer = new MutationObserver(() => {
    updateLanguageTexts();
    KTUsersUpdateDetails.init();
});
observer.observe(document.documentElement, { attributes: true, attributeFilter: ['lang'] });
