/**
 * Helpers AJAX partagés entre toutes les sections.
 * Dépend de jQuery et Bootstrap 5.
 */
(function ($, window) {
    'use strict';

    var AjaxHelpers = {

        /** Lit le token anti-forgery depuis le premier champ caché du formulaire. */
        token: function () {
            return $('input[name="__RequestVerificationToken"]').first().val() || '';
        },

        /** Affiche une alerte dans la zone #alert-zone. */
        showAlert: function (msg, ok) {
            $('#alert-zone').html(
                '<div class="alert alert-' + (ok ? 'success' : 'danger') +
                ' alert-dismissible fade show" role="alert">' + msg +
                '<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>'
            );
        },

        /**
         * Charge un partial HTML dans une modale Bootstrap.
         * @returns {jQuery.Deferred} — résolu avec le HTML chargé.
         */
        loadModal: function (modalId, bodyId, url) {
            var $body = $('#' + bodyId);
            $body.html('<div class="text-center py-3"><div class="spinner-border spinner-border-sm"></div></div>');
            bootstrap.Modal.getOrCreateInstance(document.getElementById(modalId)).show();
            return $.get(url)
                .done(function (html) { $body.html(html); })
                .fail(function () {
                    bootstrap.Modal.getInstance(document.getElementById(modalId)).hide();
                    AjaxHelpers.showAlert('Impossible de charger le formulaire.', false);
                });
        },

        /**
         * Attache le submit AJAX à un formulaire de modale.
         * @param {string} formId       - id du <form>
         * @param {string} alertId      - id de la zone d'alerte inline
         * @param {string} actionUrl    - URL POST
         * @param {string} modalId      - id de la modale parente
         * @param {Function} onSuccess  - callback appelé après succès (reçoit le message)
         */
        wireForm: function (formId, alertId, actionUrl, modalId, onSuccess) {
            var $form = $('#' + formId);
            $form[0].onsubmit = function (e) {
                e.preventDefault();
                $('#' + alertId).text('');
                var fd = new FormData($form[0]);
                $('#' + modalId + '-body').find('input,select,textarea').each(function () {
                    if (this.name) fd.set(this.name, this.value);
                });
                $.ajax({
                    url: actionUrl,
                    method: 'POST',
                    data: fd,
                    processData: false,
                    contentType: false,
                    headers: { 'RequestVerificationToken': AjaxHelpers.token() }
                }).done(function (data) {
                    if (!data.success) { $('#' + alertId).text(data.message); return; }
                    bootstrap.Modal.getInstance(document.getElementById(modalId)).hide();
                    if (typeof onSuccess === 'function') onSuccess(data.message);
                }).fail(function () {
                    $('#' + alertId).text('Erreur serveur inattendue.');
                });
            };
        }
    };

    window.AjaxHelpers = AjaxHelpers;

}(jQuery, window));
