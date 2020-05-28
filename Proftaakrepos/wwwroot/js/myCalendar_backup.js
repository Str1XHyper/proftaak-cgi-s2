var calendar;
var selectedEventID;
var rol;
var modal;
var span;
var loggedUser;
document.addEventListener('DOMContentLoaded', function () {
    modal = document.getElementById("myModal");
    span = document.getElementsByClassName("close")[0];
    var windowWidth = window.innerWidth;
    var wantedWeekends = true;
    var wantedView = 'timeGridWeek';
    var wantedDur = '01:00:00';
    if (windowWidth < 725) {
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
        height: 'auto',
        firstDay: 1,
        draggable: true,
        contentHeight: 'auto',
        lazyFetching: true,
        slotDuration: wantedDur,
        selectable: true,
        eventLimit: true,
        editable: true,
        droppable: true,
        dropAccept: true,
        slotLabelFormat: {
            hour: 'numeric',
            minute: '2-digit',
        },
        header: {
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: 'prev,today,next',
            left: 'title',
        },
        views: {
            dayGrid: {
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' },
                displayEventEnd: true,
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
        eventClick: function (info) {
            $.ajax({
                url: '/Planner/GetEventInfo',
                type: 'GET',
                data: { EventId: info.event.id },
                success: function (data) {
                    console.log(data);
                    selectedEventID = data[0];
                    data[7] == 1 ? document.getElementById("fullDayField").selectedIndex = 1 : document.getElementById("fullDayField").selectedIndex = 0;
                    var currentTokens = $('#voornaamFieldHeader').tokenfield('getTokens');
                    for (var i = 0; i < currentTokens.length; i++) {
                        var userIdByToken = currentTokens[i].value.split(" ");
                        if (userIdByToken[0] == data[1]) {
                            $('#voornaamField').tokenfield('setTokens', currentTokens[i].value);
                            break;
                        }
                    }
                    var rol = $("#rol").val();
                    document.getElementById("fullDayField").value = document.getElementById("fullDayField").selectedIndex;
                    document.getElementById("eventIdField").value = data[0];
                    document.getElementById("titleField").value = data[2];
                    document.getElementById("descriptionField").value = data[3];
                    document.getElementById("startField").value = data[4];
                    document.getElementById("endField").value = data[5];
                    console.log(data[6]);
                    document.getElementById("themeColorField").value = data[6];
                    document.getElementById("themeColorField").text = data[6];
                    if (rol == "roostermaker") {
                        document.getElementById("userIdField2").value = data[1];
                        document.getElementById("voornaamField").value = data[9];
                    }
                    else {
                        document.getElementById('eventIdField').readOnly = true;
                        document.getElementById('titleField').readOnly = true;
                        document.getElementById('descriptionField').readOnly = true;
                        document.getElementById('startField').readOnly = true;
                        document.getElementById('endField').readOnly = true;
                        document.getElementById('themeColorField').disabled = true;
                        document.getElementById('employee-func-btn-verlof').style.display = "none";
                        document.getElementById('employee-func-btn').style.display = "block";

                    }
                }
            });
            modal.style.display = "block";

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
            var naam = $("#userIdField1 option:selected").text();
            var start = new Date(selectionInfo.start.valueOf() - selectionInfo.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            var end = new Date(selectionInfo.end.valueOf() - selectionInfo.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", "");
            document.getElementById("eventIdField").value = 0;
            document.getElementById("titleField").value = soort;
            document.getElementById("descriptionField").value = soort;
            document.getElementById("startField").value = start;
            document.getElementById("endField").value = end;
            document.getElementById("themeColorField").value = soort;
            document.getElementById("themeColorField").text = soort;
            if (rol != "roostermaker") {

                document.getElementById('eventIdField').readOnly = true;
                document.getElementById("themeColorField").selectedIndex = 3;
                document.getElementById("titleField").value = "Verlof";
                document.getElementById("descriptionField").value = "Verlof";
                document.getElementById('titleField').readOnly = true;
                document.getElementById('descriptionField').readOnly = true;
                document.getElementById('startField').readOnly = true;
                document.getElementById('endField').readOnly = true;
                document.getElementById('themeColorField').disabled = true;
                document.getElementById('employee-func-btn-verlof').style.display = "block";
            }
            else {
                document.getElementById("userIdField2").value = userIds;
            }
            document.getElementById('employee-func-btn').style.display = "none";
            
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
            else if (eventDropInfo.oldEvent.allDay == true) {
                var endTime = eventDropInfo.event.start;
                endTime.setHours(eventDropInfo.event.start.getHours() + 1);
                $.ajax(
                    {
                        type: "GET",
                        url: '/Planner/UpdateAgendaTimes?startTime=' + eventDropInfo.event.start.toISOString() + '&endTime=' + endTime.toISOString() + '&EventId=' + eventDropInfo.event.id + '&allDay=' + eventDropInfo.event.allDay,
                    });
            }
            else {
                $.ajax(
                    {
                        type: "GET",
                        url: '/Planner/UpdateAgendaTimes?startTime=' + eventDropInfo.event.start.toISOString() + '&endTime=' + eventDropInfo.event.end.toISOString() + '&EventId=' + eventDropInfo.event.id + '&allDay=' + eventDropInfo.event.allDay,
                    });
            }

        },
    });
    calendar.setOption('locale', document.getElementById("language").value);
    FetchEvents();
    calendar.render();
});

function CloseModal() {
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
            success: function () {
                RemoveEvents();
                CloseModal();
            }
        });
}
function TradeEvent(info) {
    var eventId = $("#eventIdField").val();
    var userId = $("#loggedInUser").val();
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
    document.getElementById('themeColorField').disabled = false;
    if (selectedTokens.length > 0) {
        
        for (var i = 0; i < selectedTokens.length; i++) {
            var userIdByToken = selectedTokens[i].value.split(" ");
            selectedIds += userIdByToken[0] + ",";
        }
        document.getElementById("userIdField2").value = selectedIds;
        $.ajax({
            url: '/Planner/CreateEvent',
            type: 'post',
            data: $('#modalForm').serialize(),
            success: function () {
                CloseModal();
            }
        });
    }
    else if (selectedTokens.length == 0 && rol == "roostermaker") {
        window.alert("Selecteer een werknemer");
        FetchEvents();
    }
    else {
        document.getElementById("userIdField2").value = loggedUser + ",";
        console.log($('#modalForm').serialize());
        $.ajax({
            url: '/Planner/CreateEvent',
            type: 'post',
            data: $('#modalForm').serialize(),
            success: function () {
                RemoveEvents();
                CloseModal();
            }
        });
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
$(document).ready(function () {
    $('.alert').slideDown(1000);
    $('.alert').delay(5000).slideUp(1000);
});
function slideUpDiv() {
    $('.alert').slideUp(1000);
}
window.onload = function SetLoggedInUserToken() {
    loggedUser = document.getElementById("loggedInUser").value;
    var employeedata = document.getElementById("naamLijst").value;
    this.InitTokenField(employeedata);
    this.InitHeaderTokenField(employeedata);
    var splitemployeedata = employeedata.split(",");
    for (var i = 0; i < employeedata.length; i++) {
        var userIdByToken = splitemployeedata[i].split(" ");
        if (userIdByToken[0] == loggedUser) {
            $('#voornaamFieldHeader').tokenfield('setTokens', splitemployeedata[i]);
            break;
        }
    }

}
function toggleDiv() {
    $('.werknemer-header').slideToggle();
}
function changeDivVisibility() {
    var header = document.getElementById("scheduler-tools");
    header.style.display == "none" ? $('.werknemer-header').slideDown(1000) : $('.werknemer-header').slideUp(1000);
}

function FetchEvents() {

    rol = document.getElementById("rol").value;
    var colours = document.getElementById("colours").value;
    var colourArray = colours.split(",");
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
            var list = data;
            var d = new Date();
            var n = d.getTime();
            var objarray = [];
            for (var i = 0; i < list.length; i++) {
                if (list[i].themeColor != soortEvent && soortEvent != "Allemaal" && rol == "roostermaker") {
                    continue;
                }
                if (colours != "") {
                    if (list[i].themeColor == "Stand-by")
                        list[i].themeColor = colourArray[0];
                    else if (list[i].themeColor == "Incidenten")
                        list[i].themeColor = colourArray[1];
                    else if (list[i].themeColor == "Pauze")
                        list[i].themeColor = colourArray[2];
                    else if (list[i].themeColor == "Verlof")
                        list[i].themeColor = colourArray[3];
                }
                else {
                    if (list[i].themeColor == "Stand-by")
                        list[i].themeColor = "#3b5a6f";
                    else if (list[i].themeColor == "Incidenten")
                        list[i].themeColor = "#353b45";
                    else if (list[i].themeColor == "Pauze")
                        list[i].themeColor = "#828a87";
                    else if (list[i].themeColor == "Verlof")
                        list[i].themeColor = "#830101";
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
                if (obj.allDay) {

                }
                objarray.push(obj);
            }
            console.warn(objarray)
            calendar.addEventSource(objarray);
            var e = new Date();
            var o = e.getTime();
        }
    });

}
