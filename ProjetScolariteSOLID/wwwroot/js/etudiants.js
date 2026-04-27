/**
 * Gestion AJAX de la section Étudiants — DataTables server-side.
 * Dépend de : jquery, bootstrap 5, datatables, ajax-helpers.js
 */
(function ($) {
    'use strict';

    var ctrl = document.getElementById('app-data').dataset.controller;
    var H    = window.AjaxHelpers;
    var dt;

    function actionsHtml(id) {
        return '<button type="button" class="btn btn-sm btn-outline-info btn-details me-1" data-id="' + id + '">Détails</button>'
             + '<button type="button" class="btn btn-sm btn-outline-warning btn-edit me-1" data-id="' + id + '">Éditer</button>'
             + '<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="' + id + '">Supprimer</button>';
    }

    function initDt() {
        dt = $('#dt-etudiants').DataTable({
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
                { data: 'numeroEtudiant', render: function (v) { return '<span class="badge bg-secondary">' + v + '</span>'; } },
                { data: 'nomComplet' },
                { data: 'email' },
                { data: 'telephone' },
                { data: 'dateNaissance' },
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
        var $tbody = $('#dt-etudiants tbody');

        $tbody.off('click', '.btn-details').on('click', '.btn-details', function () {
            H.loadModal('modal-details', 'modal-details-body', '/' + ctrl + '/FormDetails?id=' + $(this).data('id'));
        });

        $tbody.off('click', '.btn-edit').on('click', '.btn-edit', function () {
            $('#modal-edit-alert').text('');
            H.loadModal('modal-edit', 'modal-edit-body', '/' + ctrl + '/FormEdit?id=' + $(this).data('id'))
                .done(function () {
                    H.wireForm('form-edit', 'modal-edit-alert', '/' + ctrl + '/EditAjax', 'modal-edit', function (msg) {
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
