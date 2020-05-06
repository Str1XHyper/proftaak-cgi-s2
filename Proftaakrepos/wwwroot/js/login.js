let ip = "";
$(document).ready(async () => {
    await $.get("http://free.ipwhois.io/json/", (data) => {
        console.log(data);
        ip = data['ip'];
        $("#IP").val(ip);
    });
})