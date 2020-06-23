$(document).ready(() => {
    UpdateTable();
});

function UpdateTable() {
    $.get("/HoursWorked/UpdateTable?Date=" + $('#date').val() + "&filter=" + "week", (data) => {
        var obj = JSON.parse(data);
        for (var i = 1; i <= 7; i++) {
            $('#t' + i.toString()).text(obj.Total[i - 1] + ".00");
            $('#f' + i.toString()).text(obj.FurloughTime[i - 1] + ".00");
            $('#a' + i.toString()).text(obj.ActiveTime[i - 1] + ".00");
            $('#sb' + i.toString()).text(obj.StandByTime[i - 1] + ".00");
        }
        $('#tt').text(calcTotal(obj.Total));
        $('#ft').text(calcTotal(obj.FurloughTime));
        $('#at').text(calcTotal(obj.ActiveTime));
        $('#sbt').text(calcTotal(obj.StandByTime));
    })
}
function calcTotal(data) {
    var total = 0;
    
    data.forEach(day => {
        total += day;
    });
    return total + ".00";
}