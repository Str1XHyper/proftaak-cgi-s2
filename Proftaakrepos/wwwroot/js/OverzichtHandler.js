﻿function addRow() {
    var count = parseInt($("#amount").val());
    var newRowContent = `<tr id="${count}">
<th><input id="datum-${count}" class="form-control" type="date" name="Dates" /></th>
                <td><input onchange="CalcTotalTime(${count})" id="start-${count}" class="form-control" type="text" data-mask="00:00" name="Start" /></td>
                <td><input onchange="CalcTotalTime(${count})" id="eind-${count}" class="form-control" type="text" data-mask="00:00" name="End" /></td>
                <td><input id="totaal-${count}" disabled class="form-control" type="text" data-mask="00:00" value="00:00" /></td>
                <td><input id="overuren-${count}" class="form-control" type="text" data-mask="00:00" value="00:00" name="OverTime" /></td>
                <td><label class="switch"><input name="Type" id="incidentswitch-${count}" onchange="UpdateIncidentState(${count})" type="checkbox"><span class="slider round"></span></label><input type="hidden" value="off" name="Type" id="hiddenswitch-${count}" /></td>
                <td><select class="custom-select" disabled id="incidentname-${count}"></select></td>
                <td><div class="float-right mr-2"><button class="btn btn-outline-danger" id="sendTimeSheet" onclick="removeRow(${count})" style="border-radius: 100%; width: 35px; height: 35px;" type="button"><i class="fas fa-times ml-0"></i></button></div></td>
</tr>`;
    $("#tableBody").append(newRowContent);
    getIncidentIDthingy(count)
    var help = parseInt($("#amount").val());
    $("#amount").val(help + 1);
}
function UpdateIncidentState(id) {
    if ($("#incidentswitch-" + id).prop("checked")) {
        $("#incidentname-" + id).prop("disabled", false);
        $("#hiddenswitch-" + id).prop("disabled", true);
    }
    else {
        $("#incidentname-" + id).prop("disabled", true);
        $("#hiddenswitch-" + id).prop("disabled", false);
    }
}

function CalcTotalTime(id) {
    console.log($("#eind-" + id).val())
    $("#sendTimeSheet").prop("disabled", false);
    $("#error").html("");
    if ($("#start-" + id).val() != "" && $("#eind-" + id).val() != "") {
        var startTijd = $("#start-" + id).val();
        var eindTijd = $("#eind-" + id).val();
        startTijd = startTijd.split(":");
        eindTijd = eindTijd.split(":");
        var startDate = new Date(0, 0, 0, startTijd[0], startTijd[1], 0)
        var eindDate = new Date(0, 0, 0, eindTijd[0], eindTijd[1], 0)
        var startMinuten = startDate.getHours() * 60 + startDate.getMinutes();
        var eindMinuten = eindDate.getHours() * 60 + eindDate.getMinutes();
        var totaleTijd = eindMinuten - startMinuten;
        var minuten = totaleTijd % 60;
        if (totaleTijd < 0) {
            $("#error").html("<strong>Urentotaal is negatief!</strong>");
            $("#sendTimeSheet").prop("disabled", true);
            $("#totaal-" + id).val("--:--");
        } else {
            var uren = (totaleTijd - minuten) / 60;
            if (uren < 10 && uren >= 0) {
                uren = "0" + uren;
            }
            if (minuten < 10 && minuten >= 0) {
                minuten = "0" + minuten;
            }
            $("#totaal-" + id).val(uren + ":" + minuten);
        }
    }
}
function getIncidentIDthingy(id) {
    $.get("../api/incidenten", function (data) {
        populateIncidentNameSelect(data, id)
    });
}
function populateIncidentNameSelect(data, id) {
    var $el = $("#incidentname-" + id);
    $el.empty();
    $.each(data, function (value, key) {
        $el.append($("<option></option>")
            .attr("value", key).text(key));
    });
}
function removeRow(id) {
    $("#" + id).remove();
}

window.onload = getIncidentIDthingy(-1);