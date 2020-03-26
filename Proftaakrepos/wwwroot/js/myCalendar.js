document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        customButtons: {
            addEventButton: {
                text: 'Add event',
                click: function () {
                    window.location.href = '/Home/CreateEvent'
                }
            }
        },
        plugins: ['timeGrid', 'dayGrid'],
        defaultView: 'timeGridWeek',
        nowIndicator: 'true',
        minTime: "06:00:00",
        maxTime: "24:00:00",
        height: 'auto',
        header: {
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: 'prev,today,next',
            left: 'addEventButton,title',
        },
        views: {
            dayGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' }
            },
            timeGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' }
            },
        }
    });
    $.ajax({
        url: '/Home/FetchAllEvents',
        type: 'GET', // type of the HTTP request
        dataType: 'json',
        success: function (data) {
            var list = data;
            for (var i = 0; i < list.length; i++) {
                var obj = {}
                obj.title = list[i].title;
                obj.start = list[i].startDate;
                obj.end = list[i].endDate;
                calendar.addEvent(obj);
                calendar.rerenderEvents()
            }
        }
    });
    calendar.render();
});