let alert = $("#IPAlert");

$(document).ready(() => {
    $.get("https://freegeoip.app/json/", (data) => {
        alert.html("<center>Uw IP adres is: <b>" + data["ip"] + "</b></center>");
    })
})