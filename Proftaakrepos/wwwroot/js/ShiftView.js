//function openCity(cityName) {
//    var i;
//    var x = document.getElementsByClassName("city");
//    for (i = 0; i < x.length; i++) {
//        x[i].style.display = "none";
//    }
//    document.getElementById(cityName).style.display = "block";
//}

function openCity(evt, cityName) {
    var i, x, tablinks, status;
    x = document.getElementsByClassName("city");
    status = document.getElementById("status");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" w3-red", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " w3-red";
    status.innerHTML = cityName;
}