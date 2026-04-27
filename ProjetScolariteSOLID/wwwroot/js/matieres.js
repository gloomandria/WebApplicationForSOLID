/**
 * Gestion AJAX de la section Matières — DataTables server-side + Quill.
 * Dépend de : jquery, bootstrap 5, datatables, ajax-helpers.js, Quill
 */
(function ($) {
    'use strict';

    var ctrl = document.getElementById('app-data').dataset.controller;
    var H    = window.AjaxHelpers;
    var dt;
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
                dt.ajax.reload(null, false);
            }).fail(function () {
                $('#' + alertId).text('Erreur serveur inattendue.');
            });
        };
    }

    function actionsHtml(id) {
        return '<button type="button" class="btn btn-sm btn-outline-warning btn-edit me-1" data-id="' + id + '">Éditer</button>'
             + '<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="' + id + '">Supprimer</button>';
    }

    function initDt() {
        dt = $('#dt-matieres').DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: '/' + ctrl + '/DataJson',
                type: 'GET',
                data: function (d) {
                    var order = d.order && d.order.length ? d.order[0] : {};
                    return {
                        draw:        d.draw,
                        start:       d.start,
                        length:      d.length,
                        searchValue: d.search ? d.search.value : '',
                        sortCol:     order.column !== undefined ? order.column : 0,
                        sortDir:     order.dir || 'asc'
                    };
                }
            },
            columns: [
                { data: 'code', render: function (v) { return '<code>' + v + '</code>'; } },
                { data: 'intitule' },
                { data: 'coefficient', className: 'text-center', render: function (v) { return '<span class="badge bg-warning text-dark">' + v + '</span>'; } },
                { data: 'volumeHoraire', className: 'text-center', render: function (v) { return v + ' h'; } },
                { data: 'enseignant' },
                { data: 'id', orderable: false, className: 'text-end', render: function (id) { return actionsHtml(id); } }
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

    function bindRowButtons() {
        var $tbody = $('#dt-matieres tbody');

        $tbody.off('click', '.btn-edit').on('click', '.btn-edit', function () {
            $('#modal-edit-alert').text('');
            H.loadModal('modal-edit', 'modal-edit-body', '/' + ctrl + '/FormEdit?id=' + $(this).data('id'))
                .done(function () {
                    var initial = document.querySelector('#modal-edit-body input[name="Matiere.Description"]');
                    initQuill(initial ? initial.value : '', 'modal-edit-body');
                    wireFormWithQuill('form-edit', 'modal-edit-alert', '/' + ctrl + '/EditAjax', 'modal-edit', 'modal-edit-body');
                });
        });

        $tbody.off('click', '.btn-delete').on('click', '.btn-delete', function () {
            $('#modal-delete-alert').text('');
            H.loadModal('modal-delete', 'modal-delete-body', '/' + ctrl + '/FormDelete?id=' + $(this).data('id'))
                .done(function () {
                    H.wireForm('form-delete', 'modal-delete-alert', '/' + ctrl + '/DeleteAjax', 'modal-delete', function (msg) {
                        H.showAlert(msg, true);
                        dt.ajax.reload(null, false);
                    });
                });
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
    $('#modal-edit').on('hide.bs.modal', destroyQuill);

    initDt();

}(jQuery));
