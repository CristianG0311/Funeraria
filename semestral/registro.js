const form = document.getElementById("registroForm");

form.addEventListener("submit", function (e) {
    e.preventDefault();

    const usuario = document.getElementById("usuario").value;
    const correo = document.getElementById("correo").value;
    const password = document.getElementById("password").value;

    let usuarios = JSON.parse(localStorage.getItem("usuarios")) || [];

    // Verificar si el usuario ya existe
    if (usuarios.some(u => u.usuario === usuario)) {
        document.getElementById("mensaje").textContent = "Ese usuario ya existe ❌";
        return;
    }

    usuarios.push({ usuario, correo, password });

    localStorage.setItem("usuarios", JSON.stringify(usuarios));

    document.getElementById("mensaje").textContent = "Registro exitoso ✔️";
    form.reset();
});
