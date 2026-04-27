/**
 * Gestion AJAX de la section Matières.
 * Dépend de : jquery, bootstrap 5, ajax-helpers.js
 */
(function ($) {
    'use strict';

    var ctrl = document.getElementById('app-data').dataset.controller;
    var H    = window.AjaxHelpers;

    function refreshTable() {
        return $.get('/' + ctrl + '/Table')
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
                    refreshTable();
                });
            });
    }

    $('#btn-open-create').on('click', function () {
        $('#modal-create-alert').text('');
        loadAndWire(
            'modal-create', 'modal-create-body', '/' + ctrl + '/FormCreate',
            'form-create',  'modal-create-alert', '/' + ctrl + '/CreateAjax'
        );
    });

    function bindRowButtons() {
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
