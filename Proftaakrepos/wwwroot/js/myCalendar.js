var calendar;

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById("myModal");
    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
    }
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
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
        plugins: ['dayGrid', 'bootstrap', 'interaction', 'timeGrid'],
        defaultView: 'timeGridWeek',
        nowIndicator: 'true',
        //minTime: "06:00:00",
        //maxTime: "24:00:00",
        height: 'auto',
        draggable: true,
        lazyFetching: true,
        slotDuration: '01:00:00',
        selectable: true,
        eventLimit: true,
        selectHelper: true,
        editable: true,
        eventLimit: true,
        droppable: true,
        dropAccept: true,
        header: {
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: 'prev,today,next',
            left: 'title',
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
                    console.log(fullDay);
                    if (fullDay == true) {
                        document.getElementById("fullDayField").selectedIndex = 1;
                    }
                    else {
                        document.getElementById("fullDayField").selectedIndex = 0;
                    }
                    document.getElementById("userIdField").value = userId;
                    document.getElementById("eventIdField").value = eventId;
                    document.getElementById("titleField").value = title;
                    document.getElementById("descriptionField").value = description;
                    document.getElementById("startField").value = start;
                    document.getElementById("endField").value = end;
                    document.getElementById("themeColorField").value = themeColor;
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
                eventLimit: 3,
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
            $.ajax(
                {
                    type: "GET",
                    url: '/Planner/UpdateAgendaTimes?startTime=' + eventDropInfo.event.start.toISOString() + '&endTime=' + eventDropInfo.event.end.toISOString() + '&EventId=' + eventDropInfo.event.id,
                });
        },
    });
    calendar.setOption('locale', 'nl');
    calendar.render();
    FetchEvents();

});
function CloseModal() {
    var modal = document.getElementById("myModal");
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
function TradeEvent(info) {
    var eventId = $("#eventIdField").val();
    var userId = $("#userIdField").val();
    $.ajax(
        {
            type: "POST",
            url: '/Shiftview/CreateRequest?EventID=' + eventId + "&UserID=" + userId,
        });
    CloseModal();
}
function HandleRequest() {
    $.ajax({
        url: '/Planner/CreateEvent',
        type: 'post',
        data: $('#modalForm').serialize(),
        success: function () {
            CloseModal();
        }
    });
}
function EditTitle(info) {
    document.getElementById("titleField").value = info;
    document.getElementById("descriptionField").value = info;
}
function FetchEvents() {
    var selectedIndex = $("#userIdField1").val();
    var soortEvent = $("#soortDienstField").val();
    console.log(soortEvent);
    calendar.getEvents().forEach(function (item, index) { item.remove() });
    $.ajax({
        url: '/Planner/FetchAllEvents?SendUserId=' + selectedIndex,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var list = data;
            for (var i = 0; i < list.length; i++) {
                if (list[i].themeColor != soortEvent && soortEvent != "Allemaal") {
                    continue;
                }
                var obj = {}
                obj.id = list[i].eventId;
                obj.title = list[i].title;
                obj.start = list[i].startDate;
                obj.end = list[i].endDate;
                obj.allDay = list[i].isFullDay;
                obj.backgroundColor = list[i].themeColor;
                obj.borderColor = '#010203';
                calendar.addEvent(obj);
            }
        }
    });
}
