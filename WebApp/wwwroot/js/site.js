// ------------ Utility functions ------------

//Password Visibility
function togglePasswordVisibility(id) {
    const passwordField = document.querySelector(`#${id}`);
    const togglePasswordIcon = document.querySelector(`#toggle${id}Icon`);
    const isPasswordVisible = passwordField.type === 'text';

    passwordField.type = isPasswordVisible ? 'password' : 'text';
    togglePasswordIcon.classList.replace(isPasswordVisible ? 'fa-eye-slash' : 'fa-eye', isPasswordVisible ? 'fa-eye' : 'fa-eye-slash');
    togglePasswordIcon.alt = isPasswordVisible ? 'Show Password' : 'Hide Password';
}

function initializePhotoUpload(photoInputId) {
    const fileInput = document.getElementById(photoInputId);
    if (!fileInput) return;

    fileInput.addEventListener("change", (e) => {
        const file = e.target.files[0];
        const img = document.querySelector('.upload-image');
        const placeholder = document.querySelector('.photo-placeholder-square');
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                img.src = e.target.result;
                img.classList.remove('d-none');
                placeholder.classList.add('d-none');
            };
            reader.readAsDataURL(file);
        } else {
            img.classList.add('d-none');
            placeholder.classList.remove('d-none');
        }
    });
}

//Relative time

function updateRelativeTime() {
    const elements = document.querySelectorAll('.notification-time');
    const now = new Date();

    elements.forEach(element => {
        const created = new Date(element.getAttribute('data-created'));
        const diff = now - created;
        const diffSeconds = Math.floor(diff / 1000);
        const diffMinutes = Math.floor(diffSeconds / 60);
        const diffHours = Math.floor(diffMinutes / 60);
        const diffDays = Math.floor(diffHours / 24);
        const diffWeeks = Math.floor(diffDays / 7);

        let relativeTime = '';

        if (diffMinutes < 1) {
            relativeTime = 'Just now';
        } else if (diffMinutes < 60) {
            relativeTime = `${diffMinutes} minute${diffMinutes > 1 ? 's' : ''} ago`;
        } else if (diffHours < 2) {
            relativeTime = diffHours + ' hour ago'
        } else if (diffHours < 24) {
            relativeTime = diffHours + ' hours ago'
        } else if (diffDays < 2) {
            relativeTime = diffDays + ' day ago'
        } else if (diffDays < 7) {
            relativeTime = diffDays + ' days ago'
        } else {
            relativeTime = diffWeeks + ' Weeks ago'
        }
        element.textContent = relativeTime;
    })
}


//-------------- Handle Modals------------------

// AJAX request helper function
function makeRequest(url, method, data = null) {
    return new Promise((resolve, reject) => {
        const options = {
            method: method,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        };

        if (data && !(data instanceof FormData)) {
            options.headers['Content-Type'] = 'application/x-www-form-urlencoded';
            options.body = new URLSearchParams(data).toString();
        } else if (data) {
            options.body = data;
        }


        fetch(url, options)
            .then(response => {

                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    return response.json();
                }
                return response.text();
            })
            .then(data => resolve(data))
            .catch(error => reject(error));
    });
}


//Show Modal with Bootstrap
function showModal(){
    const modalElement = document.getElementById('formModal');
    const bootstrapModal = new bootstrap.Modal(modalElement);
    bootstrapModal.show();
}

//Hide Modal with Bootstrap
function hideModal() {
    const modalElement = document.getElementById('formModal');
    const bootstrapModal = new bootstrap.Modal(modalElement);
    if (bootstrapModal) {
        bootstrapModal.hide();
    }
}

// Load the Create form in modal
function loadCreateModal(controller, formId) {
    makeRequest(`/${controller}/Create`, 'GET')
        .then(response => {
            document.getElementById('modal-content').innerHTML = response;
            showModal();
            initializeFormSubmission(formId);
            initializeValidation(`#${formId}`);
            if (controller === "Project") {
                initializePhotoUpload("ProjectPhoto");
                initializeMemberInput(formId);
            }
            if (controller === "Member") {
                initializePhotoUpload("Avatar");
            }
        })
        .catch(error => console.error(`Error loading ${formId} modal:`, error));
}

// Load the Edit form in modal
function loadEditModal(controller, formId, id) {
    makeRequest(`/${controller}/Edit/${id}`, 'GET')
        .then(response => {
            document.getElementById('modal-content').innerHTML = response;
            showModal();
            initializeFormSubmission(formId);
            initializeValidation(`#${formId}`);
            if (controller === "Project") {
                initializePhotoUpload("ProjectPhoto");
                initializeMemberInput(formId);
            }
            if (controller === "Member") {
                initializePhotoUpload("Avatar");
            }
        })
        .catch(error => console.error(`Error loading ${formId} modal:`, error));
}

// Load the Details form in modal
function loadDetailsModal(model, id) {
    makeRequest(`/${model}/Details/${id}`, 'GET')
        .then(response => {
            document.getElementById('modal-content').innerHTML = response;
            showModal();
        })
        .catch(error => console.error(`Error loading details modal:`, error));
}

// Load the Add members form in modal
function loadMembersModal(controller, formId, id) {
    makeRequest(`/${controller}/AddMembers/${id}`, 'GET')
        .then(response => {
            document.getElementById('modal-content').innerHTML = response;
            showModal();
            initializeFormSubmission(formId);
            initializeValidation(`#${formId}`);
            initializeMemberInput(formId);
        })
        .catch(error => console.error(`Error loading ${formId} modal:`, error));
}

// Load the Create form in modal
function loadMemberRoleModal() {
    makeRequest(`/Role/AddMemberToRole`, 'GET')
        .then(response => {
            document.getElementById('modal-content').innerHTML = response;
            showModal();
            initializeFormSubmission("add-member-to-role-form");
            initializeValidation(`#add-member-to-role-form`);
        })
        .catch(error => console.error(`Error loading ${formId} modal:`, error));
}

// Initialize form submission for dynamically loaded forms
function initializeFormSubmission(formId) {
    const form = document.getElementById(formId);
    if (!form) return;

    form.addEventListener('submit', function (event) {
        event.preventDefault();

        if (!validateForm(form)) {
            return;
        }

        const hasFileInputs = form.querySelector('input[type=file]') !== null;
        let formData;

        if (hasFileInputs) {
            formData = new FormData(form);
        } else {
            formData = Object.fromEntries(new FormData(form).entries());

            const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenInput) {
                formData['__RequestVerificationToken'] = tokenInput.value;
            }
        }

        makeRequest(form.action, form.method, formData)
            .then(result => {
                if (typeof result === 'object' && result.success) {
                    hideModal();
                    window.location.reload();
                } else {
                    document.getElementById('modal-content').innerHTML = result;
                    initializeFormSubmission(formId);
                    initializeValidation(`#${formId}`);
                }
            })
            .catch(error => console.error('Error submitting the form:', error))
    });
}


//-------------- Handle MemberInput ------------------

function initializeMemberInput(formId) {
    window.formId = formId
    // Look for hidden data containers that were added via AJAX
    const membersDataElement = document.getElementById("members-data");
    const selectedMembersDataElement = document.getElementById("selected-members-data");

    if (!membersDataElement || !selectedMembersDataElement) {
        console.error("Member data elements not found");
        return;
    }

    try {
        // Parse the JSON from the hidden div content
        // Note: textContent preserves the raw text without HTML parsing
        window.members = JSON.parse(membersDataElement.textContent || '[]');
        window.selectedMembers = JSON.parse(selectedMembersDataElement.textContent || '[]');
        console.log(window.selectedMembers);
    } catch (e) {
        console.error("Error parsing member data:", e);
        window.members = [];
        window.selectedMembers = [];
    }
    const input = document.getElementById("member-input");
    const suggestions = document.getElementById("suggestions");
    const selectedMembersContainer = document.getElementById("selected-members");

    if (!input || !suggestions || !selectedMembersContainer) return;

    // Clear any existing event listeners (important for dynamically loaded content)
    input.removeEventListener("input", handleInput);
    input.addEventListener("input", handleInput);

    // Initial render of selected members
    renderSelectedMembers();
    updateHiddenInputs();
}

function handleInput() {
    const input = document.getElementById("member-input");
    const suggestions = document.getElementById("suggestions");
    const query = input.value.toLowerCase();
    suggestions.innerHTML = "";

    if (query.length === 0) {
        suggestions.style.display = "none";
        return;
    }

    const filteredMembers = window.members.filter(member => {
        if (!member || !member.FirstName) return false;

        const nameMatches = member.FirstName.toLowerCase().includes(query);

        // More robust duplicate detection in filter
        const alreadySelected = window.selectedMembers.some(selected =>
            (selected && member && selected.Id === member.Id) ||
            (selected && member && selected.UserId === member.UserId) ||
            (selected && member && selected.EmailAddress && member.EmailAddress &&
                selected.EmailAddress.toLowerCase() === member.EmailAddress.toLowerCase())
        );

        return nameMatches && !alreadySelected;
    });

    if (filteredMembers.length > 0) {
        suggestions.style.display = "block";
        filteredMembers.forEach(member => {
            const li = document.createElement("li");
            // Use avatarUrl instead of avatar, and concatenate firstName + lastName instead of name
            const displayName = `${member.FirstName || ''} ${member.LastName || ''}`.trim();
            const avatarUrl = member.AvatarUrl || '../icons/avatars/1.svg';

            li.innerHTML = `<img src="/uploads/${avatarUrl}" alt=""> ${displayName}`;
            li.addEventListener("click", () => addMember(member));
            suggestions.appendChild(li);
        });
    } else {
        suggestions.style.display = "none";
    }
}

function addMember(member) {
    const input = document.getElementById("member-input");
    const suggestions = document.getElementById("suggestions");

    // More robust duplicate detection
    const isDuplicate = window.selectedMembers.some(existing =>
        (existing.Id && member.Id && existing.Id === member.Id) ||
        (existing.UserId && member.UserId && existing.UserId === member.UserId) ||
        (existing.EmailAddress && member.EmailAddress &&
            existing.EmailAddress.toLowerCase() === member.EmailAddress.toLowerCase())
    );

    if (!isDuplicate) {
        window.selectedMembers.push(member);
        input.value = "";
        input.focus();
        suggestions.style.display = "none";
        renderSelectedMembers();
        updateHiddenInputs();
    } else {
        console.log("Prevented adding duplicate member:", member);
    }
}

function removeMember(memberId) {
    window.selectedMembers = window.selectedMembers.filter(member => member.Id !== memberId);
    renderSelectedMembers();
    updateHiddenInputs();
}

function renderSelectedMembers() {
    const selectedMembersContainer = document.getElementById("selected-members");
    if (!selectedMembersContainer) return;

    selectedMembersContainer.innerHTML = "";
    window.selectedMembers.forEach(member => {
        const chip = document.createElement("div");
        chip.classList.add("member-chip");

        const displayName = `${member.FirstName || ''} ${member.LastName || ''}`.trim();
        const avatarUrl = member.AvatarUrl || '/images/default-avatar.png';

        chip.innerHTML = `
            <img src="/uploads/${avatarUrl}" alt="">
            ${displayName}
            <span class="remove-btn" onclick="removeMember('${member.Id}')">&times;</span>
        `;
        selectedMembersContainer.appendChild(chip);
    });
}

function updateHiddenInputs() {
    const form = document.getElementById(window.formId);

    if (!form) {
        console.error("Form not found");
        return;
    }

    const existingInputs = form.querySelectorAll("input[name^='Members']");
    existingInputs.forEach(input => input.remove());

    window.selectedMembers.forEach((member, index) => {
        const hiddenInput = document.createElement("input");
        hiddenInput.type = "hidden";
        hiddenInput.name = `Members[${index}].UserId`;
        hiddenInput.value = member.UserId;
        form.appendChild(hiddenInput);
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
    updateRelativeTime()
    setInterval(updateRelativeTime, 60000)
});
// ------------ Darkmode ------------