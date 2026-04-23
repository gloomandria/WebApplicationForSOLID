/**
 * Gestion AJAX de la section Matières.
 * Dépend de : jquery, bootstrap 5, ajax-helpers.js, Quill
 */
(function ($) {
    'use strict';

    var ctrl = document.getElementById('app-data').dataset.controller;
    var H    = window.AjaxHelpers;
    var quillInstance = null;

    function destroyQuill() { quillInstance = null; }

    function initQuill(initialHtml, modalBodyId) {
        destroyQuill();
        var container = document.getElementById('quill-description');
        if (!container) return;
        quillInstance = new Quill('#quill-description', {
            theme: 'snow',
            placeholder: 'Description de la matière…',
            modules: {
                toolbar: [
                    ['bold', 'italic', 'underline'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    ['clean']
                ]
            }
        });
        // Quill v2 : root.innerHTML au lieu de clipboard.dangerouslyPasteHTML
        if (initialHtml) quillInstance.root.innerHTML = initialHtml;
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        quillInstance.on('text-change', function () {
            var h = scope.querySelector('input[name="Matiere.Description"]');
            if (h) h.value = quillInstance.root.innerHTML;
        });
    }

    function syncQuill(modalBodyId) {
        if (!quillInstance) return;
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        var h = scope.querySelector('input[name="Matiere.Description"]');
        if (h) h.value = quillInstance.root.innerHTML;
    }

    function wireFormWithQuill(formId, alertId, actionUrl, modalId, modalBodyId) {
        var $form = $('#' + formId);
        $form[0].onsubmit = function (e) {
            e.preventDefault();
            syncQuill(modalBodyId);
            $('#' + alertId).text('');
            var fd = new FormData($form[0]);
            $.ajax({
                url: actionUrl,
                method: 'POST',
                data: fd,
                processData: false,
                contentType: false,
                headers: { 'RequestVerificationToken': H.token() }
            }).done(function (data) {
                if (!data.success) { $('#' + alertId).text(data.message); return; }
                bootstrap.Modal.getInstance(document.getElementById(modalId)).hide();
                H.showAlert(data.message, true);
                refreshTable();
            }).fail(function () {
                $('#' + alertId).text('Erreur serveur inattendue.');
            });
        };
    }

    function refreshTable() {
        return $.get('/' + ctrl + '/Table')
            .done(function (html) {
                $('#table-body').html(html);
                bindRowButtons();
            });
    }

    $('#btn-open-create').on('click', function () {
        $('#modal-create-alert').text('');
        H.loadModal('modal-create', 'modal-create-body', '/' + ctrl + '/FormCreate')
            .done(function () {
                initQuill('', 'modal-create-body');
                wireFormWithQuill('form-create', 'modal-create-alert', '/' + ctrl + '/CreateAjax', 'modal-create', 'modal-create-body');
            });
    });
    $('#modal-create').on('hide.bs.modal', destroyQuill);

    function bindRowButtons() {
        $('.btn-edit').off('click').on('click', function () {
            $('#modal-edit-alert').text('');
            H.loadModal('modal-edit', 'modal-edit-body', '/' + ctrl + '/FormEdit?id=' + $(this).data('id'))
                .done(function () {
                    var initial = document.querySelector('#modal-edit-body input[name="Matiere.Description"]');
                    initQuill(initial ? initial.value : '', 'modal-edit-body');
                    wireFormWithQuill('form-edit', 'modal-edit-alert', '/' + ctrl + '/EditAjax', 'modal-edit', 'modal-edit-body');
                });
        });

        $('.btn-delete').off('click').on('click', function () {
            $('#modal-delete-alert').text('');
            H.loadModal('modal-delete', 'modal-delete-body', '/' + ctrl + '/FormDelete?id=' + $(this).data('id'))
                .done(function () {
                    H.wireForm('form-delete', 'modal-delete-alert', '/' + ctrl + '/DeleteAjax', 'modal-delete', function (msg) {
                        H.showAlert(msg, true);
                        refreshTable();
                    });
                });
        });
    }
    $('#modal-edit').on('hide.bs.modal', destroyQuill);

    bindRowButtons();

}(jQuery));
