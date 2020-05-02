var calendar;
var selectedEventID;

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById("myModal");
    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
    }
    var windowWidth = window.innerWidth;
    var wantedWeekends = true;
    var wantedView = 'timeGridWeek';
    var wantedDur = '01:00:00';
    if (windowWidth < 725) {
        $(document).ready(function () {
            // Handler for .ready() called.
            $('html, body').animate({
                scrollTop: $('#button-header').offset().top - 55,
            }, 1000);
        });
        wantedWeekends = false;
    }
    var calendarEl = document.getElementById('calendar');
    calendar = new FullCalendar.Calendar(calendarEl, {
        allDaySlot: true,
        allDayText: 'Hele dag',
        weekends: wantedWeekends,
        buttonText:
        {
            month: 'Maand',
            week: 'Week',
            day: 'Dag',
            today: '  ‌‌▼  ',
        },
        plugins: ['dayGrid', 'bootstrap', 'interaction', 'timeGrid'],
        defaultView: wantedView,
        nowIndicator: 'true',
        //minTime: "06:00:00",
        //maxTime: "24:00:00",
        height: 'auto',
        firstDay: 1,
        draggable: true,
        lazyFetching: true,
        locale: 'nl',
        slotDuration: wantedDur,
        selectable: true,
        selectHelper: true,
        eventLimit: true,
        editable: true,
        droppable: true,
        dropAccept: true,
        longPressDelay: 500,
        slotLabelFormat: {
            hour: 'numeric',
            minute: '2-digit',
        },
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
                    selectedEventID = eventId;
                    var title = data[2];
                    var description = data[3];
                    var start = data[4];
                    var end = data[5];
                    var themeColor = data[6];
                    var fullDay = data[7];
                    var voornaam = data[9];
                    if (fullDay == 1) {
                        document.getElementById("fullDayField").selectedIndex = 1;
                    }
                    else {
                        document.getElementById("fullDayField").selectedIndex = 0;
                    }
                    var currentTokens = $('#voornaamFieldHeader').tokenfield('getTokens');

                    for (var i = 0; i < currentTokens.length; i++) {
                        var userIdByToken = currentTokens[i].value.split(" ");
                        if (userIdByToken[0] == userId) {
                            $('#voornaamField').tokenfield('setTokens', currentTokens[i].value);
                            break;
                        }
                    }
                    var rol = $("#rol").val();
                    document.getElementById("fullDayField").value = document.getElementById("fullDayField").selectedIndex;
                    document.getElementById("eventIdField").value = eventId;
                    document.getElementById("titleField").value = title;
                    document.getElementById("descriptionField").value = description;
                    document.getElementById("startField").value = start;
                    document.getElementById("endField").value = end;
                    document.getElementById("themeColorField").value = themeColor;
                    if (rol == "roostermaker") {
                        document.getElementById("userIdField2").value = userId;
                        document.getElementById("voornaamField").value = voornaam;
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
            var splitUserIdArray = [];
            var userIdArray = [];
            var userIds = "";
            var currentTokens = $('#voornaamFieldHeader').tokenfield('getTokens');
            for (var i = 0; i < currentTokens.length; i++) {
                userIdArray[i] = currentTokens[i].value;
                splitUserIdArray[i] = userIdArray[i].split(" ")[0];
            }
            $('#voornaamField').tokenfield('setTokens', currentTokens);
            for (var j = 0; j < splitUserIdArray.length; j++) {
                userIds += splitUserIdArray[j] + ",";
            }
            var soort = $("#themeColorField option:selected").text();
            var themeColor = $("#themeColorField option:selected").val();
            var naam = $("#userIdField1 option:selected").text();
            var start = new Date(selectionInfo.start.valueOf() - selectionInfo.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            var end = new Date(selectionInfo.end.valueOf() - selectionInfo.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            document.getElementById("eventIdField").value = 0;
            document.getElementById("titleField").value = soort;
            document.getElementById("descriptionField").value = soort;
            document.getElementById("startField").value = start;
            document.getElementById("endField").value = end;
            document.getElementById("themeColorField").value = themeColor;
            document.getElementById("submitButton").value = "Bevestig";
            document.getElementById("userIdField2").value = userIds;
            modal.style.display = "block";
        },
        eventDrop: function (eventDropInfo) {
            if (eventDropInfo.event.allDay == true) {
                $.ajax(
                    {
                        type: "GET",
                        url: '/Planner/UpdateAllDay?EventId=' + eventDropInfo.event.id + '&allDay=' + eventDropInfo.event.allDay,
                    });
            }
            else {
                var endTime = eventDropInfo.event.start;
                endTime.setHours(eventDropInfo.event.start.getHours() + 1);
                $.ajax(
                    {
                        type: "GET",
                        url: '/Planner/UpdateAgendaTimes?startTime=' + eventDropInfo.event.start.toISOString() + '&endTime=' + endTime.toISOString() + '&EventId=' + eventDropInfo.event.id + '&allDay=' + eventDropInfo.event.allDay,
                    });
            }
            
        },
    });

    calendar.setOption('locale', 'nl');
    FetchEvents();
    calendar.render();

});

function CloseModal() {
    var modal = document.getElementById("myModal");
    modal.style.display = "none";
    $('#voornaamField').tokenfield('setTokens', []);
    FetchEvents();
}
function SetUserID(namen, userids) {
    var selectednamen = document.getElementById("voornaamField").value;
    var selectednamenArray = selectednamen.split(",");

    var namenArray = namen.split(",");
    var useridsArray = userids.split(",");

    var userIds = "";
    if (useridsArray.length == namenArray.length) {
        for (var j = 0; j < selectednamenArray.length; j++) {
            for (var i = 0; i < namenArray.length; i++) {
                if (namenArray[i] == selectednamenArray[j]) {
                    userIds += useridsArray[i] + ",";
                    continue;
                }
            }

        }
        document.getElementById("userIdField2").value = userIds;
    }
}
function SetUserIDHeader(namen, userids) {
    var selectednamen = document.getElementById("voornaamFieldHeader").value;
    var selectednamenArray = selectednamen.split(",");

    var namenArray = namen.split(",");
    for (var i = 0; i < namenArray.length; i++) {
        namenArray[i] = namenArray[i].trim();
    }
    for (var i = 0; i < selectednamenArray.length; i++) {
        selectednamenArray[i] = selectednamenArray[i].trim();
    }
    var useridsArray = userids.split(",");

    var userIds = "";
    if (useridsArray.length == namenArray.length) {
        for (var j = 0; j < selectednamenArray.length; j++) {
            for (var i = 0; i < namenArray.length; i++) {
                if (namenArray[i] == selectednamenArray[j]) {
                    userIds += useridsArray[i] + ",";
                    continue;
                }
            }

        }
        document.getElementById("userIdField1").value = userIds;
    }
}
function DeleteEvent() {
    $.ajax(
        {
            type: "GET",
            url: '/Planner/DeleteEvent?EventId=' + selectedEventID,
        });
    console.log("Delete event");
}
function TradeEvent(info) {
    var selectedTokens = $('#voornaamField').tokenfield('getTokens');
    var employeedata = document.getElementById("naamLijst").value;
    var splitemployeedata = employeedata.split(",");
    for (var i = 0; i < employeedata.length; i++) {
        var userIdByToken = splitemployeedata[i].split(" ");
        if (userIdByToken[0] == loggedUser) {
            console.log(splitemployeedata[i]);
            $('#voornaamField').tokenfield('setTokens', splitemployeedata[i]);
            break;
        }
    }
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
    var selectedIds = "";
    var selectedTokens = $('#voornaamField').tokenfield('getTokens');
    if (selectedTokens.length > 0) {
        for (var i = 0; i < selectedTokens.length; i++) {
            var userIdByToken = selectedTokens[i].value.split(" ");
            selectedIds += userIdByToken[0] + ",";
        }
        document.getElementById("userIdField2").value = selectedIds;
        console.log($('#modalForm').serialize());
        $.ajax({
            url: '/Planner/CreateEvent',
            type: 'post',
            data: $('#modalForm').serialize(),
            success: function () {
                CloseModal();
            }
        });
        //if (Notification.permission === 'granted') {
        //    navigator.serviceWorker.getRegistration()
        //        .then(function (reg) {
        //            var options = {
        //                body: 'Uw rooster is aangepast',
        //                icon: 'img/cgi.png',
        //                data: {
        //                    dateOfArrival: Date.now(),
        //                    primaryKey: 1
        //                }
        //            };
        //            reg.showNotification('Event created!', options);
        //            console.log("Its working tho");
        //        });
        //}
    }
    else {
        window.alert("Selecteer een werknemer");
        FetchEvents();
    }


}
function EditTitle(info) {
    document.getElementById("titleField").value = info;
    document.getElementById("descriptionField").value = info;
}
function RemoveEvents() {
    var eventSources = calendar.getEventSources();
    eventSources.forEach(element => element.remove());
}

function InitTokenField(data) {
    var employeedata = data;
    var splitemployeedata = employeedata.split(",");
    $("#voornaamField").tokenfield({
        autocomplete: {
            source: splitemployeedata,
        },
        showAutocompleteOnFocus: true,
    });
    $('#voornaamField').on('tokenfield:createtoken', function (event) {
        var exists = false;
        $.each(splitemployeedata, function (index, value) {
            if (event.attrs.value === value) {
                exists = true;
            }
        });
        if (!exists) {
            event.preventDefault();
        }
        setTimeout(function () {
            $('#voornaamField').blur();
            $('#voornaamField').focus();
        }, 0)
    });
}

function InitHeaderTokenField(data) {
    var employeedata = data;
    var splitemployeedata = employeedata.split(",");

    $("#voornaamFieldHeader").tokenfield({
        autocomplete: {
            source: splitemployeedata,
        },
        showAutocompleteOnFocus: true,
    });
    $('#voornaamFieldHeader').on('tokenfield:createtoken', function (event) {
        var exists = false;
        var currentTokens = $('#voornaamFieldHeader').tokenfield('getTokens');
        $.each(splitemployeedata, function (index, value) {
            if (event.attrs.value === value) {
                exists = true;
            }
        });
        if (!exists) {
            event.preventDefault();
        }
        else {
            for (var i = 0; i < currentTokens.length; i++) {
                if (currentTokens[i].value === event.attrs.value) {
                    event.preventDefault();
                    document.getElementById('popup').style.display = 'block';
                    var strCmd = "document.getElementById('popup').style.display = 'none'";
                    var hideTimer = setTimeout(strCmd, 2000);
                }
            }
        }
        setTimeout(function () {
            $('#voornaamFieldHeader').blur();
            $('#voornaamFieldHeader').focus();
        }, 0)
    });
}
window.onload = function SetLoggedInUserToken() {
    var loggedUser = document.getElementById("loggedInUser").value;
    var employeedata = document.getElementById("naamLijst").value;
    this.InitTokenField(employeedata);
    this.InitHeaderTokenField(employeedata);
    var splitemployeedata = employeedata.split(",");
    for (var i = 0; i < employeedata.length; i++) {
        var userIdByToken = splitemployeedata[i].split(" ");
        if (userIdByToken[0] == loggedUser) {
            console.log(splitemployeedata[i]);
            $('#voornaamFieldHeader').tokenfield('setTokens', splitemployeedata[i]);
            break;
        }
    }

}
function changeDivVisibility() {
    var toolsDiv = document.getElementById("scheduler-tools");
    if (toolsDiv.style.display == "none") {
        toolsDiv.style.display = "block";
    }
    else {
        toolsDiv.style.display = "none";
    }
}
function FetchEvents() {
    var rol = $("#rol").val();
    var selectedIndex = "0";
    if (rol == "roostermaker") {
        selectedIndex = document.getElementById("userIdField1").value;
        var soortEvent = document.getElementById("soortDienstField").value;
    }
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
            console.log(calendar.eventSources);
            var e = new Date();
            var o = e.getTime();
            console.log(o - n);
        }
    });

}
