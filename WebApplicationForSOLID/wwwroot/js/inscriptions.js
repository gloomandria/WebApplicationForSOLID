/**
 * Gestion AJAX de la section Inscriptions — DataTables server-side.
 * Dépend de : jquery, bootstrap 5, datatables, ajax-helpers.js
 */
(function ($) {
    'use strict';

    var ctrl = document.getElementById('app-data').dataset.controller;
    var H    = window.AjaxHelpers;
    var dt;

    var statutClasses = {
        'Active':    'bg-success',
        'Suspendue': 'bg-warning text-dark'
    };

    function statutHtml(v) {
        var cls = statutClasses[v] || 'bg-danger';
        return v ? '<span class="badge ' + cls + '">' + v + '</span>' : '';
    }

    function actionsHtml(id) {
        return '<button type="button" class="btn btn-sm btn-outline-warning btn-edit-statut me-1" data-id="' + id + '">Statut</button>'
             + '<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="' + id + '">Supprimer</button>';
    }

    function initDt() {
        dt = $('#dt-inscriptions').DataTable({
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
                { data: 'etudiant' },
                { data: 'classe' },
                { data: 'dateInscription' },
                { data: 'statut', render: function (v) { return statutHtml(v); } },
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
        var $tbody = $('#dt-inscriptions tbody');

        $tbody.off('click', '.btn-edit-statut').on('click', '.btn-edit-statut', function () {
            $('#modal-edit-alert').text('');
            H.loadModal('modal-edit-statut', 'modal-edit-body', '/' + ctrl + '/FormEditStatut?id=' + $(this).data('id'))
                .done(function () {
                    H.wireForm('form-edit-statut', 'modal-edit-alert', '/' + ctrl + '/EditStatutAjax', 'modal-edit-statut', function (msg) {
                        H.showAlert(msg, true);
                        dt.ajax.reload(null, false);
                    });
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
                H.wireForm('form-create', 'modal-create-alert', '/' + ctrl + '/CreateAjax', 'modal-create', function (msg) {
                    H.showAlert(msg, true);
                    dt.ajax.reload(null, false);
                });
            });
    });

    initDt();

}(jQuery));
