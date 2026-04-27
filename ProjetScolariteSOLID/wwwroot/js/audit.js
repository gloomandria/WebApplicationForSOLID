/**
 * Gestion DataTables server-side — Journal d'audit.
 * Dépend de : jquery, bootstrap 5, datatables.
 */
(function ($) {
    'use strict';

    var dt;

    var actionBadge = {
        'INSERT': '<span class="badge bg-success">INSERT</span>',
        'UPDATE': '<span class="badge bg-warning text-dark">UPDATE</span>',
        'DELETE': '<span class="badge bg-danger">DELETE</span>'
    };

    function initDt() {
        dt = $('#dt-audit').DataTable({
            serverSide: true,
            processing: true,
            ajax: {
                url: '/Audit/DataJson',
                type: 'GET',
                data: function (d) {
                    var order = d.order && d.order.length ? d.order[0] : {};
                    return {
                        draw:        d.draw,
                        start:       d.start,
                        length:      d.length,
                        searchValue: d.search ? d.search.value : '',
                        tableFilter: $('#filter-table').val(),
                        userFilter:  $('#filter-user').val(),
                        sortCol:     order.column !== undefined ? order.column : 0,
                        sortDir:     order.dir || 'desc'
                    };
                }
            },
            columns: [
                { data: 'id' },
                { data: 'timestamp', className: 'text-nowrap' },
                { data: 'tableName', render: function (v) { return '<code>' + v + '</code>'; } },
                { data: 'action',    render: function (v) { return actionBadge[v] || '<span class="badge bg-secondary">' + v + '</span>'; } },
                { data: 'keyValues', render: function (v) { return '<small class="text-muted">' + v + '</small>'; } },
                { data: 'userId',    render: function (v) { return v ? '<small class="text-muted">' + v + '</small>' : '<span class="text-muted">—</span>'; } },
                {
                    data: 'id', orderable: false, className: 'text-end',
                    render: function (id) {
                        return '<a href="/Audit/Details/' + id + '" class="btn btn-sm btn-outline-info">Détail</a>';
                    }
                }
            ],
            order: [[1, 'desc']],
            pageLength: 20,
            lengthMenu: [[10, 20, 50, 100], [10, 20, 50, 100]],
            pagingType: 'full_numbers',
            language: {
                url: 'https://cdn.datatables.net/plug-ins/2.1.8/i18n/fr-FR.json'
            },
            columnDefs: [{ targets: 'nosort', orderable: false }]
        });
    }

    $('#btn-filter').on('click', function () {
        dt.ajax.reload();
    });

    $('#btn-reset').on('click', function () {
        $('#filter-table').val('');
        $('#filter-user').val('');
        dt.search('').ajax.reload();
    });

    $(document).ready(function () { initDt(); });

}(jQuery));
