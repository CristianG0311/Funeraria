// ============================
//   AUTENTICACIÓN
// ============================
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

let documentosData = [];

// ============================
//   CARGAR SERVICIOS
// ============================
async function loadServicios() {
    try {
        const servicios = await apiRequest(API_CONFIG.ENDPOINTS.SERVICIOS);

        // Filtro lateral
        const selectFiltro = document.getElementById('filtroServicio');
        servicios.forEach(servicio => {
            const option = document.createElement('option');
            option.value = servicio.id;
            option.textContent = `${servicio.codigoExpediente} - ${servicio.nombreDifunto}`;
            selectFiltro.appendChild(option);
        });

        // Modal Nuevo Documento
        const selectModal = document.getElementById('servicioId');
        servicios.forEach(servicio => {
            const option = document.createElement('option');
            option.value = servicio.id;
            option.textContent = `${servicio.codigoExpediente} - ${servicio.nombreDifunto}`;
            selectModal.appendChild(option);
        });

    } catch (error) {
        console.error('Error cargando servicios:', error);
    }
}

// ============================
//   CARGAR DOCUMENTOS
// ============================
async function loadDocumentos() {
    const loading = document.getElementById('loading');
    const container = document.getElementById('documentosContainer');

    const filtroServicio = document.getElementById('filtroServicio').value;
    const filtroEstado = document.getElementById('filtroEstado').value;
    const filtroTipo = document.getElementById('filtroTipo').value;

    loading.classList.remove('hidden');
    container.classList.add('hidden');

    try {
        let endpoint = "";

        // Si se selecciona un servicio → GET /Documentos/servicio/{id}
        if (filtroServicio) {
            endpoint = `${API_CONFIG.ENDPOINTS.DOCUMENTOS}/servicio/${filtroServicio}`;
        }
        // Si NO se selecciona servicio → GET /Documentos (TODOS)
        else {
            endpoint = `${API_CONFIG.ENDPOINTS.DOCUMENTOS}`;
        }

        documentosData = await apiRequest(endpoint);

        // Filtrar por estado
        if (filtroEstado) {
            documentosData = documentosData.filter(d => d.estado === filtroEstado);
        }

        // Filtrar por tipo
        if (filtroTipo) {
            documentosData = documentosData.filter(d => d.tipoDocumento === filtroTipo);
        }

        renderDocumentos(documentosData);
        calcularEstadisticas(documentosData);

        loading.classList.add('hidden');
        container.classList.remove('hidden');

    } catch (error) {
        loading.innerHTML = `
            <div class="alert alert-danger">
                <strong>Error:</strong> ${error.message}
            </div>
        `;
    }
}

// ============================
//   CALCULAR ESTADÍSTICAS
// ============================
function calcularEstadisticas(documentos) {
    const total = documentos.length;
    const completos = documentos.filter(d => d.estado === 'Completo').length;
    const pendientes = documentos.filter(d => d.estado === 'Pendiente').length;
    const rechazados = documentos.filter(d => d.estado === 'Rechazado').length;

    document.getElementById('totalDocumentos').textContent = total;
    document.getElementById('documentosCompletos').textContent = completos;
    document.getElementById('documentosPendientes').textContent = pendientes;
    document.getElementById('documentosRechazados').textContent = rechazados;
}

// ============================
//   RENDER DOCUMENTOS
// ============================
function renderDocumentos(documentos) {
    const tbody = document.getElementById('documentosList');

    if (!documentos || documentos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">No hay documentos registrados</td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = documentos.map(doc => {
        let estadoBadge = 'secondary';
        if (doc.estado === 'Completo') estadoBadge = 'success';
        else if (doc.estado === 'Pendiente') estadoBadge = 'warning';
        else if (doc.estado === 'Rechazado') estadoBadge = 'danger';

        const obligatorioBadge = doc.esObligatorio
            ? '<span class="badge badge-danger">Obligatorio</span>'
            : '<span class="badge badge-secondary">Opcional</span>';

        return `
            <tr>
                <td><strong>${doc.codigoExpediente || 'N/A'}</strong></td>
                <td>${doc.tipoDocumento}</td>
                <td>${doc.numeroDocumento || 'N/A'}</td>
                <td>${doc.entidadEmisora || 'N/A'}</td>
                <td>${doc.fechaEmision ? formatDate(doc.fechaEmision) : 'N/A'}</td>
                <td><span class="badge badge-${estadoBadge}">${doc.estado}</span></td>
                <td>${obligatorioBadge}</td>
                <td>
                    ${generarBotonesAccion(doc)}
                </td>
            </tr>
        `;
    }).join('');
}

// ============================
//   GENERAR BOTONES DE ACCIÓN
// ============================
function generarBotonesAccion(doc) {
    // Documentos pendientes: pueden ser aceptados o rechazados
    if (doc.estado === 'Pendiente') {
        return `
            <button class="btn btn-success btn-sm" onclick="cambiarEstadoDocumento(${doc.id}, 'Completo')" title="Aceptar" style="margin-right: 3px;">✅</button>
            <button class="btn btn-danger btn-sm" onclick="cambiarEstadoDocumento(${doc.id}, 'Rechazado')" title="Rechazar" style="margin-right: 3px;">❌</button>
            <button class="btn btn-secondary btn-sm" onclick="verDocumento(${doc.id})" title="Ver detalles">📄</button>
        `;
    }
    
    // Documentos completos y rechazados: SOLO VER (historial)
    return `<button class="btn btn-secondary btn-sm" onclick="verDocumento(${doc.id})" title="Ver detalles">📄 Ver</button>`;
}

// ============================
//   CAMBIAR ESTADO DOCUMENTO
// ============================
async function cambiarEstadoDocumento(documentoId, nuevoEstado) {
    const doc = documentosData.find(d => d.id === documentoId);
    if (!doc) {
        showAlert('Documento no encontrado', 'danger');
        return;
    }

    let confirmMsg = '';
    let observaciones = doc.observaciones || '';
    let numeroDoc = doc.numeroDocumento || null;
    let entidad = doc.entidadEmisora || null;
    let fechaEmision = doc.fechaEmision || null;

    if (nuevoEstado === 'Completo') {
        confirmMsg = '¿Desea marcar este documento como COMPLETO/ACEPTADO?';
        
        // Si no tiene número de documento, pedirlo
        if (!numeroDoc) {
            numeroDoc = prompt('Ingrese el número del documento:');
            if (!numeroDoc || numeroDoc.trim() === '') {
                showAlert('Debe ingresar el número del documento', 'warning');
                return;
            }
            numeroDoc = numeroDoc.trim();
        }
        
        // Si no tiene entidad emisora, pedirla
        if (!entidad) {
            entidad = prompt('Ingrese la entidad emisora:');
            if (!entidad || entidad.trim() === '') {
                showAlert('Debe ingresar la entidad emisora', 'warning');
                return;
            }
            entidad = entidad.trim();
        }
        
        // Si no tiene fecha, usar la actual
        if (!fechaEmision) {
            fechaEmision = new Date().toISOString();
        }
        
        observaciones = 'Documento aceptado y completado';
        
    } else if (nuevoEstado === 'Rechazado') {
        confirmMsg = '¿Desea RECHAZAR este documento?';
        const motivo = prompt('Ingrese el motivo del rechazo:');
        if (motivo === null) return; // Usuario canceló
        if (!motivo || motivo.trim() === '') {
            showAlert('Debe ingresar el motivo del rechazo', 'warning');
            return;
        }
        observaciones = motivo.trim();
        
    } else if (nuevoEstado === 'Pendiente') {
        confirmMsg = '¿Desea marcar este documento como PENDIENTE?';
        observaciones = observaciones || 'Documento marcado como pendiente';
    } else {
        showAlert('Estado no válido', 'danger');
        return;
    }

    if (!confirm(confirmMsg)) return;

    try {
        const updateData = {
            numeroDocumento: numeroDoc,
            fechaEmision: fechaEmision,
            entidadEmisora: entidad,
            estado: nuevoEstado,
            observaciones: observaciones
        };

        console.log('📤 Actualizando documento ID:', documentoId);
        console.log('📊 Nuevo estado:', nuevoEstado);
        console.log('📦 Datos completos:', updateData);

        await apiRequest(`${API_CONFIG.ENDPOINTS.DOCUMENTOS}/${documentoId}`, {
            method: 'PUT',
            body: JSON.stringify(updateData)
        });

        let mensaje = '';
        let tipo = 'success';
        
        switch(nuevoEstado) {
            case 'Completo':
                mensaje = '✅ Documento aceptado correctamente';
                tipo = 'success';
                break;
            case 'Rechazado':
                mensaje = '❌ El documento está rechazado';
                tipo = 'warning';
                break;
            case 'Pendiente':
                mensaje = '⏳ El documento está pendiente';
                tipo = 'info';
                break;
            default:
                mensaje = '✅ Documento actualizado correctamente';
        }
        
        showAlert(mensaje, tipo);
        loadDocumentos();

    } catch (error) {
        console.error('Error al cambiar estado:', error);
        showAlert(`❌ Error al cambiar estado: ${error.message}`, 'danger');
    }
}

// ============================
//   VER DOCUMENTO
// ============================
function verDocumento(id) {
    const doc = documentosData.find(d => d.id === id);
    if (!doc) {
        showAlert('Documento no encontrado', 'danger');
        return;
    }

    alert(`
📄 DETALLES DEL DOCUMENTO

Servicio: ${doc.codigoExpediente}
Tipo: ${doc.tipoDocumento}
Número: ${doc.numeroDocumento || 'N/A'}
Entidad: ${doc.entidadEmisora || 'N/A'}
Fecha: ${doc.fechaEmision ? formatDate(doc.fechaEmision) : 'N/A'}
Estado: ${doc.estado}
Obligatorio: ${doc.esObligatorio ? 'Sí' : 'No'}
Observaciones: ${doc.observaciones || 'N/A'}
    `);
}

// ============================
//   NUEVO DOCUMENTO
// ============================
function showNuevoDocumentoModal() {
    document.getElementById('modalNuevoDocumento').classList.add('show');
    document.getElementById('formNuevoDocumento').reset();
    // Establecer estado por defecto como Pendiente
    document.getElementById('estado').value = 'Pendiente';
}

function closeModalDocumento() {
    document.getElementById('modalNuevoDocumento').classList.remove('show');
}

async function crearDocumento(event) {
    event.preventDefault();

    const btn = event.target.querySelector('button[type="submit"]');
    const original = btn.textContent;

    btn.disabled = true;
    btn.textContent = '⏳ Guardando...';

    try {
        const data = {
            servicioId: parseInt(document.getElementById('servicioId').value),
            tipoDocumento: document.getElementById('tipoDocumento').value,
            numeroDocumento: document.getElementById('numeroDocumento').value || null,
            entidadEmisora: document.getElementById('entidadEmisora').value || null,
            fechaEmision: document.getElementById('fechaEmision').value
                ? new Date(document.getElementById('fechaEmision').value).toISOString()
                : null,
            estado: document.getElementById('estado').value,
            esObligatorio: document.getElementById('esObligatorio').checked,
            observaciones: document.getElementById('observaciones').value || null
        };

        console.log('📤 Datos enviados al backend:', data);
        console.log('📊 Estado seleccionado:', document.getElementById('estado').value);

        const response = await apiRequest(API_CONFIG.ENDPOINTS.DOCUMENTOS, {
            method: 'POST',
            body: JSON.stringify(data)
        });

        console.log('✅ Respuesta del servidor:', response);
        console.log('✅ Documento creado con estado:', data.estado);

        // Mensaje según el estado
        let mensaje = '';
        if (data.estado === 'Completo') {
            mensaje = '✅ Documento registrado y aceptado correctamente';
        } else if (data.estado === 'Rechazado') {
            mensaje = '❌ El documento está rechazado';
        } else if (data.estado === 'Pendiente') {
            mensaje = '⏳ El documento está pendiente';
        } else {
            mensaje = `✅ Documento registrado con estado: ${data.estado}`;
        }

        showAlert(mensaje, data.estado === 'Completo' ? 'success' : 'info');
        closeModalDocumento();
        loadDocumentos();

    } catch (error) {
        showAlert(`❌ Error: ${error.message}`, 'danger');
    }

    btn.disabled = false;
    btn.textContent = original;
}

// ============================
//   CERRAR MODAL AL HACER CLICK FUERA
// ============================
window.onclick = function (event) {
    const modal = document.getElementById('modalNuevoDocumento');
    if (event.target === modal) {
        closeModalDocumento();
    }
};

// ============================
//   INICIALIZACIÓN
// ============================
loadServicios();
loadDocumentos();
