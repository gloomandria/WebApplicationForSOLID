/**
 * Gestion AJAX de la section EmailTemplates.
 * Dépend de : jquery, bootstrap 5, ajax-helpers.js, Quill
 */
(function ($) {
    'use strict';

    var ctrl = 'EmailTemplates';
    var H    = window.AjaxHelpers;

    // ── Quill : instance courante (une seule modale ouverte à la fois) ─
    var quillInstance = null;

    function destroyQuill() {
        quillInstance = null;
    }

    function initQuill(containerId, hiddenInputName, initialHtml, modalBodyId) {
        destroyQuill();

        var container = document.getElementById(containerId);
        if (!container) return;

        container.style.height = '260px';

        quillInstance = new Quill('#' + containerId, {
            theme: 'snow',
            placeholder: 'Corps HTML du template…',
            modules: {
                toolbar: [
                    [{ header: [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline'],
                    [{ color: [] }, { background: [] }],
                    ['link'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    ['clean']
                ]
            }
        });

        // Quill v2 : pas de clipboard.dangerouslyPasteHTML — on écrit directement dans le DOM
        if (initialHtml) {
            quillInstance.root.innerHTML = initialHtml;
        }

        // Synchronise le hidden input à chaque changement (scopé au modalBody)
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        quillInstance.on('text-change', function () {
            var hiddenInput = scope.querySelector('input[name="' + hiddenInputName + '"]');
            if (hiddenInput) hiddenInput.value = quillInstance.root.innerHTML;
        });
    }

    // ── Synchronise Quill → hidden input avant soumission ─────────────
    function syncQuillBeforeSubmit(hiddenInputName, modalBodyId) {
        if (!quillInstance) return;
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        var hiddenInput = scope.querySelector('input[name="' + hiddenInputName + '"]');
        if (hiddenInput) hiddenInput.value = quillInstance.root.innerHTML;
    }

    // ── Refresh table ──────────────────────────────────────────────────
    function refreshTable() {
        return $.get('/' + ctrl + '/Table')
            .done(function (html) {
                $('#table-body').html(html);
                bindRowButtons();
            });
    }

    // ── Ouverture modale Créer ─────────────────────────────────────────
    $('#btn-open-create').on('click', function () {
        $('#modal-create-alert').text('');
        H.loadModal('modal-create', 'modal-create-body', '/' + ctrl + '/FormCreate')
            .done(function () {
                initQuill('quill-corps', 'Corps', '', 'modal-create-body');
                wireTemplateForm('form-create', 'modal-create-alert', '/' + ctrl + '/CreateAjax', 'modal-create', 'Corps', 'modal-create-body');
            });
    });

    $('#modal-create').on('hide.bs.modal', destroyQuill);

    // ── Ouverture modale Éditer ────────────────────────────────────────
    function bindRowButtons() {
        $('.btn-edit').off('click').on('click', function () {
            var id = $(this).data('id');
            $('#modal-edit-alert').text('');
            H.loadModal('modal-edit', 'modal-edit-body', '/' + ctrl + '/FormEdit?id=' + id)
                .done(function () {
                    var $hidden = $('#modal-edit-body input[name="Corps"]');
                    var initialHtml = $hidden.val() || '';
                    initQuill('quill-corps', 'Corps', initialHtml, 'modal-edit-body');
                    wireTemplateForm('form-edit', 'modal-edit-alert', '/' + ctrl + '/EditAjax', 'modal-edit', 'Corps', 'modal-edit-body');
                });
        });

        $('.btn-delete').off('click').on('click', function () {
            var id = $(this).data('id');
            var modal = new bootstrap.Modal(document.getElementById('modal-delete'));
            modal.show();

            $('#form-delete').off('submit').on('submit', function (e) {
                e.preventDefault();
                var token = $(this).find('input[name="__RequestVerificationToken"]').val();
                $.post('/' + ctrl + '/DeleteAjax', { id: id, __RequestVerificationToken: token })
                    .done(function (data) {
                        modal.hide();
                        if (data.success) {
                            H.showAlert(data.message, true);
                            refreshTable();
                        } else {
                            H.showAlert(data.message || 'Erreur', false);
                        }
                    })
                    .fail(function () {
                        modal.hide();
                        H.showAlert('Erreur serveur inattendue.', false);
                    });
            });
        });
    }

    $('#modal-edit').on('hide.bs.modal', destroyQuill);

    // ── Wiring formulaire avec sync Quill avant submit ─────────────────
    function wireTemplateForm(formId, alertId, actionUrl, modalId, hiddenInputName, modalBodyId) {
        var $form = $('#' + formId);
        $form[0].onsubmit = function (e) {
            e.preventDefault();
            syncQuillBeforeSubmit(hiddenInputName, modalBodyId);
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

    bindRowButtons();

})(jQuery);
