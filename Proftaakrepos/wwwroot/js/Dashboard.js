document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['TimeGrid'],
        googleCalendarApiKey: 'AIzaSyDnLWG_laGq2r92JHi6Smzr3wcMiK_DSps',
        events: {
            googleCalendarId: 'proftaakbde@gmail.com'
        }
    });
    calendar.render();
});

