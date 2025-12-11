// =========================
// VERIFICAR LOGIN
// =========================
if (!getToken()) {
    window.location.href = 'index.html';
}

const user = getUser();
if (user) {
    document.getElementById('userRole').textContent = user.rol;
}

function logout() {
    if (confirm('¿Está seguro que desea cerrar sesión?')) {
        localStorage.clear();
        window.location.href = 'index.html';
    }
}

// =========================
// VARIABLES
// =========================
let serviciosData = [];

// =========================
// CARGAR SERVICIOS
// =========================
async function loadServicios() {
    const loading = document.getElementById('loading');
    const container = document.getElementById('serviciosContainer');
    const filtroEstado = document.getElementById('filtroEstado').value;
    
    loading.classList.remove('hidden');
    container.classList.add('hidden');
    
    try {
        let endpoint = API_CONFIG.ENDPOINTS.SERVICIOS;
        if (filtroEstado) endpoint += `?estado=${filtroEstado}`;
        
        serviciosData = await apiRequest(endpoint);
        renderServicios(serviciosData);
        
        loading.classList.add('hidden');
        container.classList.remove('hidden');
        
    } catch (error) {
        loading.innerHTML = `<div class="alert alert-danger"><strong>Error:</strong> ${error.message}</div>`;
    }
}

// =========================
// RENDER TABLA
// =========================
function renderServicios(servicios) {
    const tbody = document.getElementById('serviciosList');
    
    if (!servicios || servicios.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" class="text-center">No hay servicios registrados</td></tr>`;
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
                <td><span class="badge badge-info">${servicio.paquete}</span></td>
                <td>${servicio.tipoServicio}</td>
                <td><span class="badge badge-${estadoBadge}">${servicio.estado}</span></td>
                <td><strong>${formatCurrency(servicio.total || 0)}</strong></td>
                <td><button class="btn btn-secondary" onclick="verServicio(${servicio.id})">📄 Ver</button></td>
            </tr>
        `;
    }).join('');
    
    tbody.innerHTML = serviciosHTML;
}

// =========================
// FILTRAR POR TEXTO
// =========================
function filtrarServicios() {
    const texto = document.getElementById('buscarTexto').value.toLowerCase();
    if (!texto) {
        renderServicios(serviciosData);
        return;
    }
    
    const filtrados = serviciosData.filter(servicio => 
        servicio.nombreDifunto.toLowerCase().includes(texto) ||
        servicio.codigoExpediente.toLowerCase().includes(texto)
    );
    
    renderServicios(filtrados);
}

//validacion de cedula
// Validación de la cédula panameña con el formato 8-438-2439
document.getElementById("formServicio").addEventListener("submit", async (e) => {
    e.preventDefault();

    const cedula = document.getElementById("cedulaDifunto").value;

    // Validar cédula
    if (!validarCedulaPanama(cedula)) {
        alert("La cédula proporcionada no es válida.");
        return; // Detener el flujo si la cédula es inválida
    }

    // Si la cédula es válida, continuar con el proceso de guardado
    const servicio = {
        nombreDifunto: nombreDifunto.value,
        cedulaDifunto: cedulaDifunto.value,
        fechaFallecimiento: fechaFallecimiento.value,
        edad: parseInt(edad.value),
        lugarFallecimiento: lugarFallecimiento.value,
        nombreFamiliar: nombreFamiliar.value,
        parentesco: parentesco.value,
        telefonoFamiliar: telefonoFamiliar.value,
        emailFamiliar: emailFamiliar.value,
        paquete: paquete.value,
        tipoServicio: tipoServicio.value,
        salaVelacion: salaVelacion.value,
        ceremoniaReligiosa: ceremoniaReligiosa.checked,
        gestionDocumentalCompleta: gestionDocumentalCompleta.checked,
        observaciones: observaciones.value
    };

    try {
        await apiRequest(API_CONFIG.ENDPOINTS.SERVICIOS, {
            method: 'POST',
            body: JSON.stringify(servicio)
        });

        closeModal();
        loadServicios();
        showAlert("Servicio registrado correctamente", "success");

    } catch (err) {
        showAlert("Error: " + err.message, "danger");
    }
});





// =========================
// VER DETALLE
// =========================
function verServicio(id) {
    const servicio = serviciosData.find(s => s.id === id);
    if (!servicio) {
        alert('Servicio no encontrado');
        return;
    }
    
    alert(`
📋 DETALLES DEL SERVICIO

Código Expediente: ${servicio.codigoExpediente}
Estado: ${servicio.estado}

👤 Información del Difunto
Nombre: ${servicio.nombreDifunto}
Cédula: ${servicio.cedulaDifunto || 'N/A'}
Edad: ${servicio.edad || 'N/A'} años
Fecha Fallecimiento: ${formatDate(servicio.fechaFallecimiento)}
Lugar Fallecimiento: ${servicio.lugarFallecimiento || 'N/A'}

📞 Contacto Familiar
Nombre: ${servicio.nombreFamiliar}
Parentesco: ${servicio.parentesco || 'N/A'}
Teléfono: ${servicio.telefonoFamiliar}
Email: ${servicio.emailFamiliar || 'N/A'}

🎫 Detalles del Servicio
Paquete: ${servicio.paquete}
Tipo Servicio: ${servicio.tipoServicio}
Sala Velación: ${servicio.salaVelacion || 'N/A'}
Ceremonia Religiosa: ${servicio.ceremoniaReligiosa ? 'Sí' : 'No'}
Gestión Documental: ${servicio.gestionDocumentalCompleta ? 'Completa' : 'Parcial'}

💰 Información Financiera
Total: ${formatCurrency(servicio.total || 0)}

📄 Observaciones
${servicio.observaciones || 'Sin observaciones'}

Fecha de Registro: ${formatDate(servicio.fechaRegistro)}
    `);
}

// =========================
// ABRIR / CERRAR MODAL
// =========================
function showNuevoServicioModal() {
    const modal = document.getElementById('modalServicio');
    modal.classList.remove('hidden');
}

function closeModal() {
    const modal = document.getElementById('modalServicio');
    modal.classList.add('hidden');
}

// =========================
// GUARDAR SERVICIO
// =========================
document.getElementById("formServicio").addEventListener("submit", async(e)=>{
    e.preventDefault();

    const servicio = {
        nombreDifunto: nombreDifunto.value,
        cedulaDifunto: cedulaDifunto.value,
        fechaFallecimiento: fechaFallecimiento.value,
        edad: parseInt(edad.value),
        lugarFallecimiento: lugarFallecimiento.value,
        nombreFamiliar: nombreFamiliar.value,
        parentesco: parentesco.value,
        telefonoFamiliar: telefonoFamiliar.value,
        emailFamiliar: emailFamiliar.value,
        paquete: paquete.value,
        tipoServicio: tipoServicio.value,
        salaVelacion: salaVelacion.value,
        ceremoniaReligiosa: ceremoniaReligiosa.checked,
        gestionDocumentalCompleta: gestionDocumentalCompleta.checked,
        observaciones: observaciones.value
    };

    try{
        await apiRequest(API_CONFIG.ENDPOINTS.SERVICIOS,{
            method:'POST',
            body: JSON.stringify(servicio)
        });

        closeModal();
        loadServicios();
        showAlert("Servicio registrado correctamente","success");

    }catch(err){
        showAlert("Error: "+err.message,"danger");
    }
});

// =========================
// INICIO
// =========================
loadServicios();
