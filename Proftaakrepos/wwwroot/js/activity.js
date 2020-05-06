let alert = $("#IPAlert");

$(document).ready(() => {
    $.get("http://free.ipwhois.io/json/", (data) => {
        alert.html("<center>Uw IP adres is: <b>" + data["ip"] + "</b></center>");
    })
})