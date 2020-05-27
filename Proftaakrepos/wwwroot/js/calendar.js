// Init global variables
var calendar;
var UserIDs = $('UserID').val() + ",";
var modal;
var modalUIDs;
var uname = [];
var uid = [];
var origin;

// Load calendar
$(document).ready(async () => {
    modal = $("#eventModal")[0];
    await initTokenField();
    await initModalTokenField();
    SetUserIDs();
    initCalendar();
    handleAbsenceAlert();
});

//Calendar functions
function initCalendar() {
    var wantedWeekends = true;
    // Set mobile view
    //if (window.innerWidth < 725) {
    //    wantedWeekends = false;
    //}
    var calendarEl = $("#calendar")[0];
    calendar = new FullCalendar.Calendar(calendarEl, {
        // Set plugins for FullCalendar and set default params.
        defaultView: 'timeGridWeek',
        editable: true,
        firstDay: 1,
        height: "auto",
        locale: $('#language').val(),
        longPressDelay: 500,
        navLinks: true,
        nowIndicator: true,
        plugins: ['dayGrid', 'timeGrid', 'bootstrap', 'interaction'],
        selectable: true,
        selectHelper: true,
        selectMirror: true,
        slotDuration: '01:00:00',
        slotLabelFormat: {
            hour: 'numeric',
            minute: '2-digit',
        },
        timeZone: 'local',
        weekends: wantedWeekends,
        weekNumbers: true,

        // Set events
        events: (info, succesCallback, failureCallback) => {
            getEvents(info, succesCallback, failureCallback);
        },

        // Set button text
        buttonText:
        {
            today: '  ‌‌▼  ',
        },

        // Create custom buttons 
        customButtons: {
            DayGridMonthView: {
                text: 'Month',
                click: () => {
                    MonthButtonClick();
                }
            },
            TimeGridWeekView: {
                text: 'Week',
                click: () => {
                    WeekButtonClick();
                }
            },
            TimeGridDayView: {
                text: 'Day',
                click: () => {
                    DayButtonClick();
                }
            }
        },

        // Set header buttons
        header: {
            left: 'title',
            center: 'prev,today,next',
            right: 'TimeGridDayView,TimeGridWeekView,DayGridMonthView,'
        },

        // Set header format
        titleFormat: {
            year: "numeric",
            month: "2-digit",
            day: "2-digit"
        },

        // Drag to create event functions
        select: (info) => {
            $('#startField').val(new Date(info.start.valueOf() - info.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            $('#endField').val(new Date(info.end.valueOf() - info.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            info.allDay == true ? $('#fullDayField').val(1) : $('#fullDayField').val(0);
            origin = "select";
            setModalValues(info);
            setEmployeeElements("select");
            changeModalState();
        },

        // Selecting an event functions
        eventClick: (info) => {
            $('#startField').val(new Date(info.event.start.valueOf() - info.event.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            if (info.event.allDay != true) $('#endField').val(new Date(info.event.end.valueOf() - info.event.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            $("#eventIdField").val(info.event.id);
            info.event.allDay == true ? $('#fullDayField').val(1) : $('#fullDayField').val(0);
            origin = "click";
            setModalValues(info.event);
            setEmployeeElements("click");
            changeModalState();
        },

        // navLinks functions
        navLinkDayClick: (date, jsEvent) => {
            goToDay(date, jsEvent);
        },
        navLinkWeekClick: (date, jsEvent) => {
            goToWeek(date, jsEvent);
        },

        // Handle drag 'n drop functionality
        eventDrop: (eventDropInfo) => {
            moveEvent(eventDropInfo);
        },
        eventResize: (eventResizeInfo) => {
            resizeEvent(eventResizeInfo);
        },

    });

    // Display calendar
    calendar.render();
}
function DayButtonClick() {
    // Execute when day button is pressed
    calendar.changeView('timeGridDay');
}
function WeekButtonClick() {
    // Execute when week button is pressed
    calendar.changeView('timeGridWeek');
}
function MonthButtonClick() {
    // Execute when month button is pressed
    calendar.changeView('dayGridMonth');
}
function goToDay(date, jsevent) {
    // Execute when navLink day clicked.
    calendar.changeView('timeGridDay', date);
}
function goToWeek(weekStart, jsEvent) {
    // Execute when navLink week clicked.
    calendar.changeView('timeGridWeek', weekStart);
}

//Event functions
function createCalendarEvent() {
    // Called when dragging in the callendar is completed
    SetModalUserIDs();
    enableInputs();
    $.post("/Planner/CreateEvent", $('#eventForm').serialize()).done(() => {
        changeModalState();
        calendar.refetchEvents();
    });
    setEmployeeElements();
}
function modifyEvent() {
    SetModalUserIDs();
    enableInputs();
    deleteEvent();
    createCalendarEvent();
    setEmployeeElements();
}
function chooseFunc() {
    if (origin == "select") {
        createCalendarEvent();
    }
    else if (origin == "click") {
        modifyEvent();
    }
}
function moveEvent(eventDropInfo) {
    // Execute when you finish dropping an event
    var endtime = eventDropInfo.event.end;
    if (endtime == null) {
        endtime = eventDropInfo.event.start;
        endtime.setHours(endtime.getHours() + 1);
    }
    $.get("/Planner/UpdateEvent?start=" + eventDropInfo.event.start.toISOString() + "&end=" + endtime.toISOString() + "&eventid=" + eventDropInfo.event.id + "&allday=" + eventDropInfo.event.allDay, (data) => {
    })
}
function resizeEvent(eventResizeInfo) {
    // Execute when you finish dropping an event
    $.get("/Planner/UpdateEvent?start=" + eventResizeInfo.event.start.toISOString() + "&end=" + eventResizeInfo.event.end.toISOString() + "&eventid=" + eventResizeInfo.event.id + "&allday=" + eventResizeInfo.event.allDay, (data) => {
    })
}
function deleteEvent() {
    // Deletes an event
    $.post('/Planner/DeleteEvent', { EventId: $("#eventIdField").val() }, (data) => {
        calendar.refetchEvents();
    });
}
function getEvents(info, succesCallback, failureCallback) {
    // Put events in calendar
    var soort = $("#eventType")[0].selectedOptions[0].value;
    var events;
    $.get('/Planner/FetchAllEvents?userIds=' + UserIDs + "&type=" + soort, (data) => {
        events = data;
    }).done(() => {
        succesCallback(events);
    }).fail((err) => {
        failureCallback(err);
    });
}
function tradeEvent() {
    var eventId = $("#eventIdField").val();
    var userId = $("#UserID").val();
    $.post('/Shiftview/CreateRequest', { EventID: eventId, UserId: userId }, (data) => {
        changeModalState();
        calendar.refetchEvents();
    });
}

//View related functions
function changeModalState() {
    //Change a modal's visibility
    initModalTokenField();
    modal.style.display == "block" ? modal.style.display = "none" : modal.style.display = "block";
}
function setModalValues(info) {
    console.log(info);
    // Set input fields in modal
    if (info != null) {
        $("#modalUserTokens").val(UserIDs);
        if (origin == "click") {
            $('#modalUserTokens').tokenfield('setTokens', uname[info.extendedProps.userId - 1]);
            $("#modalUserTokens").val($('#UserID').val() + ",");
        }
        typeof info.extendedProps !== 'undefined' ? $("#themeColorField").val(info.extendedProps.soort) : $("#themeColorField").val("Stand-by");
        typeof info.title !== 'undefined' ? $("#titleField").val($("#themeColorField")[0].selectedOptions[0].value) : $("#titleField").val("Stand-by");
        typeof info.extendedProps !== 'undefined' ? $("#descriptionField").val($("#themeColorField")[0].selectedOptions[0].value) : $("#descriptionField").val("Stand-by");
    }
}
function enableInputs() {
    $("#titleField").removeAttr('disabled');
    $("#descriptionField").removeAttr('disabled');
    $("#themeColorField").removeAttr('disabled');
    $("#fullDayField").removeAttr('disabled');
    $("#modalUserTokens").tokenfield('enable');
}
function slideTools() {
    if ($('#scheduler-tools').css("display") == "none") {
        $('#scheduler-tools').slideDown(100);
    }
    else {
        $('#scheduler-tools').slideUp(100);
    }
}
function handleAbsenceAlert() {
    $('#pop-up').slideDown(1000);
}

//Tokenfield functions
async function initModalTokenField() {
    // Set tokenfield
    $('#modalUserTokens').tokenfield({
        autocomplete: {
            source: uname,
        },
        showAutocompleteOnFocus: true
    })

    // Set user in tokenfield
    if (origin != "click")
        $('#modalUserTokens').tokenfield('setTokens', $('#UIDTokenField').tokenfield('getTokens'));
}
$("#modalUserTokens").on('tokenfield:createtoken', (event) => {
    // When token added to token field

    // Check to see if token is already in tokenfield
    var exists = false;
    uname.forEach(name => {
        var values = event.currentTarget.value.split(", ");
        if (values.includes(event.attrs.value)) {
            exists = true;
        }
    });

    // If token in tokenfield, don't add it to tokenfield
    if (exists) {
        event.preventDefault();
    }

    // Refocus tokenfield
    setTimeout(function () {
        $("#modalUserTokens-tokenfield").blur();
    }, 0);
})
async function initTokenField() {
    await $.get("/Planner/GetUsers", (data) => {
        Array.from(data).forEach(name => {
            uid.push(name.split(" ")[0]);
            //uname.push(name.split(/\d+/)[1].trim());
            uname.push(name.trim().replace(' ', ' - '));
        });
    });

    // Set tokenfield
    $('#UIDTokenField').tokenfield({
        autocomplete: {
            source: uname,
        },
        showAutocompleteOnFocus: true
    })

    initModalTokenField();
    // Set user in tokenfield
    $('#UIDTokenField').tokenfield('setTokens', uname[$("#UserID")[0].value - 1]);
}
$('#UIDTokenField').on('tokenfield:createtoken', (event) => {
    // When token added to token field

    // Check to see if token is already in tokenfield
    var exists = false;
    uname.forEach(name => {
        var values = event.currentTarget.value.split(", ");
        if (values.includes(event.attrs.value)) {
            exists = true;
        }
    });

    // If token in tokenfield, don't add it to tokenfield
    if (exists) {
        event.preventDefault();
    }

    // Refocus tokenfield
    setTimeout(function () {
        $("#UIDTokenField-tokenfield").blur();
    }, 0);
})
$('#UIDTokenField').on('tokenfield:createdtoken', (event) => {
    // Execute when token is created
    SetUserIDs();
    if (calendar != null) {
        calendar.refetchEvents();
    }
});
$('#UIDTokenField').on('tokenfield:removedtoken', (event) => {
    // Execute when token is created
    RemoveUserID(event.attrs.value);
    var tokencount = $("#UIDTokenField").tokenfield('getTokens').length;
    if (tokencount == 0) {
        UserIDs = "0,";
    }
    if (calendar != null) {
        calendar.refetchEvents();
    }
});
$("#eventType").change((info) => {
    // Execute when select change
    calendar.refetchEvents();
});

//UserID functions
function SetUserIDs() {
    // Set UserID variable to selected tokens.
    if (rol.value == "roostermaker") {
        var TokenFieldVal = $("#UIDTokenField").tokenfield('getTokens');
        UserIDs = "";
        TokenFieldVal.forEach(token => {
            UserIDs += uid[uname.indexOf(token.value)] + ",";
        });
    }
    else {
        UserIDs = $("#UserID").val() + ",";
    }
}
function SetModalUserIDs() {
    // Set UserID variable to selected tokens from modal.
    if (rol.value == "roostermaker") {
        var TokenFieldVal = $("#modalUserTokens").tokenfield('getTokens');
        modalUIDs = "";
        TokenFieldVal.forEach(token => {
            modalUIDs += uid[uname.indexOf(token.value)] + ",";
        });
        $("#modalUserID").val(modalUIDs);
    }
}
function RemoveUserID(value) {
    // Remove UserID from UserIDs
    var userid = uid[uname.indexOf(value)];
    var UIDArray = UserIDs.split(",");
    UIDArray.forEach(UID => {
        if (UID != "") {
            if (UID === userid) {
                UIDArray.splice(UIDArray.indexOf(UID), 1);
            }
        }
    });
    UserIDs = UIDArray.toString();
}

//Rights based functions
function setEmployeeElements(origin) {
    // Adjust/disable elements to match an employees rights
    var bool = false;
    if ($("#rol").val() != "roostermaker") {
        bool = true;
        if (origin == "select") {
            $("#verlofbtn").css("display", "block");
            $("#tradebtn").css("display", "none");
            $("#titleField").val("Verlof");
            $("#descriptionField").val("Verlof");
            $("#themeColorField").val("Verlof");
            $("#fullDayField").removeAttr('disabled');
        }
        else if (origin == "click") {
            $("#verlofbtn").css("display", "none");
            $("#tradebtn").css("display", "block");
            $("#fullDayField").attr('disabled', 'disabled');
            $("#startField").val("Verlof");
            $("#startField").removeAttr('disabled');
        }
    }
    $("#titleField").prop("disabled", bool);
    $("#descriptionField").prop("disabled", bool);
    $("#themeColorField").prop("disabled", bool);
}

//Element stuffz
$("#verlofbtn").click(createCalendarEvent);
$("#submitButton").click(chooseFunc);
$("#deletebtn").click(() => {
    deleteEvent();
    changeModalState();
});
$("#themeColorField").change(() => {
    $("#modalUserTokens").tokenfield('enable');
    var defaults = ["Stand-by", "Incidenten", "Verlof", "Pauze"];
    var title = $("#titleField").val();
    var description = $("#descriptionField").val();
    if (defaults.includes(title)) {
        $("#titleField").val($("#themeColorField")[0].selectedOptions[0].value);
    }
    if (defaults.includes(description)) {
        $("#descriptionField").val($("#themeColorField")[0].selectedOptions[0].value);
    }
    if ($("#themeColorField")[0].selectedOptions[0].value == "Verlof") {
        $("#modalUserTokens").tokenfield('setTokens', uname[$("#UserID")[0].value - 1]);
        $("#modalUserTokens").tokenfield('disable');
    }
});
$("#schedulerToolsHeader").click(slideTools);
$("#pop-up").click(() => {
    $('#pop-up').slideUp(100);
});

/*Bugs below*/
//                        /'.    /|   .'\
//                 ,._   |+i\  /++\  / +|    ,,
//                 |*+'._/+++\/+ ++\/+++<_.-'+|
//            :-.  \ ++++?++ +++++*++++++ +++ /  .-:
//            |*+\_/++++ +++*++ ++++++ ++?++++\_/ +|
//        ,    \*+++++ ++++ +++*+++ ++++ +++ +++++/   ,
//        \'-._> +__+*++__*+++_+__*++ ++__++++__*<_.-'/
//         `>*+++|  \++/  |+*/     `\ +|  |++/  |++++<'
//      _,-'+ * +*\  \/  /++|__.-.  |+ |  |+/  /+ +*+'-._
//      '-.*+++++++\    /+ ++++++/  / *|  |/  /+ ++++++.-'
//          > *+++++\  /*++++ +/` / `+++|     < *++ +++< 
//      _,-'* +++ ++|  |++ +*/` / ` +* +|  |\  \+ ++++++'-._
//      `-._ + +* ++? +|  | +++*| '-----.+|  |+\  \+* ++ +_.-'
//         _`\++++++|__|+ *+|________|+|__|++\__|++++/`_
//        /*++_+* + +++++ ++ + ++++ +++++ ++ ++++ ++_+*+\
//        '--' `>*+++ +++++ +++++*++++  +++ ++++ ?<' '--'
//             /++_++ ++ ++++++ ++?+ +++++*+++ ++++ \
//             |_/ `\++ ++ +++*++++++++++ ++++*./`\_|
//                  /+*.-.*+ +_ ++*+ _+++ .-.* +\
//            jgs   | /   | +/ `\?+/` \*+|    \ |
//                   '    \.'    |/    './     '
