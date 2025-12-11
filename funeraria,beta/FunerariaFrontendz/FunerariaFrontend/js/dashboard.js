// Verificar autenticación
if (!getToken()) {
    window.location.href = 'index.html';
}

// Mostrar información del usuario
const user = getUser();
if (user) {
    document.getElementById('userName').textContent = user.nombre;
    document.getElementById('userRole').textContent = user.rol;
}

// Función de logout
function logout() {
    if (confirm('¿Está seguro que desea cerrar sesión?')) {
        localStorage.clear();
        window.location.href = 'index.html';
    }
}

// Variable para almacenar los servicios recientes
let serviciosRecientesData = [];

// Cargar datos del dashboard
async function loadDashboard() {
    const loading = document.getElementById('loading');
    const content = document.getElementById('dashboardContent');
    
    try {
        const data = await apiRequest(API_CONFIG.ENDPOINTS.DASHBOARD);
        
        // Actualizar cards
        document.getElementById('serviciosActivos').textContent = data.serviciosActivos;
        document.getElementById('documentosPendientes').textContent = data.documentosPendientes;
        document.getElementById('articulosCriticos').textContent = data.articulosCriticos;
        document.getElementById('facturacionMes').textContent = formatCurrency(data.facturacionMes);
        
        // Mostrar alertas
        renderAlertas(data.alertas);
        
        // Guardar y mostrar servicios recientes
        serviciosRecientesData = data.serviciosRecientes || [];
        renderServiciosRecientes(serviciosRecientesData);
        
        // Ocultar loading y mostrar contenido
        loading.classList.add('hidden');
        content.classList.remove('hidden');
        
    } catch (error) {
        loading.innerHTML = `
            <div class="alert alert-danger">
                <strong>Error:</strong> ${error.message}
            </div>
        `;
    }
}

// Renderizar alertas
function renderAlertas(alertas) {
    const container = document.getElementById('alertasContainer');
    
    if (!alertas || alertas.length === 0) {
        container.innerHTML = '';
        return;
    }
    
    const alertasHTML = alertas.map(alerta => {
        let type = 'info';
        if (alerta.nivel === 'Critico') type = 'danger';
        else if (alerta.nivel === 'Advertencia') type = 'warning';
        
        return `
            <div class="alert alert-${type}">
                <strong>${alerta.tipo}:</strong> ${alerta.mensaje}
                <small style="margin-left: 10px; opacity: 0.7;">
                    ${formatDate(alerta.fecha)}
                </small>
            </div>
        `;
    }).join('');
    
    container.innerHTML = alertasHTML;
}

// Renderizar servicios recientes
function renderServiciosRecientes(servicios) {
    const tbody = document.getElementById('serviciosRecientes');
    
    if (!servicios || servicios.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" class="text-center">No hay servicios recientes</td>
            </tr>
        `;
        return;
    }
    
    const serviciosHTML = servicios.map(servicio => {
        let estadoBadge = 'secondary';
        if (servicio.estado === 'Completado') estadoBadge = 'success';
        else if (servicio.estado === 'EnProceso') estadoBadge = 'info';
        else if (servicio.estado === 'Cancelado') estadoBadge = 'danger';
        
        return `
            <tr>
                <td><strong>${servicio.codigoExpediente}</strong></td>
                <td>${servicio.nombreDifunto}</td>
                <td>${formatDate(servicio.fechaFallecimiento)}</td>
                <td><span class="badge badge-${estadoBadge}">${servicio.estado}</span></td>
                <td><strong>${formatCurrency(servicio.total)}</strong></td>
                <td>
                    <button class="btn btn-secondary" onclick="verDetalles(${servicio.id})" style="padding: 6px 12px; font-size: 12px;">
                        Ver Detalles
                    </button>
                </td>
            </tr>
        `;
    }).join('');
    
    tbody.innerHTML = serviciosHTML;
}

// Ver detalles de un servicio
function verDetalles(servicioId) {
    const servicio = serviciosRecientesData.find(s => s.id === servicioId);
    if (!servicio) {
        alert('Servicio no encontrado');
        return;
    }
    
    alert(`
📋 DETALLES DEL SERVICIO

Código Expediente: ${servicio.codigoExpediente}
Difunto: ${servicio.nombreDifunto}
Fecha Fallecimiento: ${formatDate(servicio.fechaFallecimiento)}
Lugar Servicio: ${servicio.lugarServicio || 'N/A'}
Estado: ${servicio.estado}

📞 Contacto Familiar
Nombre: ${servicio.nombreFamiliar}
Teléfono: ${servicio.telefonoFamiliar}

💰 Información Financiera
Total: ${formatCurrency(servicio.total)}

📄 Observaciones
${servicio.observaciones || 'Sin observaciones'}

Fecha de Registro: ${formatDate(servicio.fechaRegistro)}
    `);
}

// Cargar dashboard al iniciar
loadDashboard();

// Actualizar cada 30 segundos
setInterval(loadDashboard, 30000);
