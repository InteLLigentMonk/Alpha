const validateField = (field) => {
    let errorSpan = document.querySelector(`span[data-valmsg-for='${field.name}']`);
    if (!errorSpan) return;

    let errorMessage = "";
    let value = field.type === "checkbox" ? field.checked : field.value.trim();

    if (field.hasAttribute("data-val-required") && (value === "" || value === false)) {
        errorMessage = field.getAttribute("data-val-required");
    }

    if (field.hasAttribute("data-val-regex") && value !== "" && field.type !== "checkbox") {
        let pattern = new RegExp(field.getAttribute("data-val-regex-pattern"));
        if (!pattern.test(value)) {
            errorMessage = field.getAttribute("data-val-regex");
        }
    }

    if (errorMessage) {
        field.classList.add("input-validation-error");
        errorSpan.classList.remove("field-validation-valid");
        errorSpan.classList.add("field-validation-error");
        errorSpan.textContent = errorMessage;
        return false;
    } else {
        field.classList.remove("input-validation-error");
        errorSpan.classList.remove("field-validation-error");
        errorSpan.classList.add("field-validation-valid");
        errorSpan.textContent = "";
        return true;
    }
};



const validateForm = (form) => {
    const fields = form.querySelectorAll("input[data-val='true'], textarea[data-val='true']");
    let isFormValid = true;

    fields.forEach(field => {
        if (!validateField(field)) {
            console.log(`${field} not valid`)
            isFormValid = false;
        }
    });

    return isFormValid;
};



const initializeValidation = (formSelector) => {
    const form = document.querySelector(formSelector);

    if (!form) {
        console.log("Form not found:", formSelector);
        return;
    }

    console.log("Initializing validation for form:", formSelector);
    const fields = form.querySelectorAll("input[data-val='true'], textarea[data-val='true']");

    fields.forEach(field => {
        field.addEventListener("input", function () {
            validateField(field);
        });
        if (field.type === "checkbox") {
            field.addEventListener("change", function () {
                validateField(field);
            });
        }
    });

    form.addEventListener("submit", function (e) {
        if (!validateForm(form)) {
            e.preventDefault();
        }
    });
};




document.addEventListener('DOMContentLoaded', function () {
    initializeValidation('form');
});

