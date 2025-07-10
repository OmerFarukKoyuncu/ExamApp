"use strict";
// Class definition
var KTUsersAddUser = function () {
    // Shared variables
    const element = document.getElementById('kt_modal_add_class');
    const form = element.querySelector('#kt_modal_add_class_form');
    const modal = new bootstrap.Modal(element);
   
    // Init add schedule modal
    var initAddUser = () => {
       
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        //var validator = FormValidation.formValidation(
        //    form,
        //    {
        //        fields: {
        //            'Name': {
        //                validators: {
        //                    notEmpty: {
        //                        message: '<p>Sınıf Adı alanı zorunludur</p>'
        //                    }
        //                }
        //            },
        //            'OpeningDate': {
        //                validators: {
        //                    notEmpty: {
        //                        message: 'Açılış tarihi alanı zorunludur'
        //                    },
        //                    callback: {
        //                        message: 'Açılış tarihi kapanış tarihinden önce olmalıdır', // Açılış tarihinin kapanış tarihinden önce olma mesajı
        //                        callback: function (input) {
        //                            const openingDate = new Date(input.value);
        //                            const closedDateInput = form.querySelector('[name="ClosedDate"]');
        //                            const closedDate = new Date(closedDateInput.value);

        //                            // Eğer açılış tarihi boşsa veya kapanış tarihi geçerli değilse
        //                            if (input.value === '' || isNaN(closedDate.getTime())) {
        //                                return true;
        //                            }
        //                            // Saat değerlerini sıfırlayarak sadece tarih karşılaştırması yapmak
        //                            openingDate.setHours(0, 0, 0, 0);
        //                            closedDate.setHours(0, 0, 0, 0);

        //                            if (openingDate < closedDate) { // Açılış tarihi kapanış tarihinden önce mi?
        //                                return true;
        //                            } else if (openingDate > closedDate) { // Açılış tarihi kapanış tarihinden sonra mı?
        //                                return {
        //                                    valid: false,
        //                                    message: 'Açılış tarihi kapanış tarihinden önce olmalıdır' // Hata mesajı: Açılış tarihi kapanış tarihinden önce olmalıdır
        //                                };
        //                            } else {
        //                                return {
        //                                    valid: false,
        //                                    message: 'Açılış tarihi kapanış tarihi ile eşit olmamalıdır' // Hata mesajı: Açılış tarihi kapanış tarihi ile eşit olmamalıdır
        //                                };
        //                            }
        //                        }
        //                    }
        //                }
        //            },
        //            'ClosedDate': {
        //                validators: {
        //                    notEmpty: {
        //                        message: 'Kapanış tarihi alanı zorunludur'
        //                    },
        //                    callback: {
        //                        message: 'Kapanış tarihi açılış tarihinden sonra olmalıdır', // Kapanış tarihinin açılış tarihinden sonra olma mesajı
        //                        callback: function (input) {
        //                            const closedDate = new Date(input.value);
        //                            const openingDateInput = form.querySelector('[name="OpeningDate"]');
        //                            const openingDate = new Date(openingDateInput.value);    

        //                            // Eğer kapanış tarihi boşsa veya açılış tarihi geçerli değilse
        //                            if (input.value === '' || isNaN(openingDate.getTime())) {
        //                                return true;
        //                            }

        //                            // Saat değerlerini sıfırlayarak sadece tarih karşılaştırması yapmak
        //                            closedDate.setHours(0, 0, 0, 0);
        //                            openingDate.setHours(0, 0, 0, 0);

        //                            if (closedDate > openingDate) { // Kapanış tarihi açılış tarihinden sonra mı?
        //                                return true;
        //                            } else if (closedDate < openingDate) { // Kapanış tarihi açılış tarihinden önce mi?
        //                                return {
        //                                    valid: false,
        //                                    message: 'Kapanış tarihi açılış tarihinden sonra olmalıdır' // Hata mesajı: Kapanış tarihi açılış tarihinden sonra olmalıdır
        //                                };
        //                            } else {
        //                                return {
        //                                    valid: false,
        //                                    message: 'Kapanış tarihi açılış tarihi ile eşit olmamalıdır' // Hata mesajı: Kapanış tarihi açılış tarihi ile eşit olmamalıdır 
        //                                };
        //                            }
        //                        }
        //                    }
        //                }
        //            },
        //            'BranchId': {
        //                validators: {
        //                    notEmpty: {
        //                        message: 'Şube alanı zorunludur'
        //                    }
        //                }
        //            },
        //            'GroupTypeId': {
        //                validators: {
        //                    notEmpty: {
        //                        message: 'Eğitim tipi alanı zorunludur'
        //                    }
        //                }
        //            },
        //            'ProductIds': {
        //                validators: {
        //                    notEmpty: {
        //                        message: 'Ürün alanı zorunludur'
        //                    }
        //                }
        //            }
        //        },
        //        plugins: {
        //            trigger: new FormValidation.plugins.Trigger(),
        //            bootstrap: new FormValidation.plugins.Bootstrap5({
        //                rowSelector: '.fv-row',
        //                eleInvalidClass: '',
        //                eleValidClass: ''
        //            })
        //        }
        //    }
        //);
        // Submit button handler
        //const submitButton = element.querySelector('[data-kt-users-modal-action="submit"]');
        //submitButton.addEventListener('click', e => {
        //    e.preventDefault();
        

        //    // Validate form before submit
        //    if (validator) {
        //        validator.validate().then(function (status) {
        //            console.log('validated!');

        //            if (status == 'Valid') {
        //                // Show loading indication
        //                submitButton.setAttribute('data-kt-indicator', 'on');

        //                // Disable button to avoid multiple click 
        //                submitButton.disabled = true;

        //                // Simulate form submission. For more info check the plugin's official documentation: https://sweetalert2.github.io/
        //                setTimeout(function () {
        //                    // Remove loading indication
        //                    submitButton.removeAttribute('data-kt-indicator');

        //                    // Enable button
        //                    submitButton.disabled = false;

        //                    // Show popup confirmation 
        //                    Swal.fire({
        //                        text: localizedTexts.formSubmittedText,
        //                        icon: "success",
        //                        buttonsStyling: false,
        //                        confirmButtonText: localizedTexts.okButtonText,
        //                        customClass: {
        //                            confirmButton: "btn btn-primary"
        //                        }
        //                    }).then(function (result) {
        //                        if (result.isConfirmed) {
        //                            modal.hide();
        //                        }
        //                    });

        //                    form.submit(); // Submit form
        //                }, 2000);
        //            } 
        //        });
        //    }
        //});
        // Cancel button handler
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
                cancelButtonText: localizedTexts.cancelButtonText
            }).then((result) => {
                if (result.isConfirmed) {
                    // Event handler'ı geçici olarak kaldır
                    $('#kt_modal_add_class').off('hide.bs.modal');
                    // Modalı kapat
                    $('#kt_modal_add_class').modal('hide');
                    // Modal tamamen kapandıktan sonra event handler'ı tekrar ekle
                    $('#kt_modal_add_class').on('hidden.bs.modal', function () {
                        addModalCloseConfirmation2();
                        $(this).find('form').trigger('reset');
                    });
                }
            });
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
                cancelButtonText: localizedTexts.cancelButtonText
            }).then((result) => {
                if (result.isConfirmed) {
                    // Event handler'ı geçici olarak kaldır
                    $('#kt_modal_add_class').off('hide.bs.modal');
                    // Modalı kapat
                    $('#kt_modal_add_class').modal('hide');
                    // Modal tamamen kapandıktan sonra event handler'ı tekrar ekle
                    $('#kt_modal_add_class').on('hidden.bs.modal', function () {
                        addModalCloseConfirmation2();
                        $(this).find('form').trigger('reset');
                    });
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
$('#kt_modal_add_class').on('hidden.bs.modal', function () {
    $(this).find('form').trigger('reset');
});
function addModalCloseConfirmation2() {
    $('#kt_modal_add_class').on('hide.bs.modal', function (e) {
        e.preventDefault();
        showModalCloseConfirmation2();
    });
}

// İlk yüklemede event handler'ı ekle
addModalCloseConfirmation2();


// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTUsersAddUser.init();
});
