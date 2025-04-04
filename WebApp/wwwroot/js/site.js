// ------------ Utility functions ------------

function togglePasswordVisibility(id) {
    const passwordField = document.querySelector(`#${id}`);
    const togglePasswordIcon = document.querySelector(`#toggle${id}Icon`);
    const isPasswordVisible = passwordField.type === 'text';

    passwordField.type = isPasswordVisible ? 'password' : 'text';
    togglePasswordIcon.classList.replace(isPasswordVisible ? 'fa-eye-slash' : 'fa-eye', isPasswordVisible ? 'fa-eye' : 'fa-eye-slash');
    togglePasswordIcon.alt = isPasswordVisible ? 'Show Password' : 'Hide Password';
}



function toggleModal(modalId, closeButtonId, formId) {
    const modal = document.querySelector(`#${modalId}`);
    const closeButton = document.querySelector(`#${closeButtonId}`);

    if (!modal || !closeButton) {
        return;
    }

    modal.style.display = "block";

    if (formId) {
        initializeValidation(`#${formId}`);
    }

    const closeModal = () => {
        modal.style.display = "none";

        if (formId) {
            const form = document.querySelector(`#${formId}`);
            const hiddenInput = document.querySelector("#Id");
            const img = document.querySelector("#member-img");
            const button = document.querySelector("#btn-member");
            const modalTitle = document.querySelector("#modal-title");
            const photoPlaceholder = document.querySelector("#photo-placeholder");
            if (form) {
                form.reset();
            }
            if (hiddenInput) {
                hiddenInput.value = "";
            }
            if (img) {
                img.classList.add("d-none")
                img.src = "#"
                img.alt = "Avatar placeholder"
            }
            if (button) {
                button.innerHTML = "Add Member"
            }
            if (modalTitle) {
                modalTitle.innerHTML = "New Member"
            }
            if (photoPlaceholder) {
                photoPlaceholder.classList.remove("d-none")
            }
        }

        closeButton.removeEventListener("click", closeModal);
        window.removeEventListener("click", windowCloseModal);
    }

    const windowCloseModal = (e) => {
        if (e.target == modal) {
            closeModal();
        }
    }
    closeButton.addEventListener("click", closeModal);
    window.addEventListener("click", windowCloseModal);
}

function toggleModalWithData(modalId, closeButtonId, formId, data) {
    toggleModal(modalId, closeButtonId, formId)
    // set the data in the inputs
    const modal = document.querySelector(`#${modalId}`);
    // Change modal h3 to Edit Member
    const modalTitle = document.querySelector("#modal-title");
    const photoPlaceholder = document.querySelector("#photo-placeholder");
    const img = document.querySelector("#member-img");
    const button = document.querySelector("#btn-member");
    if (img) {
        img.src = `/uploads/${data.AvatarUrl}`;
        img.alt = `${data.FirstName} ${data.LastName}`;
        img.classList.remove("d-none");
        photoPlaceholder.classList.add("d-none");
    }
    if (modalTitle) {
        modalTitle.innerHTML = "Edit Member";
    }
    if (button) {
        button.innerHTML = "Save"
    }


    const inputs = modal.querySelectorAll("input, select, textarea");
    inputs.forEach(input => {
        const name = input.getAttribute("name");
        if (data[name]) {
            input.value = data[name];
        }
    });

}


function toggleMemberCardMenu() {
    const menuButtons = document.querySelectorAll(".btn-card-settings");

    if (!menuButtons || menuButtons.length === 0) {
        return;
    }

    menuButtons.forEach(button => {
        button.addEventListener('click', (event) => {
            const memberId = button.getAttribute("data-member-id");

            const menu = document.querySelector(`#member-card-menu-${memberId}`)
            if (menu) {
                if (menu.style.display === "none" || menu.style.display === "") {
                    menu.style.display = "block";

                    const closeMenuOnClickOutside = (e) => {
                        if (!menu.contains(e.target) && e.target !== button) {
                            menu.style.display = "none";
                            document.removeEventListener("click", closeMenuOnClickOutside);
                        }
                    };

                    setTimeout(() => {
                        document.addEventListener("click", closeMenuOnClickOutside);
                    }, 500);

                    console.log(`Member menu opened for ID: ${memberId}`);
                } else {
                    menu.style.display = "none";
                }
            }
            event.stopPropagation();
        })
    })

}
// ------------ Utility functions ------------


// ------------ Darkmode ------------
function initializeDarkMode() {
    var savedDarkMode = localStorage.getItem("darkMode");
    if (savedDarkMode === null) {
        if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
            localStorage.setItem("darkMode", "true")
            document.documentElement.classList.add("dark");
        } else {
            localStorage.setItem("darkMode", "false")
        }
    } else {
        const selection = savedDarkMode === "true";
        if (selection) {
            document.documentElement.classList.add("dark");
        }
    }
}

function switchDarkMode() {
    const darkModeToggle = document.querySelector("#flexSwitchCheckChecked");
    const savedDarkMode = localStorage.getItem("darkMode") === "true";

    if(darkModeToggle){
        darkModeToggle.checked = savedDarkMode;

        darkModeToggle.addEventListener("change", () => {
            if(darkModeToggle.checked) {
                document.documentElement.classList.add("dark");
                darkModeToggle.checked = true;
                localStorage.setItem("darkMode", "true");
            } else {
                document.documentElement.classList.remove("dark");
                darkModeToggle.checked = false;
                localStorage.setItem("darkMode", "false");
            }
        });
    }
}

document.addEventListener("DOMContentLoaded", () => {
    switchDarkMode();
    toggleMemberCardMenu()
});
// ------------ Darkmode ------------