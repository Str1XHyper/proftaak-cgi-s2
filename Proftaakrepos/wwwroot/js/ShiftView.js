//function openCity(cityName) {
//    var i;
//    var x = document.getElementsByClassName("city");
//    for (i = 0; i < x.length; i++) {
//        x[i].style.display = "none";
//    }
//    document.getElementById(cityName).style.display = "block";
//}

function openTab(evt, tabName) {
    var i;
    var tabs = document.getElementsByClassName("tab");
    var tablinks = document.getElementsByClassName("tablink");

    for (i = 0; i < tabs.length; i++) {
        tabs[i].style.display = "none";
    }

    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace("active", "");
    }

    document.getElementById(tabName).style.display = "block";
    document.getElementById(tabName + "Button").classList.add('active');
}