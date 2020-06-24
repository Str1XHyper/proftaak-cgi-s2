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
function UpdateClaimedHoursTable() {
    $("#tableBody").find("tr:not(:last)").remove();
    $.get("/HoursWorked/UpdateRows?date=" + $('#date').val(), (data) => {
        var jsonData = JSON.parse(data);
        console.log(jsonData);
        jsonData.forEach(row => {
            console.log(row.Dates);
            //Xander approved spaghet below
            var newRowContent;
            if (row.Type == "Incidenten") {
                newRowContent = `<tr>
            <th><input disabled class="form-control" type="date" value="`+ row.Dates + `"/></th>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.Start + `"/></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.End + `"/></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.TotalTime + `" /></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.OverTime + `" /></td>
                <td><label class="switch"><input checked disabled type="checkbox"><span class="slider round"></span></label></td>
                <td><input type="text" class="custom-select" disabled value="`+ row.Type + `"/></td>
            </tr>`;
            }
            else {
                newRowContent = `<tr>
            <th><input disabled class="form-control" type="date" value="`+ row.Dates + `"/></th>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.Start + `"/></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.End + `"/></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.TotalTime + `" /></td>
                <td><input disabled class="form-control" type="text" data-mask="00:00" value="`+ row.OverTime + `" /></td>
                <td><label class="switch"><input disabled type="checkbox"><span class="slider round"></span></label></td>
                <td><input type="text" class="custom-select" disabled value="`+ row.Type + `"/></td>
            </tr>`;
            }
            $("#tableBody").prepend(newRowContent);
        })
    })

}
function calcTotal(data) {
    var total = 0;

    data.forEach(day => {
        total += day;
    });
    return total + ".00";
}