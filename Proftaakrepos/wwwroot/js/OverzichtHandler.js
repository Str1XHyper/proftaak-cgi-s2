function addRow() {
    var count = parseInt($("#amount").val());
    var newRowContent = `<tr id="${count}"><th><input id="datum-${count}" class="form-control" type="date" /></th><td><input onchange="CalcTotalTime(${count})" id="start-${count}" class="form-control" type="time" /></td><td><input onchange="CalcTotalTime(${count})" id="eind-${count}" class="form-control" type="time" /></td><td><input onchange="CalcTotalTime(${count})" id="pauze-${count}" class="form-control" type="time" /></td><td><input id="totaal-${count}" disabled class="form-control" type="time" value="00:00" /></td><td><input id="overuren-${count}" class="form-control" type="time" value="00:00" /></td><td><input id="ziekte-${count}" class="form-control" type="time" value="00:00" style="width: 150px; display: inline-block" /> <button class="btn btn-outline-danger" id="sendTimeSheet" onclick="removeRow(${count})" style="border-radius: 100%; width: 35px; height: 35px;"><i class="fas fa-times ml-0"></i></button></td></tr><input type="hidden" value="${count}" />`;
    $("#tableBody").append(newRowContent);
    var help = parseInt($("#amount").val());
    $("#amount").val(help + 1);
}

function CalcTotalTime(id) {
    $("#sendTimeSheet").prop("disabled", false);
    $("#error").html("");
    if ($("#start-" + id).val() != "" && $("#eind-" + id).val() != "" && $("#pauze-" + id).val() != "") {
        var startTijd = $("#start-" + id).val();
        var eindTijd = $("#eind-" + id).val();
        var pauzeTijd = $("#pauze-" + id).val();
        startTijd = startTijd.split(":");
        eindTijd = eindTijd.split(":");
        pauzeTijd = pauzeTijd.split(":");
        var startDate = new Date(0, 0, 0, startTijd[0], startTijd[1], 0)
        var eindDate = new Date(0, 0, 0, eindTijd[0], eindTijd[1], 0)
        var pauzeDate = new Date(0, 0, 0, pauzeTijd[0], pauzeTijd[1], 0)
        var startMinuten = startDate.getHours() * 60 + startDate.getMinutes();
        var eindMinuten = eindDate.getHours() * 60 + eindDate.getMinutes();
        var pauzeMinuten = pauzeDate.getHours() * 60 + pauzeDate.getMinutes();
        var totaleTijd = eindMinuten - startMinuten - pauzeMinuten;
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

function removeRow(id) {
    $("#" + id).remove();
}