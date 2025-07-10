$(document).ready(function () {
    tinymce.init({
        selector: '#template-input-update',
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'fullscreen', 'code',
            'insertdatetime', 'media', 'table', 'help', 'wordcount'
        ],
        toolbar: 'undo redo | styles | bold italic underline strikethrough | ' +
            'alignleft aligncenter alignright alignjustify | ' +
            'bullist numlist outdent indent | link image | ' +
            'forecolor backcolor | formatselect | ' +
            'removeformat | help',
        menubar: 'file edit view insert format tools table help',
        content_style: 'body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Arial, sans-serif; font-size: 16px; }',
        height: 400,
        z_index: 1200,
        setup: function (editor) {
            editor.on('init', function () {
                $('#updateEmailTemplate').on('show.bs.modal', function (event) {
                    let button = $(event.relatedTarget);
                    let templateId = button.data('template-id');
                    let templateSubject = button.data('template-subject');
                    let templateModelName = button.data('template-modelname');
                    let templateBody = button.data('template-template');

                    let modal = $(this);
                    modal.find('input[name="Id"]').val(templateId);
                    modal.find('input[name="Subject"]').val(templateSubject);
                    modal.find('input[name="ModelName"]').val(templateModelName);
                    editor.setContent(templateBody);
                });
            });
        }
    });
    tinymce.init({
        selector: '#template-input-details',
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'fullscreen', 'code',
            'insertdatetime', 'media', 'table', 'help', 'wordcount'
        ],
        toolbar: [
            'code',
        ],
        menubar: false,
        content_style: 'body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Arial, sans-serif; font-size: 16px; }',
        height: 400,
        z_index: 1200,
        readonly: true, // Başlangıçta readonly modu aktif
        setup: function (editor) {
            editor.on('init', function () {
                $('button[aria-label="Source code"]').parent('.tox-toolbar__group').on('click', function () {
                    editor.setMode('design'); //Source Code tiklaninca calisabilsin diye
                    const observer = new MutationObserver(() => {
                        const $dialog = $('[role="dialog"]');
                        if ($dialog.length && $dialog.is(':visible')) {
                            editor.setMode('readonly'); //Source Code acildiginda tekrar readonly
                            observer.disconnect();
                        }
                    });
                    observer.observe($('body')[0], {
                        childList: true,
                        subtree: true
                    });
                });
            });
        }
    });
    $('.dto-card').on('click', function () {
        const $card = $(this);
        const isUpdateShown = $('#updateEmailTemplate').hasClass('show');
        if (isUpdateShown) {
            // console.log(isUpdateShown);
            const cardNameUpdate = $card.find('#cardNameUpdate').text().trim().replace(/\s+/g, ' ');
            // console.log(cardNameUpdate);
            // console.log($('#nameInputUpdate').val());
            $('#nameInputUpdate').val(cardNameUpdate);
            // console.log($('#nameInputUpdate').val());
        }
        const $cardColumn = $card.closest('.col-md-4');
        const $container = $card.closest('.row');
        // Visual feedback
        $('.dto-card').removeClass('selected');
        $card.addClass('selected');
        // Get positions for animation
        const startPos = $cardColumn.offset();
        const firstPos = $container.find('.col-md-4:first').offset();
        // Calculate distance
        const moveX = firstPos.left - startPos.left;
        const moveY = firstPos.top - startPos.top;

        // Animate the card
        $cardColumn
            .css({
                'transition': 'transform 0.5s ease',
                'transform': `translate(${moveX}px, ${moveY}px)`
            })
            .promise()
            .done(function () {
                setTimeout(function () {
                    $cardColumn
                        .css({
                            'transition': 'none',
                            'transform': 'none'
                        })
                        .prependTo($container); // Now this will only prepend to the specific parent row
                }, 500);
            });
    });
});