"use strict";

var KTUsersUpdateDetails = function () {
    const element = document.getElementById('kt_modal_update_classroom');
    const form = document.getElementById('kt_modal_update_classroom_form');
    const modal = new bootstrap.Modal(element);

    var initUpdateDetails = () => {
        $('#ProductsIds').select2(); 

     
        const closeButton = element.querySelector('[data-kt-users-modal-action="closes"]');
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
                    $('#kt_modal_update_classroom').off('hide.bs.modal');
                    $('#kt_modal_update_classroom').modal('hide');
                    $('#kt_modal_update_classroom').on('hidden.bs.modal', function () {
                        addModalCloseConfirmation();
                        $(this).find('form').trigger('reset');
                    });
                }
            });
        });

        
        const cancelButton = element.querySelector('[data-kt-users-modal-action="cancels"]');
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
                    $('#kt_modal_update_classroom').off('hide.bs.modal');
                    $('#kt_modal_update_classroom').modal('hide');
                    $('#kt_modal_update_classroom').on('hidden.bs.modal', function () {
                        addModalCloseConfirmation();
                        $(this).find('form').trigger('reset');
                    });
                }
            });
        });

        // Form submit -> Do NOT use e.preventDefault() so browser validation works!
        const submitButton = element.querySelector('[data-kt-users-modal-action="submits"]');
        submitButton.addEventListener('click', function (e) {
            // Do nothing. Let the form submit naturally so validation works.
        });
    }

    return {
        init: function () {
            initUpdateDetails();
        }
    };
}();

async function loadClassroomData(id) {
    const form = document.getElementById('kt_modal_update_classroom_form');

    const classroom = await getClassroom(id);
    console.log('Classroom Data:', classroom);

    form.elements["Id"].value = classroom.id;
    form.elements["Name"].value = classroom.name;
    form.elements["OpeningDate"].value = classroom.openingDate.split("T")[0];
    form.elements["ClosedDate"].value = classroom.closedDate.split("T")[0];
    form.elements["BranchId"].value = classroom.branchId;
    form.elements["GroupTypeId"].value = classroom.groupTypeId;

    const productSelect = form.elements["ProductIds"];
    for (let option of productSelect.options) {
        option.selected = false;
    }

    for (let productId of classroom.productIds) {
        for (let option of productSelect.options) {
            if (option.value == productId) {
                option.selected = true;
                break;
            }
        }
    }
    const event = new Event('change');
    productSelect.dispatchEvent(event);
}

async function getClassroom(classroomId) {
    return $.ajax({
        url: '/Admin/Classroom/GetClassroom',
        data: { classroomId: classroomId }
    });
}

function addModalCloseConfirmation() {
    $('#kt_modal_update_classroom').on('hide.bs.modal', function (e) {
        e.preventDefault();
        showModalCloseConfirmation();
    });
}

addModalCloseConfirmation();

$('#kt_modal_update_classroom').on('hidden.bs.modal', function () {
    $(this).find('form').trigger('reset');
});

KTUtil.onDOMContentLoaded(function () {
    KTUsersUpdateDetails.init();
});
