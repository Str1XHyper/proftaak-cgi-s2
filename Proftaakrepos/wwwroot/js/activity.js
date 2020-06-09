let alert = $("#IPAlert");

$(document).ready(() => {
    $.get("https://freegeoip.app/json/", (data) => {
        alert.html("<center>IP: <b>" + data["ip"] + "</b></center>");
    })
})