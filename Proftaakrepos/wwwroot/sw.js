var cacheName = 'cgi-pwa';
var filesToCache = [
    '/',
    '/Planner/Agenda',
    //'/css/style.css',
    //'/js/main.js'
];

/* Start the service worker and cache all of the app's content */
self.addEventListener('install', function (e) {
    e.waitUntil(
        caches.open(cacheName).then(function (cache) {
            return cache.addAll(filesToCache);
        })
    );
});

/* Serve cached content when offline */
self.addEventListener('fetch', function (e) {
    e.respondWith(
        caches.match(e.request).then(function (response) {
            return response || fetch(e.request);
        })
    );
});
//self.addEventListener('fetch', function (event) {
//    return event.respondWith(
//        caches.match(event.request)
//            .then(function (response) {
//                let requestToCache = event.request.clone();

//                return fetch(requestToCache).then().catch(error => {
//                    // Check if the user is offline first and is trying to navigate to a web page
//                    if (event.request.method === 'GET' && event.request.headers.get('accept').includes('text/html')) {
//                        // Return the offline page
//                        return caches.match(offlineUrl);
//                    }
//                });
//            })
//    );
//});

