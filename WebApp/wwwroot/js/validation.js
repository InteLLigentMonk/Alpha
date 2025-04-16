const validateField = (field, checkRelatedField = true) => {
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

    if ((field.name === "EndDate" || field.name === "StartDate") && !errorMessage) {
        const startDateField = document.querySelector('input[name="StartDate"]');
        const endDateField = document.querySelector('input[name="EndDate"]');

        if (startDateField && endDateField) {
            const startDate = new Date(startDateField.value);
            const endDate = new Date(endDateField.value);

            // Check if dates are valid before comparing
            if (!isNaN(endDate.getTime()) && !isNaN(startDate.getTime())) {
                const datesAreValid = endDate >= startDate;

                // Clear both fields or set error messages as appropriate
                if (datesAreValid) {
                    // If dates are valid, always clear both fields regardless of which one changed
                    if (checkRelatedField) {
                        // Clear related field error too
                        const relatedField = field.name === "StartDate" ? endDateField : startDateField;
                        const relatedErrorSpan = document.querySelector(`span[data-valmsg-for='${relatedField.name}']`);

                        if (relatedErrorSpan) {
                            relatedField.classList.remove("input-validation-error");
                            relatedErrorSpan.classList.remove("field-validation-error");
                            relatedErrorSpan.classList.add("field-validation-valid");
                            relatedErrorSpan.textContent = "";
                        }
                    }
                } else {
                    // Invalid date relationship
                    if (field.name === "EndDate") {
                        errorMessage = "End date must be after the start date";
                    } else {
                        errorMessage = "Start date must be before the end date";
                    }

                    // Also validate the related date field to show an error
                    if (checkRelatedField) {
                        const relatedField = field.name === "StartDate" ? endDateField : startDateField;
                        validateField(relatedField, false); // Pass false to prevent infinite recursion
                    }
                }
            }
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

    const fields = form.querySelectorAll("input[data-val='true'], textarea[data-val='true']");

    fields.forEach(field => {
        const newField = field.cloneNode(true);
        field.parentNode.replaceChild(newField, field);

        newField.addEventListener("input", function () {
            validateField(newField);
        });
        if (newField.type === "checkbox") {
            newField.addEventListener("change", function () {
                validateField(newField);
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

