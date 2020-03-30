function FetchAllEvents() {
    $.ajax({
        url: '/Planner/FetchAllEvents',
        type: 'GET', // type of the HTTP request
        dataType: 'json',
        success: function (data) {
            return data;
        }
    });
}