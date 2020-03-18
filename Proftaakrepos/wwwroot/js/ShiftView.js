//function openCity(cityName) {
//    var i;
//    var x = document.getElementsByClassName("city");
//    for (i = 0; i < x.length; i++) {
//        x[i].style.display = "none";
//    }
//    document.getElementById(cityName).style.display = "block";
//}

function openTab(evt, tabName) {
    var i, x, tablinks, status;
    x = document.getElementsByClassName("tab");
    status = document.getElementById("status");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" w3-black", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " w3-black";
    statusText = getElementsByClassName(tabName).innerHTML
    status.innerHTML = statusText;
}