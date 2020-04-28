var calendar;

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById("myModal");
    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
    }
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
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
        locale: 'nl',
        slotDuration: '01:00:00',
        selectable: true,
        eventLimit: true,
        selectHelper: true,
        eventLimit: true,
        editable: true,
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
                    var rol = $("#rol").val();
                    if (rol == "roostermaker") {
                        document.getElementById("userIdField").value = userId;
                    }
                    else {
                        document.getElementById("fullDayField").disabled = true;
                        document.getElementById('eventIdField').readOnly = true; 
                        document.getElementById('titleField').readOnly = true; 
                        document.getElementById('descriptionField').readOnly = true; 
                        document.getElementById('startField').readOnly = true; 
                        document.getElementById('endField').readOnly = true; 
                        document.getElementById('themeColorField').disabled = true; 
                    }
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
    FetchEvents();
    calendar.render();

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
    RemoveEvents();
    FetchEvents();
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
function RemoveEvents() {
    var eventSource = calendar.getEventSources()[0];
    eventSource.remove();
}

function FetchEvents() {
    var selectedIndex = $("#userIdField1").val();
    var soortEvent = $("#soortDienstField").val();
    var rol = $("#rol").val();
    console.log(rol);
    $.ajax({
        url: '/Planner/FetchAllEvents?SendUserId=' + selectedIndex,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            var list = data;
            var d = new Date();
            var n = d.getTime();
            var objarray = [];
            for (var i = 0; i < list.length; i++) {
                if (list[i].themeColor != soortEvent && soortEvent != "Allemaal" && rol == "roostermaker") {
                    continue;
                }
                var obj = {
                    id: list[i].eventId,
                    title: list[i].title,
                    start: list[i].startDate,
                    end: list[i].endDate,
                    allDay: list[i].isFullDay,
                    backgroundColor: list[i].themeColor,
                    borderColor: '#010203',
                }
                if (rol != "roostermaker") {
                    obj.editable = false;
                }
                objarray.push(obj);
            }
            calendar.addEventSource(objarray);
            var e = new Date();
            var o = e.getTime();
            console.log(o - n);
        }
    });
}
