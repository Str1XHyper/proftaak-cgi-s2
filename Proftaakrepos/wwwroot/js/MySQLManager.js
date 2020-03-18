function FetchAllEvents() {
    $.ajax({
        url: '/Home/FetchAllEvents',
        type: 'GET', // type of the HTTP request
        dataType: 'json',
        success: function (data) {
            return data;
        }
    });
}