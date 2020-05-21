// Init global variables
var calendar;
var UserIDs;

// Load calendar
$(document).ready(() => {
    UserIDs = $("#UserID").val() + ",12";
    initCalendar();
});

function initCalendar() {
    var calendarEl = $("#calendar")[0];
    calendar = new FullCalendar.Calendar(calendarEl, {
        // Set plugins for FullCalendar and set default view
        plugins: ['dayGrid', 'timeGrid', 'bootstrap', 'interaction'],
        timeZone: 'local',
        defaultView: 'timeGridWeek',
        nowIndicator: true,
        height: "auto",
        selectable: true, 
        selectMirror: true,
        navLinks: true,
        weekNumbers: true,
        editable: true,

        // Theming
        themeSystem: 'bootstrap',

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
            createEvent(info);
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

function createEvent(info) {
    // Called when dragging in the callendar is completed
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
    console.log(eventDropInfo);
    $.get("/TestPlanner/UpdateEvent?start=" + eventDropInfo.event.start.toISOString() + "&end=" + eventDropInfo.event.end.toISOString() + "&eventid=" + eventDropInfo.event.id + "&allday=" + eventDropInfo.event.allDay, (data) => {
        Console.log("updated");
    })
}

function getEvents(info, succesCallback, failureCallback) {
    // Put events in calendar
    var events;    
    $.get('/TestPlanner/FetchAllEvents?userIds=' + UserIDs, (data) => {
        console.log(data);
        events = data;
    }).done(() => {
        succesCallback(events);
    }).fail((err) => {
        failureCallback(err);
    });
}