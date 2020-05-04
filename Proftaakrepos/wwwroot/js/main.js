window.onload = () => {
    'use strict';

    if ('serviceWorker' in navigator) {
        navigator.serviceWorker
            .register('./sw.js');
    }
    
}
Notification.requestPermission(function (status) {
    console.log('Notification permission status: ', status);
    displayNotification();
});

