function togglePasswordVisibility(id) {
    const passwordField = document.querySelector(`#${id}`);
    const togglePasswordIcon = document.querySelector(`#toggle${id}Icon`);
    const isPasswordVisible = passwordField.type === 'text';

    passwordField.type = isPasswordVisible ? 'password' : 'text';
    togglePasswordIcon.classList.replace(isPasswordVisible ? 'fa-eye-slash' : 'fa-eye', isPasswordVisible ? 'fa-eye' : 'fa-eye-slash');
    togglePasswordIcon.alt = isPasswordVisible ? 'Show Password' : 'Hide Password';
}
