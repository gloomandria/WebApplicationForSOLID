/**
 * Gestion AJAX de la section Étudiants.
 * Dépend de : jquery, bootstrap 5, ajax-helpers.js
 */
(function ($) {
    'use strict';

    var appData     = document.getElementById('app-data');
    var ctrl        = appData.dataset.controller;
    var currentPage = parseInt(appData.dataset.currentPage, 10) || 1;
    var H           = window.AjaxHelpers;

    function refreshTable(page) {
        return $.get('/' + ctrl + '/Table', { page: page })
            .done(function (html) {
                $('#table-body').html(html);
                bindRowButtons();
            });
    }

    function loadAndWire(modalId, bodyId, getUrl, formId, alertId, postUrl) {
        H.loadModal(modalId, bodyId, getUrl)
            .done(function () {
                H.wireForm(formId, alertId, postUrl, modalId, function (msg) {
                    H.showAlert(msg, true);
                    refreshTable(currentPage);
                });
            });
    }

    // ── Bouton créer ──────────────────────────────────────────────────────────
    $('#btn-open-create').on('click', function () {
        $('#modal-create-alert').text('');
        loadAndWire(
            'modal-create', 'modal-create-body', '/' + ctrl + '/FormCreate',
            'form-create',  'modal-create-alert', '/' + ctrl + '/CreateAjax'
        );
    });

    // ── Pagination ────────────────────────────────────────────────────────────
    $('#pagination-links').on('click', '.ajax-page', function (e) {
        e.preventDefault();
        currentPage = parseInt($(this).data('page'), 10);
        refreshTable(currentPage);
    });

    // ── Boutons par ligne ─────────────────────────────────────────────────────
    function bindRowButtons() {
        $('.btn-details').off('click').on('click', function () {
            H.loadModal('modal-details', 'modal-details-body', '/' + ctrl + '/FormDetails?id=' + $(this).data('id'));
        });

        $('.btn-edit').off('click').on('click', function () {
            $('#modal-edit-alert').text('');
            loadAndWire(
                'modal-edit', 'modal-edit-body', '/' + ctrl + '/FormEdit?id=' + $(this).data('id'),
                'form-edit',  'modal-edit-alert', '/' + ctrl + '/EditAjax'
            );
        });

        $('.btn-delete').off('click').on('click', function () {
            $('#modal-delete-alert').text('');
            loadAndWire(
                'modal-delete', 'modal-delete-body', '/' + ctrl + '/FormDelete?id=' + $(this).data('id'),
                'form-delete',  'modal-delete-alert', '/' + ctrl + '/DeleteAjax'
            );
        });
    }

    bindRowButtons();

}(jQuery));
