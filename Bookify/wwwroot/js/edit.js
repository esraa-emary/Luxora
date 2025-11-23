document.addEventListener("DOMContentLoaded", function () {
  const form = document.querySelector(".profile-form");

  form.addEventListener("submit", function (e) {
    const firstName = document.querySelector('[name="FirstName"]')?.value;
    const lastName = document.querySelector('[name="LastName"]')?.value;
    const email = document.querySelector('[name="Email"]')?.value;

    if (!firstName || !lastName) {
      if (!firstName) {
        const firstNameField = document.querySelector('[name="FirstName"]');
        if (firstNameField) {
          firstNameField.classList.add("shake");
          setTimeout(() => {
            firstNameField.classList.remove("shake");
          }, 600);
        }
      }
      if (!lastName) {
        const lastNameField = document.querySelector('[name="LastName"]');
        if (lastNameField) {
          lastNameField.classList.add("shake");
          setTimeout(() => {
            lastNameField.classList.remove("shake");
          }, 600);
        }
      }
    }

    if (!email) {
      const emailField = document.querySelector('[name="Email"]');
      if (emailField) {
        emailField.classList.add("shake");
        setTimeout(() => {
          emailField.classList.remove("shake");
        }, 600);
      }
    }

    const saveBtn = document.querySelector(".save-btn");
    saveBtn.innerHTML = '<i class="fas fa-check"></i> Changes Saved';
    saveBtn.style.background = "var(--success-green)";
    saveBtn.style.boxShadow = "0 6px 20px rgba(129, 199, 132, 0.4)";

    document.querySelector(
      ".user-name"
    ).textContent = `${firstName} ${lastName}`;

    setTimeout(() => {
      saveBtn.innerHTML = '<i class="fas fa-save"></i> Save Changes';
      saveBtn.style.background = "var(--gradient-primary)";
      saveBtn.style.boxShadow = "0 6px 20px rgba(212, 163, 115, 0.4)";
    }, 3000);
  });

  // Avatar upload simulation
  const avatarOverlay = document.querySelector(".avatar-overlay");

  avatarOverlay.addEventListener("click", function () {
    alert(
      "Avatar upload feature would open here. You could select a new profile image."
    );
  });

  // Add floating animation to some elements
  const elementsToFloat = document.querySelectorAll(
    ".user-avatar, .membership-badge"
  );
  elementsToFloat.forEach((el) => {
    el.classList.add("floating");
  });
});
