// Init global variables
var calendar;
var UserIDs;
var modal;
var uname = [];
var uid = [];

// Load calendar
$(document).ready(async () => {
    modal = $("#eventModal")[0];
    await initTokenField();
    SetUserIDs();
    initCalendar();
});
function initCalendar() {
    var wantedWeekends = true;
    // Set mobile view
    if (window.innerWidth < 725) {
        wantedWeekends = false;
    }
    var calendarEl = $("#calendar")[0];
    calendar = new FullCalendar.Calendar(calendarEl, {
        // Set plugins for FullCalendar and set default params.
        defaultView: 'timeGridWeek',
        editable: true,
        firstDay: 1,
        height: "auto",
        locale: 'nl',
        longPressDelay: 500,
        navLinks: true,
        nowIndicator: true,
        plugins: ['dayGrid', 'timeGrid', 'bootstrap', 'interaction'],
        selectable: true,
        selectHelper: true,
        selectMirror: true,
        slotDuration: '01:00:00',
        timeZone: 'local',
        weekends: wantedWeekends,
        weekNumbers: true,

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
            setModalValues(info);
            changeModalState();
        },

        // Selecting an event functions
        eventClick: (info) => {
            $('#startField').val(new Date(info.event.start.valueOf() - info.event.start.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            $('#endField').val(new Date(info.event.end.valueOf() - info.event.end.getTimezoneOffset() * 60000).toISOString().replace(":00.000Z", ""));
            $("#eventIdField").val(info.event.id);
            setModalValues(info);
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
        // Set events
        events: (info, succesCallback, failureCallback) => {
            getEvents(info, succesCallback, failureCallback);
        }
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

function createCalendarEvent() {
    // Called when dragging in the callendar is completed
    $.post("/TestPlanner/CreateEvent", $('#eventForm').serialize()).done(() => {
        changeModalState();
        calendar.refetchEvents();
    });
}
function modifyEvent(info) {
    // Called when an existing event is clicked

}
function goToDay(date, jsevent) {
    // Execute when navLink day clicked.
    calendar.changeView('timeGridDay', date);
}

function goToWeek(weekStart, jsEvent) {
    // Execute when navLink week clicked.
    calendar.changeView('timeGridWeek', weekStart);
}

function moveEvent(eventDropInfo) {
    // Execute when you finish dropping an event
    var endtime = eventDropInfo.event.end;
    if (endtime == null) {
        endtime = eventDropInfo.event.start;
        endtime.setHours(endtime.getHours() + 1);
    }
    $.get("/TestPlanner/UpdateEvent?start=" + eventDropInfo.event.start.toISOString() + "&end=" + endtime.toISOString() + "&eventid=" + eventDropInfo.event.id + "&allday=" + eventDropInfo.event.allDay, (data) => {
        Console.log("updated");
    })
}
function resizeEvent(eventResizeInfo) {
    // Execute when you finish dropping an event
    $.get("/TestPlanner/UpdateEvent?start=" + eventResizeInfo.event.start.toISOString() + "&end=" + eventResizeInfo.event.end.toISOString() + "&eventid=" + eventResizeInfo.event.id + "&allday=" + eventResizeInfo.event.allDay, (data) => {
        Console.log("updated");
    })
}
function deleteEvent() {
    var eventId = $("#eventIdField").val();
    $.post('/Planner/DeleteEvent', { EventId: eventId }, (data) => {
        changeModalState();
        calendar.refetchEvents();
    });
}

function getEvents(info, succesCallback, failureCallback) {
    // Put events in calendar
    var soort = $("#eventType")[0].selectedOptions[0].value;
    var events;
    $.get('/TestPlanner/FetchAllEvents?userIds=' + UserIDs + "&type=" + soort, (data) => {
        console.log(data);
        events = data;
    }).done(() => {
        succesCallback(events);
    }).fail((err) => {
        failureCallback(err);
    });
}

function changeModalState() {
    //Change a modal's visibility
    if (modal.style.display == "block") {
        modal.style.display = "none";
    }
    else {
        modal.style.display = "block";
        initModalTokenField();
    }
}
function setModalValues(info) {
    // Set input fields in modal
    console.log(info);
    if (info != null) {
        $("#modalUserTokens").val(UserIDs);
        $("#titleField").val($('#themeColorField').find(":selected").text());
        $("#descriptionField").val($('#themeColorField').find(":selected").text());
        $("#themeColorField").val("Stand-by");
        $("#fullDayField").val($('#fullDayField').find(":selected").text());
        console.log("it works");
    }

}
async function initModalTokenField() {
    // Init tokenfield with users
    await $.get("/TestPlanner/GetUsers", (data) => {
        Array.from(data).forEach(name => {
            uid.push(name.split(" ")[0]);
            uname.push(name.split(/\d+/)[1].trim());
        });
    });

    // Set tokenfield
    $('#modalUserTokens').tokenfield({
        autocomplete: {
            source: uname,
        },
        showAutocompleteOnFocus: true
    })

    // Set user in tokenfield
    $('#modalUserTokens').tokenfield('setTokens', $('#UIDTokenField').tokenfield('getTokens'));
}

async function initTokenField() {
    // Init tokenfield with users
    await $.get("/TestPlanner/GetUsers", (data) => {
        Array.from(data).forEach(name => {
            uid.push(name.split(" ")[0]);
            uname.push(name.split(/\d+/)[1].trim());
        });
    });

    // Set tokenfield
    $('#UIDTokenField').tokenfield({
        autocomplete: {
            source: uname,
        },
        showAutocompleteOnFocus: true
    })

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
        $('#UIDTokenField').blur();
        $('#UIDTokenField').focus();
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
    if (calendar != null) {
        calendar.refetchEvents();
    }
});

$("#eventType").change((info) => {
    // Execute when select change
    calendar.refetchEvents();
});



function SetUserIDs() {
    // Set UserID variable to selected tokens.
    var TokenFieldVal = $("#UIDTokenField").tokenfield('getTokens');
    UserIDs = "";
    TokenFieldVal.forEach(token => {
        UserIDs += uid[uname.indexOf(token.value)] + ",";
    });
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

function tradeEvent() {
    var eventId = $("#eventIdField").val();
    var userId = $("#UserID").val();
    $.post('/Shiftview/CreateRequest', { EventID: eventId, UserId: userId }, (data) => {
        changeModalState();
        calendar.refetchEvents();
    });
}

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
