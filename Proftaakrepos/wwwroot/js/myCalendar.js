var calendar;
document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    calendar = new FullCalendar.Calendar(calendarEl, {
        customButtons: {
            addEventButton: {
                text: 'Nieuwe afspraak',
                click: function () {
                    window.location.href = '/Planner/CreateEvent'
                }
            }
        },
        locale: 'nl',
        allDaySlot: true,
        allDayText: 'Hele dag',
        buttonText:
        {
            month: 'Maand',
            week: 'Week',
            day: 'Dag',
            today: '  ‌‌◯  ',
        },
        plugins: ['bootstrap', 'interaction', 'timeGrid', 'dayGrid'],
        defaultView: 'timeGridWeek',
        nowIndicator: 'true',
        minTime: "06:00:00",
        maxTime: "24:00:00",
        height: 'auto',
        selectable: true,
        selectHelper: true,
        editable: true,
        eventStartEditable: true,
        eventLimit: true,
        droppable: true,
        header: {
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: 'prev,today,next',
            left: 'addEventButton,title',
        },
        eventClick: function (info) {
            var url = "EditEvent?EventId=" + info.event.id;
            window.location.href = url;
        },
        views: {
            dayGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' },
            },
            timeGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' },
            },
        },
        select: function (info) {
            
        }
    });
    calendar.setOption('locale', 'nl');
    FetchEvents();
    calendar.render();
});
function FetchEvents() {
    var selectedIndex = $("#inputfield").val();
    calendar.getEvents().forEach(function (item, index) { item.remove() });
    $.ajax({
        url: '/Planner/FetchAllEvents?userId=' + selectedIndex,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var list = data;
            for (var i = 0; i < list.length; i++) {
                var obj = {}
                obj.id = list[i].eventId;
                obj.title = list[i].title;
                obj.start = list[i].startDate;
                obj.end = list[i].endDate;
                obj.allDay = list[i].isFullDay;
                obj.backgroundColor = list[i].themeColor;
                obj.borderColor = '#010203';
                calendar.addEvent(obj);
                calendar.rerenderEvents();
            }
        }
    });
}
