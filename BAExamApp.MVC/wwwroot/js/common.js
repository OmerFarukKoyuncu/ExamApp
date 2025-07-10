function showDeleteModal(deleteUrl, fullName, redirectUrl) {
    Swal.fire({
        title: `${fullName} ${localizedTexts.confirmTitle}`,
        text: localizedTexts.confirmText,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: localizedTexts.confirmButtonText2,
        cancelButtonText: localizedTexts.cancelButtonText2
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "GET",
                url: deleteUrl,
                success: function (result) {
                    if (result.isSuccess) {
                        // Başarılı bir işlem sonrası yönlendirme
                        setTimeout(() => location.href = redirectUrl, 2000);
                    }
                },
                error: function (err) {
                    Swal.fire({
                        title: localizedTexts.formSubmittedText,
                        text: err.responseText || 'Bir hata oluştu.',
                        icon: 'error',
                        confirmButtonText: localizedTexts.okButtonText
                    });
                }
            });
        }
    });
}

// Sayfa yüklemesinden sonra ".deleteAction" tıklama olayını dinliyoruz
$(document).ready(function () {
    $(".deleteAction").click(function () {
        const id = $(this).attr("data-id");
        const firstName = document.getElementById("firstName").innerText;
        const lastName = document.getElementById("lastName").innerText;
        const fullName = `${firstName} ${lastName}`;

        // Silme modalını göster
        const deleteUrl = `/Delete?id=${id}`;
        const redirectUrl = "/Admin/Trainer";

        showDeleteModal(deleteUrl, fullName, redirectUrl);
    });
});

