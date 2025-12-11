// Configuración de la API
const API_CONFIG = {
    BASE_URL: 'https://localhost:7183/api',
    ENDPOINTS: {
        AUTH: '/Auth/login',
        SERVICIOS: '/Servicios',
        DOCUMENTOS: '/Documentos',
        INVENTARIO: '/Inventario',
        FACTURACION: '/Facturacion',
        DASHBOARD: '/Dashboard/resumen'
    }
};

// Función para obtener el token
function getToken() {
    return localStorage.getItem('token');
}

// Función para obtener el usuario
function getUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
}

// Función para hacer peticiones a la API - MEJORADA
async function apiRequest(endpoint, options = {}) {
    const url = `${API_CONFIG.BASE_URL}${endpoint}`;
    
    const defaultOptions = {
        headers: {
            'Content-Type': 'application/json'
        }
    };

    const token = getToken();
    if (token) {
        defaultOptions.headers['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        ...defaultOptions,
        ...options,
        headers: {
            ...defaultOptions.headers,
            ...options.headers
        }
    };

    try {
        console.log('📤 Petición:', options.method || 'GET', url);
        console.log('📦 Datos:', options.body);
        
        const response = await fetch(url, config);
        
        console.log('📥 Respuesta Status:', response.status, response.statusText);
        
        // Manejar errores HTTP
        if (!response.ok) {
            if (response.status === 401) {
                localStorage.clear();
                window.location.href = 'index.html';
                throw new Error('Sesión expirada');
            }
            
            // Intentar leer el error del servidor
            let errorMessage = 'Error en la petición';
            try {
                const errorData = await response.json();
                errorMessage = errorData.mensaje || errorData.detalle || errorData.title || JSON.stringify(errorData);
            } catch (e) {
                // Si no hay JSON, usar el texto
                const errorText = await response.text();
                errorMessage = errorText || `Error ${response.status}: ${response.statusText}`;
            }
            
            throw new Error(errorMessage);
        }

        // Verificar si hay contenido en la respuesta
        const contentType = response.headers.get('content-type');
        const contentLength = response.headers.get('content-length');
        
        console.log('📄 Content-Type:', contentType);
        console.log('📏 Content-Length:', contentLength);
        
        // Si no hay contenido o es 204 No Content
        if (response.status === 204 || contentLength === '0') {
            console.log('✅ Respuesta exitosa sin contenido');
            return null;
        }
        
        // Si hay contenido JSON
        if (contentType && contentType.includes('application/json')) {
            const text = await response.text();
            console.log('📝 Respuesta raw:', text);
            
            if (!text || text.trim() === '') {
                console.log('✅ Respuesta vacía (exitosa)');
                return null;
            }
            
            try {
                const data = JSON.parse(text);
                console.log('✅ Respuesta parseada:', data);
                return data;
            } catch (e) {
                console.error('❌ Error parseando JSON:', e);
                console.error('Texto recibido:', text);
                throw new Error('Respuesta inválida del servidor');
            }
        }
        
        // Si no es JSON, retornar el texto
        const text = await response.text();
        console.log('📝 Respuesta texto:', text);
        return text || null;

    } catch (error) {
        console.error('❌ Error en apiRequest:', error);
        throw error;
    }
}

// Función para mostrar alertas
function showAlert(message, type = 'info') {
    // Crear elemento de alerta
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type}`;
    alertDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 10000;
        min-width: 300px;
        padding: 15px 20px;
        border-radius: 5px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        animation: slideIn 0.3s ease-out;
    `;
    
    // Colores según tipo
    const colors = {
        success: { bg: '#d4edda', color: '#155724', border: '#c3e6cb' },
        danger: { bg: '#f8d7da', color: '#721c24', border: '#f5c6cb' },
        warning: { bg: '#fff3cd', color: '#856404', border: '#ffeaa7' },
        info: { bg: '#d1ecf1', color: '#0c5460', border: '#bee5eb' }
    };
    
    const colorScheme = colors[type] || colors.info;
    alertDiv.style.backgroundColor = colorScheme.bg;
    alertDiv.style.color = colorScheme.color;
    alertDiv.style.border = `1px solid ${colorScheme.border}`;
    
    alertDiv.textContent = message;
    
    document.body.appendChild(alertDiv);
    
    // Agregar animación
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);
    
    // Remover después de 5 segundos
    setTimeout(() => {
        alertDiv.style.animation = 'slideIn 0.3s ease-out reverse';
        setTimeout(() => alertDiv.remove(), 300);
    }, 5000);
}

// Función para formatear fechas
function formatDate(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('es-PA', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
}

// Función para formatear moneda
function formatCurrency(amount) {
    if (amount === null || amount === undefined) return '$0.00';
    return new Intl.NumberFormat('es-PA', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}