//function checkPass(input) {
//    var newpass = document.getElementById('newpass1');
//    var newpass2 = document.getElementById("newpass2");
//    var passAlert = document.getElementById("PassAlert");
//    var regex = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/g;
//    if (newpass.value.length > 0) {
//        passAlert.style.display = "block";
//    } else {
//        passAlert.style.display = "none";
//        newpass.classList.remove('is-invalid');
//        newpass2.classList.remove('is-invalid');
//    }
//    if (input == 'pw1') {
//        if (newpass.value.match(regex)) {
//            console.log("regex Check")
//            newpass.classList.add('is-valid');
//            newpass.classList.remove('is-invalid');
//        } else {
//            newpass.classList.remove('is-valid');
//            newpass.classList.add('is-invalid');
//        }
//    } else {
//        if (newpass.value.match(regex) && newpass.value == newpass2.value) {
//            console.log("regex Check")
//            newpass2.classList.add('is-valid');
//            newpass2.classList.remove('is-invalid');
//        } else {
//            newpass2.classList.remove('is-valid');
//            newpass2.classList.add('is-invalid');
//        }
//    }

//}

$("#newpass1").keyup(function () {
    checkPass($("#newpass1"));
    if ($("#newpass2").val() != "") {
        checkIfEqual($("#newpass1"), $("#newpass2"));
    }
});

$("#newpass2").keyup(function () {
    checkIfEqual($("#newpass1"), $("#newpass2"));
});

async function checkPass(el) {
    let status = false;
    if (el.val() == "")
        return status;
    await $.post("../Authentication/CheckPassword", { password: el.val() }, (data) => {
        if (data) {
            el.removeClass('is-invalid');
            el.addClass('is-valid');
            status = true;
        } else {
            el.removeClass('is-valid');
            el.addClass('is-invalid');
            status = false;
        }
    });
    return status;
}

async function checkIfEqual(el1, el2) {
    if (el1.val() === el2.val() && await checkPass(el1) === true) {
        el2.removeClass('is-invalid');
        el2.addClass('is-valid');
    } else {
        el2.removeClass('is-valid');
        el2.addClass('is-invalid');
    }
}