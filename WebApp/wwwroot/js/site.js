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
    console.log("Full data object:", data);


    toggleModal(modalId, closeButtonId, formId)
    // set the data in the inputs
    const modal = document.querySelector(`#${modalId}`);
    // Change modal h3 to Edit Member
    const modalTitle = document.querySelector("#modal-title");
    const photoPlaceholder = document.querySelector("#photo-placeholder");
    const img = document.querySelector("#member-img");
    const email = document.querySelector("#Email");
    console.log(email)
    const button = document.querySelector("#btn-member");
    if (img) {
        img.src = `/uploads/${data.AvatarUrl}`;
        img.alt = `${data.FirstName} ${data.LastName}`;
        img.classList.remove("d-none");
        photoPlaceholder.classList.add("d-none");
    }
    if (email) {
        email.readOnly = true;
    }
    if (modalTitle) {
        modalTitle.innerHTML = "Edit Member";
    }
    if (button) {
        button.innerHTML = "Save"
    }

    const inputs = modal.querySelectorAll("input, select, textarea");
    console.log("Found inputs:", inputs.length);

    inputs.forEach(input => {
        const name = input.getAttribute("name");
        const id = input.getAttribute("id");
        console.log(`Input: id=${id}, name=${name}, value=${input.value}`);
        if (data[name]) {
            console.log(`  Setting ${name} to ${data[name]}`);
            input.value = data[name];
        } else {
            console.log(`  No matching data property for ${name}`);
            // Check for camelCase version
            const camelCaseName = name.charAt(0).toLowerCase() + name.slice(1);
            if (data[camelCaseName]) {
                console.log(`  Found camelCase match: ${camelCaseName} = ${data[camelCaseName]}`);
                input.value = data[camelCaseName];
            }
        }
    });

}

function toggleProjectModal(modalId, closeButtonId, formId) {
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
            const img = document.querySelector("#project-img");
            const button = document.querySelector("#btn-project");
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
                img.alt = "Project photo placeholder"
            }
            if (button) {
                button.innerHTML = "Create"
            }
            if (modalTitle) {
                modalTitle.innerHTML = "AddProject"
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

function editProject(button) {
    const projectData = {
        Id: button.getAttribute('data-project-id'),
        ProjectPhotoUrl: button.getAttribute('data-project-photo'),
        ProjectName: button.getAttribute('data-project-name'),
        ClientName: button.getAttribute('data-client-name'),
        Description: button.getAttribute('data-description'),
        StartDate: button.getAttribute('data-start-date'),
        EndDate: button.getAttribute('data-end-date'),
        Budget: button.getAttribute('data-budget')
    };

    // Handle members separately to avoid JSON parse issues
    try {
        const membersBase64 = button.getAttribute('data-members');
        if (membersBase64) {
            // Decode the Base64 string to get the JSON
            const jsonString = atob(membersBase64);
            projectData.Members = JSON.parse(jsonString);
        }
    } catch (e) {
        console.error("Error parsing members JSON:", e);
        projectData.Members = [];
    }

    toggleProjectModalWithData('ProjectModal', 'btn-close', 'project-form', projectData);
}

function toggleProjectModalWithData(modalId, closeButtonId, formId, data) {
    toggleProjectModal(modalId, closeButtonId, formId)
    const modal = document.querySelector(`#${modalId}`);
    const modalTitle = document.querySelector("#modal-title");
    const photoPlaceholder = document.querySelector("#photo-placeholder");
    const img = document.querySelector("#project-img");
    const button = document.querySelector("#btn-project");

    if (img) {
        img.src = `/uploads/${data.ProjectPhotoUrl}`;
        img.alt = `${data.FirstName} ${data.LastName}`;
        img.classList.remove("d-none");
        photoPlaceholder.classList.add("d-none");
    }
    if (modalTitle) {
        modalTitle.innerHTML = "Edit Project";
    }
    if (button) {
        button.innerHTML = "Save"
    }

    const inputs = modal.querySelectorAll("input, select, textarea, date");

    inputs.forEach(input => {
        const name = input.getAttribute("name");
        if (name === "member-input") {

            window.selectedMembers = [];

            if (data.Members && Array.isArray(data.Members)) {
                data.Members.forEach(member => {
                    if (!window.selectedMembers.some(m => m.id === member.id)) {
                        window.selectedMembers.push(member);
                    }
                });
            }

            if (typeof window.renderSelectedMembers === "function") {
                window.renderSelectedMembers();
                window.updateHiddenInputs();
            }
        } else {
            if (data[name]) {
                input.value = data[name];
            }
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

            const menu = document.querySelector(`#card-menu-${memberId}`)
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