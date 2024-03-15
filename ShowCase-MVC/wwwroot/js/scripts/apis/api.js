const form = document.getElementById("contact-form");
const formError = document.querySelector("#sendButton+ span.error");

const email = document.getElementById("email");
const emailError = document.querySelector("#email + span.error");

const subject = document.getElementById("subject");
const subjectError = document.querySelector("#subject + span.error");

const Message = document.getElementById("massege");
const massegeError = document.querySelector("#massege + span.error");

const firstname = document.getElementById("firstname");
const firstnameError = document.querySelector("#firstname + span.error");

const lastname = document.getElementById("lastname");
const lastnameError = document.querySelector("#lastname + span.error");

const phone = document.getElementById("phone");
const phoneError = document.querySelector("#phone + span.error");

const sendButton = document.getElementById("sendButton")
const sendButtonText = document.querySelector("#sendButton .text")


email.addEventListener("input", (event) => { validateEmail(80) });
subject.addEventListener("input", (event) => { validatefield(subject, subjectError, 200) });
Message.addEventListener("input", (event) => { validatefield(Message, massegeError, 600) });
firstname.addEventListener("input", (event) => { validatefield(firstname, firstnameError, 60) });
lastname.addEventListener("input", (event) => { validatefield(lastname, lastnameError, 60) });
phone.addEventListener("input", (event) => { validatePhone(20) });

function validateForm() {
    if (!validatefield(firstname, firstnameError, 60) || !validatefield(lastname, lastnameError, 60) || !validatePhone(20) || !validateEmail(80) || !validatefield(subject, subjectError, 200) || !validatefield(Message, massegeError, 600)) {
        return false
    } else {
        return true
    }
}



function validateEmail(MaxwordCount) {
    let regex = new RegExp(/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/);
    if (email.validity.valueMissing) {
        // If the field is empty,
        // display the following error message.
        emailError.textContent = "";
        emailError.textContent = "vul een e-mail aderess in";
        return false

    } else if (email.validity.typeMismatch || !regex.test(email.value)) {
        // If the field doesn't contain an email address,
        // display the following error message.
        emailError.textContent = "";
        emailError.textContent = "De ingevoerde waarde moet een e-mailadres zijn.";
        return false
    } else if (email.validity.tooShort) {
        // If the data is too short,
        // display the following error message.
        emailError.textContent = "";
        emailError.textContent = `E-mail zou op zijn minst ${email.minLength} karakters moeten zijn; jij hebt er ${email.value.length}.`;
        return false
    } else if (email.value.length > MaxwordCount) {
        emailError.textContent = "";
        emailError.textContent = `je kunt maar ${MaxwordCount} tekens gebruiken`;
        return false
    }
    emailError.textContent = "";
    return true

}

function validatefield(field, error, MaxwordCount) {
    if (field.validity.valueMissing) {
        error.textContent = "";
        error.textContent = `vul een ${field.labels[0].textContent} in.`;
        return false
    } else if (field.value.length > MaxwordCount) {
        error.textContent = "";
        error.textContent = `je kunt maar ${MaxwordCount} tekens gebruiken`;
        return false
    }
    error.textContent = "";
    return true
}

function validatePhone(MaxwordCount) {
    let regex = new RegExp(/(^\+[0-9]{2}|^\+[0-9]{2}\(0\)|^\(\+[0-9]{2}\)\(0\)|^00[0-9]{2}|^0)([0-9]{9}$|[0-9\-\s]{10}$)/);
    if (phone.validity.valueMissing) {
        phoneError.textContent = "";
        phoneError.textContent = `vul een ${phone.labels[0].textContent} in.`;
        return false
    } else if (!regex.test(phone.value)) {
        phoneError.textContent = "";
        phoneError.textContent = "Invalid phone number";
        return false
    } else if (phone.value.length > MaxwordCount) {
        phoneError.textContent = "";
        phoneError.textContent = `je kunt maar ${MaxwordCount} tekens gebruiken`;
        return false
    }
    phoneError.textContent = "";
    return true
}

sendButton.addEventListener("click", async (event) => {
    // Then we prevent the form from being sent by canceling the event
    event.preventDefault();

    if (!validateForm()) {
        verzonden(false);
        return
    }

    if (!sendButton.classList.contains("active")) {
        sendButton.classList.add("active")

        grecaptcha.ready(function () {
            grecaptcha.execute('6LfSDHgpAAAAAK_I_INXeLkkkIhYWS__b4akWAkE', { action: 'submit' }).then(async function (token) {
                const formSubmit = document.getElementById('submit');
                try {
                    const response = await fetch('https://localhost:7078/api/Captcha', {
                        method: "POST",
                        body: JSON.stringify({
                            response: token
                        }),
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        }
                    });
                    const result = await response.json();

                    if (result.score > 0.8) {
                        try {
                            let response = await fetch('https://localhost:7078/api/Contact', {
                                method: 'POST',
                                body: JSON.stringify({
                                    email: email.value,
                                    subject: subject.value,
                                    firstname: firstname.value,
                                    lastname: lastname.value,
                                    phone: phone.value,
                                    Message: Message.value
                                }),
                                headers: {
                                    'Accept': 'application/json',
                                    'Content-Type': 'application/json'
                                }
                            });

                            let data = await response.json();
                            console.log(data);

                            verzonden(data.success, data.message);
                        } catch (error) {
                            verzonden(false, "server offline");
                        }
                    }
                }
                catch (error) {
                    verzonden(false, "server offline " + error);
                }
            });
        });
    }
});

function verzonden(bool, message) {

    if (bool) {
        sendButton.classList.add("sent");
        sendButtonText.innerText = "verzonden"
    }
    else {
        sendButton.classList.add("notSent");
        sendButtonText.innerText = "niet verzonden"
        formError.textContent = "";
        formError.textContent = message;
    }

    setTimeout(() => {
        if (bool) {
            sendButtonText.innerText = "verzend"
            sendButton.classList.remove("active");
            sendButton.classList.remove("sent");
        }
        else {
            sendButtonText.innerText = "try again later"
            sendButton.classList.remove("active");
            sendButton.classList.remove("notSent");
        }

        form.reset();
    }, 2000);
}