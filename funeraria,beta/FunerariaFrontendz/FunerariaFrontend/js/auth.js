// Manejo del formulario de login
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorDiv = document.getElementById('errorMessage');
    const submitBtn = e.target.querySelector('button[type="submit"]');
    
    // Deshabilitar botón y mostrar loading
    submitBtn.disabled = true;
    submitBtn.textContent = 'Iniciando sesión...';
    errorDiv.classList.add('hidden');
    
    try {
        const response = await apiRequest(API_CONFIG.ENDPOINTS.AUTH, {
            method: 'POST',
            body: JSON.stringify({
                nombreUsuario: username,
                password: password
            })
        });
        
        // Guardar token y usuario
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify({
            id: response.usuarioId,
            nombre: response.nombreCompleto,
            rol: response.rol
        }));
        
        // Redireccionar al dashboard
        window.location.href = 'dashboard.html';
        
    } catch (error) {
        errorDiv.textContent = error.message || 'Credenciales inválidas';
        errorDiv.classList.remove('hidden');
        alert('Usuario o contraseña incorrectos. Por favor, inténtelo de nuevo.');
        
        submitBtn.disabled = false;
        submitBtn.textContent = 'Iniciar Sesión';
    }
});

// Solo verificar si ya está logueado cuando NO estás en index.html o login
// NO hacer auto-login en la página de login
const currentPage = window.location.pathname.split('/').pop();
if (currentPage && currentPage !== 'index.html' && currentPage !== '') {
    // En otras páginas, verificar autenticación
    if (!getToken()) {
        window.location.href = 'index.html';
    }
}
