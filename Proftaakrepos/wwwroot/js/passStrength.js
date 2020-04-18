function checkPass(input) {
    var newpass = document.getElementById('newpass1');
    var newpass2 = document.getElementById("newpass2");
    var passAlert = document.getElementById("PassAlert");
    var regex = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/g;
    if (newpass.value.length > 0) {
        passAlert.style.display = "block";
    } else {
        passAlert.style.display = "none";
        newpass.classList.remove('is-invalid');
        newpass2.classList.remove('is-invalid');
    }
    if (input == 'pw1') {
        if (newpass.value.match(regex)) {
            console.log("regex Check")
            newpass.classList.add('is-valid');
            newpass.classList.remove('is-invalid');
        } else {
            newpass.classList.remove('is-valid');
            newpass.classList.add('is-invalid');
        }
    } else {
        if (newpass.value.match(regex) && newpass.value == newpass2.value) {
            console.log("regex Check")
            newpass2.classList.add('is-valid');
            newpass2.classList.remove('is-invalid');
        } else {
            newpass2.classList.remove('is-valid');
            newpass2.classList.add('is-invalid');
        }
    }
    
}