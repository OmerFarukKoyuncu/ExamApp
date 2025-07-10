"use strict";

class KTModalStudentAdd {
    constructor() {
        this.element = document.querySelector('#kt_modal_add_user');
        this.form = this.element.querySelector('#kt_modal_add_user_form');
        this.modal = new bootstrap.Modal(this.element);
        this.submitButton = this.element.querySelector('[data-kt-users-modal-action="submit"]');
        this.cancelButton = this.element.querySelector('[data-kt-users-modal-action="cancel"]');
        this.closeButton = this.element.querySelector('[data-kt-users-modal-action="close"]');

        this.initializeValidation();
        this.initializeEventListeners();
        this.initializeSelect2();
    }

    initializeValidation() {
        this.validator = FormValidation.formValidation(this.form, {
            fields: {
                'FirstName': {
                    validators: {
                        notEmpty: {
                            message: localizedTexts.StudentNameRequiredText
                        }
                    }
                },
                'LastName': {
                    validators: {
                        notEmpty: {
                            message: localizedTexts.StudentSurnameRequiredText
                        }
                    }
                },
                'Email': {
                    validators: {
                        notEmpty: {
                            message: localizedTexts.StudentEmailRequiredText
                        },
                        emailAddress: {
                            message: localizedTexts.StudentEmailInvalidText
                        }
                    }
                },
                'ClassroomIds': {
                    validators: {
                        notEmpty: {
                            message: localizedTexts.ClassroomRequiredText
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
        });
    }

    async handleSubmit(e) {
        e.preventDefault();

        try {
            const status = await this.validator.validate();

            if (status === 'Valid') {
                this.submitButton.setAttribute('data-kt-indicator', 'on');
                this.submitButton.disabled = true;

                // Simulating form submission delay
                await new Promise(resolve => setTimeout(resolve, 2000));

                this.submitButton.removeAttribute('data-kt-indicator');
                this.submitButton.disabled = false;

                const result = await Swal.fire({
                    text: localizedTexts.formSubmittedText,
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: localizedTexts.okButtonText,
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });

                if (result.isConfirmed) {
                    this.modal.hide();
                    this.form.submit();
                }
            }
        } catch (error) {
            console.error('Form submission error:', error);
            // Handle error appropriately
            Swal.fire({
                text: 'Form gönderilirken bir hata oluştu!',
                icon: 'error',
                buttonsStyling: false,
                confirmButtonText: 'Tamam',
                customClass: {
                    confirmButton: "btn btn-primary"
                }
            });
        }
    }

    async handleModalClose(e) {
        e.preventDefault();

        const result = await Swal.fire({
            title: localizedTexts.unsavedChangesTitle,
            text: localizedTexts.unsavedChangesText,
            icon: 'warning',
            showCancelButton: true,
            cancelButtonColor: '#3085d6',
            confirmButtonColor: '#d33',
            confirmButtonText: localizedTexts.confirmButtonText,
            cancelButtonText: localizedTexts.cancelButtonText
        });

        if (result.value) {
            this.validator.resetForm();
            $(this.element).off('hide.bs.modal');
            this.modal.hide();

            $(this.element).one('hidden.bs.modal', () => {
                this.addModalCloseConfirmation();
                this.form.reset();
            });
        }
    }

    initializeEventListeners() {
        this.submitButton.addEventListener('click', this.handleSubmit.bind(this));
        this.cancelButton.addEventListener('click', this.handleModalClose.bind(this));
        this.closeButton.addEventListener('click', this.handleModalClose.bind(this));

        // Form input'ları değiştiğinde validation'ı tetikle
        this.form.querySelectorAll('input, select').forEach(element => {
            element.addEventListener('change', () => {
                this.validator.revalidateField(element.name);
            });
        });
    }

    initializeSelect2() {
        $('#classSelect').select2({
            dropdownParent: $(this.element),
            placeholder: "Select classrooms",
            allowClear: true
        }).on('change', () => {
            // Select2 değiştiğinde validation'ı tekrar çalıştır
            this.validator.revalidateField('ClassroomIds');
        });
    }

    addModalCloseConfirmation() {
        $(this.element).on('hide.bs.modal', async (e) => {
            // Form değişiklik yapılmışsa
            if (this.form.querySelector(':input').value) {
                e.preventDefault();
                await this.handleModalClose(e);
            }
        });
    }

    static init() {
        return new KTModalStudentAdd();
    }
}

// Initialize on DOM content loaded
document.addEventListener('DOMContentLoaded', () => {
    KTModalStudentAdd.init();
});