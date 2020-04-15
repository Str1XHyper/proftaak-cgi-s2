var calendar;

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById("myModal");
    var span = document.getElementsByClassName("close")[0];
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }
    span.onclick = function () {
        modal.style.display = "none";
    }
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
        draggable: true,
        selectable: true,
        eventLimit: true,
        selectHelper: true,
        editable: true,
        eventLimit: true,
        droppable: true,
        dropAccept: true,
        eventDragStop: function (info) {
            console.log(info.event.end)
        },
        
        header: {
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: 'prev,today,next',
            left: 'addEventButton,title',
        },
        eventClick: function (info) {
            $.ajax({
                url: '/Planner/GetEventInfo',
                type: 'GET',
                data: { EventId: info.event.id },
                success: function (data) {
                    var eventId = data[0];
                    var userId = data[1];
                    var title = data[2];
                    var description = data[3];
                    var start = data[4];
                    console.log(start);
                    var end = data[5];
                    var themeColor = data[6];
                    var fullDay = data[7];
                    document.getElementById("userIdField").value = userId;
                    document.getElementById("eventIdField").value = eventId;
                    document.getElementById("titleField").value = title;
                    document.getElementById("descriptionField").value = description;
                    document.getElementById("startField").value = start;
                    document.getElementById("endField").value = end;
                    document.getElementById("themeColorField").value = themeColor;
                    document.getElementById("fullDayField").value = fullDay;
                }
            });
            modal.style.display = "block";
            
        },
        views: {
            dayGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' },
            },
            timeGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' },
            },
        },
        eventResize: function (eventResizeInfo) {
            $.ajax(
                {
                    type: "GET",
                    url: '/Planner/UpdateAgendaTimes?startTime=' + eventResizeInfo.event.start.toISOString() + '&endTime=' + eventResizeInfo.event.end.toISOString() + '&EventId=' + eventResizeInfo.event.id,
                });
        },
        select: function (selectionInfo) {
            var selectedIndex = $("#userIdField1").val();
            console.log(selectedIndex);
            var start = new Date(selectionInfo.start.valueOf() - selectionInfo.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            var end = new Date(selectionInfo.end.valueOf() - selectionInfo.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            document.getElementById("eventIdField").value = 0;
            document.getElementById("startField").value = start;
            document.getElementById("endField").value = end;
            document.getElementById("submitButton").value = "Bevestig";
            document.getElementById("userIdField").value = selectedIndex;
            modal.style.display = "block";
        },
        eventDrop: function (eventDropInfo) {
            console.log(eventDropInfo.event.end)
            $.ajax(
                {
                    type: "GET",
                    url: '/Planner/UpdateAgendaTimes?startTime=' + eventDropInfo.event.start.toISOString() + '&endTime=' + eventDropInfo.event.end.toISOString() + '&EventId=' + eventDropInfo.event.id,
                });
        },
    });
    calendar.setOption('locale', 'nl');
    FetchEvents();
    calendar.render();
});
function CloseModal() {
    modal.style.display = "none";
    FetchEvents();
}

function DeleteEvent(info) {
    $.ajax(
        {
            type: "GET",
            url: '/Planner/DeleteEvent?EventId=' + info.value,
        });
    CloseModal();
}
function FetchEvents() {
    var selectedIndex = $("#userIdField1").val();
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
                obj.startEditable = true;
                obj.eventResourceEditable = true;
                obj.editable = true;
                obj.eventEditable = true;
                calendar.addEvent(obj);
            }
        }
    });
}
