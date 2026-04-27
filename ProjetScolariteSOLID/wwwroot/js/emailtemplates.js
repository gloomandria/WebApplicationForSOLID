/**
 * Gestion AJAX de la section EmailTemplates — DataTables client-side + Quill.
 * Dépend de : jquery, bootstrap 5, datatables, ajax-helpers.js, Quill
 */
(function ($) {
    'use strict';

    var ctrl = 'EmailTemplates';
    var H    = window.AjaxHelpers;
    var dt;
    var quillInstance = null;

    // ── Quill helpers ──────────────────────────────────────────────────
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
        if (initialHtml) quillInstance.root.innerHTML = initialHtml;
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        quillInstance.on('text-change', function () {
            var hiddenInput = scope.querySelector('input[name="' + hiddenInputName + '"]');
            if (hiddenInput) hiddenInput.value = quillInstance.root.innerHTML;
        });
    }

    function syncQuillBeforeSubmit(hiddenInputName, modalBodyId) {
        if (!quillInstance) return;
        var scope = modalBodyId ? document.getElementById(modalBodyId) : document;
        var hiddenInput = scope.querySelector('input[name="' + hiddenInputName + '"]');
        if (hiddenInput) hiddenInput.value = quillInstance.root.innerHTML;
    }

    // ── Rendu colonnes ─────────────────────────────────────────────────
    function actionsHtml(id) {
        return '<button type="button" class="btn btn-sm btn-outline-warning btn-edit me-1" data-id="' + id + '">Éditer</button>'
             + '<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="' + id + '">Supprimer</button>';
    }

    function statutHtml(estActif) {
        return estActif
            ? '<span class="badge bg-success">Actif</span>'
            : '<span class="badge bg-danger">Inactif</span>';
    }

    // ── DataTables (client-side) ───────────────────────────────────────
    function initDt() {
        dt = $('#dt-emailtemplates').DataTable({
            serverSide: false,
            processing: true,
            ajax: {
                url: '/' + ctrl + '/DataJson',
                type: 'GET',
                dataSrc: 'data'
            },
            columns: [
                { data: 'code', className: 'ps-3', render: function (v) { return '<span class="badge bg-secondary">' + v + '</span>'; } },
                { data: 'nom' },
                { data: 'sujet' },
                { data: 'estActif', className: 'text-center', render: function (v) { return statutHtml(v); } },
                { data: 'description', render: function (v) { return '<small class="text-muted">' + (v || '') + '</small>'; } },
                { data: 'id', orderable: false, className: 'text-end pe-3', render: function (id) { return actionsHtml(id); } }
            ],
            pageLength: 10,
            lengthMenu: [[10, 20, 30, 50], [10, 20, 30, 50]],
            pagingType: 'full_numbers',
            language: {
                url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/fr-FR.json'
            },
            drawCallback: function () { bindRowButtons(); }
        });
    }

    // ── Liaison des boutons de ligne ───────────────────────────────────
    function bindRowButtons() {
        var $tbody = $('#dt-emailtemplates tbody');

        $tbody.off('click', '.btn-edit').on('click', '.btn-edit', function () {
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

        $tbody.off('click', '.btn-delete').on('click', '.btn-delete', function () {
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
                            dt.ajax.reload(null, false);
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
                dt.ajax.reload(null, false);
            }).fail(function () {
                $('#' + alertId).text('Erreur serveur inattendue.');
            });
        };
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
    $('#modal-edit').on('hide.bs.modal', destroyQuill);

    initDt();

}(jQuery));
