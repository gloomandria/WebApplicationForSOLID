/**
 * Gestion DataTables server-side — Utilisateurs.
 * Dépend de : jquery, bootstrap 5, datatables.
 */
(function ($) {
    'use strict';

    var dt;
    var toggleModal     = new bootstrap.Modal(document.getElementById('modal-toggle'));
    var validationModal = new bootstrap.Modal(document.getElementById('modal-validation'));
    var pendingUserId   = null;

    function roleBadge(role) {
        var cls = role === 'Administrateur' ? 'bg-danger'
                : role === 'Enseignant'     ? 'bg-info text-dark'
                : role === 'Etudiant'       ? 'bg-success'
                : 'bg-secondary';
        return '<span class="badge rounded-pill ' + cls + '">' + role + '</span>';
    }

    function actifBadge(estActif) {
        return estActif
            ? '<span class="badge rounded-pill bg-success">● Actif</span>'
            : '<span class="badge rounded-pill bg-danger">● Inactif</span>';
    }

    function emailBadge(confirmed) {
        return confirmed
            ? '<span class="badge rounded-pill bg-success">✓ Confirmé</span>'
            : '<span class="badge rounded-pill bg-warning text-dark">⏳ En attente</span>';
    }

    function actionsHtml(row) {
        var toggleClass = row.estActif ? 'btn-outline-danger' : 'btn-outline-success';
        var toggleLabel = row.estActif ? '🔒 Désactiver' : '🔓 Activer';
        var actionLabel = row.estActif ? 'désactiver' : 'activer';
        return '<div class="d-flex justify-content-end gap-1 flex-wrap">'
             + '<a href="/Admin/AssignRole?userId=' + row.id + '" class="btn btn-sm btn-outline-primary">🎭 Rôle</a>'
             + '<button type="button" class="btn btn-sm ' + toggleClass + ' btn-toggle-active"'
             +   ' data-user-id="' + row.id + '" data-user-name="' + row.nomComplet + '" data-action-label="' + actionLabel + '">'
             + toggleLabel + '</button>'
             + '<button type="button" class="btn btn-sm btn-outline-info btn-send-validation"'
             +   ' data-user-id="' + row.id + '" data-user-name="' + row.nomComplet + '" data-user-email="' + row.email + '">'
             + '✉️ Valider</button>'
             + '</div>';
    }

    function initDt() {
        dt = $('#dt-users').DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: '/Admin/UsersDataJson',
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
                { data: 'nomComplet',    className: 'fw-semibold' },
                { data: 'email',         className: 'text-muted small' },
                { data: 'role',          render: function (v) { return roleBadge(v); } },
                { data: 'emailConfirme', className: 'text-center', render: function (v) { return emailBadge(v); } },
                { data: 'estActif',      className: 'text-center', render: function (v) { return actifBadge(v); } },
                { data: 'dateCreation',  className: 'text-muted small' },
                { data: null, orderable: false, className: 'text-end pe-3', render: function (_, __, row) { return actionsHtml(row); } }
            ],
            pageLength: 10,
            lengthMenu: [[10, 20, 50], [10, 20, 50]],
            pagingType: 'full_numbers',
            language: {
                url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/fr-FR.json'
            },
            columnDefs: [{ targets: 'nosort', orderable: false }],
            drawCallback: function () { bindRowButtons(); }
        });
    }

    function bindRowButtons() {
        var $tbody = $('#dt-users tbody');

        $tbody.off('click', '.btn-toggle-active').on('click', '.btn-toggle-active', function () {
            var isDeactivate = $(this).data('action-label') === 'désactiver';
            pendingUserId = $(this).data('user-id');

            document.getElementById('modal-toggle-action').textContent = $(this).data('action-label');
            document.getElementById('modal-toggle-user').textContent   = $(this).data('user-name');
            document.getElementById('modal-toggle-icon').textContent   = isDeactivate ? '🔒' : '🔓';
            document.getElementById('modal-toggle-title').textContent  = isDeactivate ? 'Désactiver le compte' : 'Activer le compte';
            document.getElementById('modal-toggle-header').className   = 'modal-header border-bottom '
                + (isDeactivate ? 'bg-danger bg-opacity-10' : 'bg-success bg-opacity-10');

            var btn = document.getElementById('modal-toggle-btn');
            btn.className   = 'btn px-4 ' + (isDeactivate ? 'btn-danger' : 'btn-success');
            btn.textContent = isDeactivate ? '🔒 Désactiver' : '🔓 Activer';

            toggleModal.show();
        });

        $tbody.off('click', '.btn-send-validation').on('click', '.btn-send-validation', function () {
            pendingUserId = $(this).data('user-id');
            document.getElementById('modal-validation-user').textContent  = $(this).data('user-name');
            document.getElementById('modal-validation-email').textContent = $(this).data('user-email');
            validationModal.show();
        });
    }

    function getToken() {
        var el = document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    document.getElementById('modal-toggle-btn').addEventListener('click', function () {
        if (!pendingUserId) return;
        $.post('/Admin/ToggleActive', { userId: pendingUserId, __RequestVerificationToken: getToken() })
            .done(function (res) {
                toggleModal.hide();
                if (res.success) dt.ajax.reload(null, false);
            });
    });

    document.getElementById('btn-confirm-validation').addEventListener('click', function () {
        if (!pendingUserId) return;
        $.post('/Admin/SendValidation', { userId: pendingUserId, __RequestVerificationToken: getToken() })
            .done(function () {
                validationModal.hide();
            });
    });

    $(document).ready(function () { initDt(); });

}(jQuery));
