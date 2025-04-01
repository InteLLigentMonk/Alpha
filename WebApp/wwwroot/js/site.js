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


function toggleMemberCardMenu() {
    const menuButtons = document.querySelectorAll(".btn-card-settings");

    if (!menuButtons || menuButtons.length === 0) {
        console.log("stop here");
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
                    }, 0);

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