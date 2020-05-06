let minReq;

$("#newpass1").keyup(async function () {
    await checkPass($("#newpass1"));
    if ($("#newpass2").val() != "") {
        await checkIfEqual($("#newpass1"), $("#newpass2"));
    }
    if ($("#newpass1").val() == "") {
        reset($("#newpass1"));
    }
});

$("#newpass2").keyup(async function () {
    await checkIfEqual($("#newpass1"), $("#newpass2"));
    if ($("#newpass2").val() == "") {
        reset($("#newpass2"));
    }
});

async function checkPass(el) {
    let status = false;
    if (el.val() == "")
        return status;
    await $.post("../Authentication/CheckPassword", { password: el.val() }, (data) => {
        minReq = "<b>Wachtwoord vereisten: </b><br>";
        minReq += "Minimum lengte: " + data["minLength"] + "<br>";
        if (data["numberRequired"]) {
            minReq += "Een nummer. (0-9)<br>"
        }
        if (data["specialCharRequired"]) {
            minReq += "Een speciaal karakter. (#?!@$%^&*-)<br>"
        }
        if (data["upperRequired"]) {
            minReq += "Een hoofdletter. <br>";
        }
        if (data["lowerRequired"]) {
            minReq += "Een kleine letter. <br>";
        }
        minReq += ""
        console.log(data);
        if (data["lower"] && data["upper"] && data["specialChar"] && data["number"] && data["length"]) {
            el.removeClass('is-invalid');
            el.addClass('is-valid');
            status = true;
        } else {
            el.removeClass('is-valid');
            el.addClass('is-invalid');
            alert(true);
            disableButton();
            status = false;
        }
    });
    return status;
}

async function checkIfEqual(el1, el2) {
    if (el1.val() === el2.val() && await checkPass(el1) === true) {
        el2.removeClass('is-invalid');
        el2.addClass('is-valid');
        alert(false);
        enableButton();
    } else {
        el2.removeClass('is-valid');
        el2.addClass('is-invalid');
        alert(true);
        disableButton();
    }
}

function reset(el) {
    el.removeClass('is-valid');
    el.removeClass('is-invalid');
    if ($("#newpass2").val() == "" && $("#newpass1").val() == "") {
        alert(false);
        enableButton();
    }
}

function alert(status) {
    if (status) {
        $("#PassAlert").html(minReq);
        $("#PassAlert").show();
    } else {
        $("#PassAlert").hide();
    }
}

function disableButton() {
    let button = $("#submit");
    button.prop("disabled", true);
}

function enableButton() {
    let button = $("#submit");
    button.prop("disabled", false);
}