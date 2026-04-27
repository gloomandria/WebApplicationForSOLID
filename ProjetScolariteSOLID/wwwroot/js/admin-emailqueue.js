/**
 * Gestion DataTables server-side — File d'emails.
 * Dépend de : jquery, bootstrap 5, datatables.
 */
(function ($) {
    'use strict';

    var statutBadge = {
        'EnAttente': '<span class="badge bg-warning text-dark">En attente</span>',
        'Envoye':    '<span class="badge bg-success">Envoyé</span>',
        'Echoue':    '<span class="badge bg-danger">Échoué</span>'
    };

    function initDt() {
        $('#dt-emailqueue').DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: '/Admin/EmailQueueDataJson',
                type: 'GET',
                data: function (d) {
                    var order = d.order && d.order.length ? d.order[0] : {};
                    return {
                        draw:        d.draw,
                        start:       d.start,
                        length:      d.length,
                        searchValue: d.search ? d.search.value : '',
                        sortCol:     order.column !== undefined ? order.column : 0,
                        sortDir:     order.dir || 'desc'
                    };
                }
            },
            columns: [
                { data: 'id' },
                { data: 'destinataire' },
                { data: 'sujet' },
                { data: 'statut',        render: function (v) { return statutBadge[v] || '<span class="badge bg-secondary">' + v + '</span>'; } },
                { data: 'nbTentatives',  className: 'text-center' },
                { data: 'dateCreation' },
                { data: 'dateEnvoi' },
                { data: 'messageErreur', className: 'text-danger small', orderable: false }
            ],
            order: [[5, 'desc']],
            pageLength: 20,
            lengthMenu: [[10, 20, 50, 100], [10, 20, 50, 100]],
            pagingType: 'full_numbers',
            language: {
                url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/fr-FR.json'
            },
            columnDefs: [{ targets: 'nosort', orderable: false }]
        });
    }

    $(document).ready(function () { initDt(); });

}(jQuery));
