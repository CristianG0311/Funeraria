const form = document.getElementById("loginForm");

form.addEventListener("submit", function (e) {
    e.preventDefault();

    const usuario = document.getElementById("usuario").value;
    const password = document.getElementById("password").value;

    const usuarios = JSON.parse(localStorage.getItem("usuarios")) || [];

    const encontrado = usuarios.find(u => u.usuario === usuario && u.password === password);

    if (encontrado) {
        document.getElementById("mensaje").textContent = "Inicio de sesión exitoso ✔️";

        // Redirigir a otra página si quieres
        // setTimeout(() => window.location.href = "dashboard.html", 1000);

    } else {
        document.getElementById("mensaje").textContent = "Usuario o contraseña incorrectos ❌";
    }
});
