let ip = "";
$(document).ready(async () => {
    await $.get("https://freegeoip.app/json/", (data) => {
        console.log(data);
        ip = data['ip'];
        $("#IP").val(ip);
    });
})