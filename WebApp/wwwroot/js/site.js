// ------------ Utility functions ------------

function togglePasswordVisibility(id) {
    const passwordField = document.querySelector(`#${id}`);
    const togglePasswordIcon = document.querySelector(`#toggle${id}Icon`);
    const isPasswordVisible = passwordField.type === 'text';

    passwordField.type = isPasswordVisible ? 'password' : 'text';
    togglePasswordIcon.classList.replace(isPasswordVisible ? 'fa-eye-slash' : 'fa-eye', isPasswordVisible ? 'fa-eye' : 'fa-eye-slash');
    togglePasswordIcon.alt = isPasswordVisible ? 'Show Password' : 'Hide Password';
}



function toggleAddProjectModal() {
    const modal = document.getElementById("newProjectModal");
    const showButton = document.querySelector("#btn-new-project");
    const closeButton = document.getElementById("btn-new-project-close");

    showButton.addEventListener("click", () => {
        modal.style.display = "block";
        initializeValidation("#add-project-form");
    });

    closeButton.addEventListener("click", () => {
        modal.style.display = "none";
    });

    window.addEventListener("click", (event) => {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });

    const form = document.querySelector("#add-project-form");
    form.addEventListener("submit", function (event) {
        console.log(selectedMembers)
    });
}



function toggleEditProjectMenu(id) {
    const menu = document.querySelector(".project-menu-card");
    const showButton = document.querySelector("#btn-new-project");
    const closeButton = document.getElementById("btn-new-project-close");

    showButton.addEventListener("click", () => {
        modal.style.display = "block";
        initializeValidation("#add-project-form");
    });

    closeButton.addEventListener("click", () => {
        modal.style.display = "none";
    });

    window.addEventListener("click", (event) => {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });

    const form = document.querySelector("#add-project-form");
    form.addEventListener("submit", function (event) {
        console.log(selectedMembers)
    });
}
// ------------ Utility functions ------------


// ------------ Darkmode ------------
function initializeDarkMode() {
    var savedDarkMode = localStorage.getItem("darkMode");
    console.log(savedDarkMode);
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
    toggleAddProjectModal()
});
// ------------ Darkmode ------------