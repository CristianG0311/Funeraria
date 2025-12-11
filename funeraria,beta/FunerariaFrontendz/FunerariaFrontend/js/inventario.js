// ============================
//   VALIDAR SESIÓN
// ============================
if (!getToken()) window.location.href = 'index.html';

const user = getUser();
if (user) document.getElementById('userRole').textContent = user.rol;

function logout() {
    if (confirm('¿Está seguro que desea cerrar sesión?')) {
        localStorage.clear();
        window.location.href = 'index.html';
    }
}

// ============================
//   VARIABLES
// ============================
let inventarioData = [];

// ============================
//   CALCULAR ESTADO STOCK
// ============================
function calcularEstadoStock(cantidad, minimo) {
    if (cantidad <= 0) return "Agotado";
    if (cantidad <= minimo) return "Critico";
    if (cantidad <= minimo + 2) return "Bajo";
    return "Normal";
}

// ============================
//   CARGAR INVENTARIO
// ============================
async function loadInventario() {
    const loading = document.getElementById('loading');
    const container = document.getElementById('inventarioContainer');
    const filtroCategoria = document.getElementById('filtroCategoria').value;
    const filtroEstadoStock = document.getElementById('filtroEstadoStock').value;

    loading.classList.remove('hidden');
    container.classList.add('hidden');

    try {
        let endpoint = API_CONFIG.ENDPOINTS.INVENTARIO;
        const params = [];

        if (filtroCategoria) params.push(`categoria=${filtroCategoria}`);
        if (filtroEstadoStock) params.push(`estadoStock=${filtroEstadoStock}`);

        if (params.length > 0) endpoint += `?${params.join('&')}`;

        inventarioData = await apiRequest(endpoint);
        renderInventario(inventarioData);
        calcularEstadisticas(inventarioData);

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
//   ESTADÍSTICAS
// ============================
function calcularEstadisticas(articulos) {
    const total = articulos.length;
    const criticos = articulos.filter(a => a.estadoStock === 'Critico').length;
    const agotados = articulos.filter(a => a.estadoStock === 'Agotado').length;
    const valorTotal = articulos.reduce((sum, a) => sum + (a.cantidadDisponible * a.precioVenta), 0);

    document.getElementById('totalArticulos').textContent = total;
    document.getElementById('stockCritico').textContent = criticos;
    document.getElementById('articulosAgotados').textContent = agotados;
    document.getElementById('valorInventario').textContent = formatCurrency(valorTotal);
}

// ============================
//   RENDER TABLA
// ============================
function renderInventario(articulos) {
    const tbody = document.getElementById('inventarioList');

    if (!articulos || articulos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">No hay artículos en inventario</td>
            </tr>
        `;
        return;
    }

    const inventarioHTML = articulos.map(articulo => {
        let estadoBadge = 'success';
        if (articulo.estadoStock === 'Critico') estadoBadge = 'danger';
        else if (articulo.estadoStock === 'Bajo') estadoBadge = 'warning';
        else if (articulo.estadoStock === 'Agotado') estadoBadge = 'secondary';

        return `
            <tr>
                <td><strong>${articulo.codigo}</strong></td>
                <td>${articulo.nombre}</td>
                <td><span class="badge badge-info">${articulo.categoria}</span></td>
                <td><strong>${articulo.cantidadDisponible}</strong></td>
                <td>${articulo.stockMinimo}</td>
                <td><span class="badge badge-${estadoBadge}">${articulo.estadoStock}</span></td>
                <td><strong>${formatCurrency(articulo.precioVenta)}</strong></td>
                <td>
                    <button class="btn btn-success btn-sm" onclick="mostrarAjustarStock(${articulo.id})" title="Ajustar Stock" style="padding: 6px 10px; font-size: 12px; margin-right: 5px;">
                        ➕ Stock
                    </button>
                    <button class="btn btn-secondary btn-sm" onclick="verDetalles(${articulo.id})" title="Ver Detalles" style="padding: 6px 10px; font-size: 12px;">
                        📄
                    </button>
                </td>
            </tr>
        `;
    }).join('');

    tbody.innerHTML = inventarioHTML;
}

// ============================
//   FILTRO POR TEXTO
// ============================
function filtrarInventario() {
    const texto = document.getElementById('buscarTexto').value.toLowerCase();

    if (!texto) {
        renderInventario(inventarioData);
        calcularEstadisticas(inventarioData);
        return;
    }

    const filtrados = inventarioData.filter(articulo =>
        articulo.nombre.toLowerCase().includes(texto) ||
        articulo.codigo.toLowerCase().includes(texto)
    );

    renderInventario(filtrados);
    calcularEstadisticas(filtrados);
}

// ============================
//   VER DETALLES
// ============================
function verDetalles(articuloId) {
    const articulo = inventarioData.find(a => a.id === articuloId);
    if (!articulo) {
        showAlert('Artículo no encontrado', 'danger');
        return;
    }
    
    const detalles = `
📦 DETALLES DEL ARTÍCULO

Código: ${articulo.codigo}
Nombre: ${articulo.nombre}
Categoría: ${articulo.categoria}
Descripción: ${articulo.descripcion || 'N/A'}

Stock Disponible: ${articulo.cantidadDisponible}
Stock Mínimo: ${articulo.stockMinimo}
Estado: ${articulo.estadoStock}

Precio Costo: ${formatCurrency(articulo.precioCosto)}
Precio Venta: ${formatCurrency(articulo.precioVenta)}

Última Actualización: ${formatDate(articulo.fechaActualizacion)}
    `;
    
    alert(detalles);
}

// ============================
//   ABRIR MODAL
// ============================
function showNuevoArticuloModal() {
    document.getElementById('modalNuevoArticulo').classList.add('show');
    document.getElementById('formNuevoArticulo').reset();
}

// ============================
//   CERRAR MODAL
// ============================
function closeModal() {
    document.getElementById('modalNuevoArticulo').classList.remove('show');
}

// ============================
//   CREAR ARTÍCULO
// ============================
async function crearArticulo(event) {
    event.preventDefault();

    const submitBtn = event.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Guardando...';

    try {
        // Capturar valores del formulario
        const codigo = document.getElementById('codigo').value.trim();
        const nombre = document.getElementById('nombre').value.trim();
        const descripcion = document.getElementById('descripcion').value.trim();
        const categoria = document.getElementById('categoria').value;
        const cantidadDisponible = parseInt(document.getElementById('cantidadDisponible').value);
        const stockMinimo = parseInt(document.getElementById('stockMinimo').value);
        const precioCosto = parseFloat(document.getElementById('precioCosto').value);
        const precioVenta = parseFloat(document.getElementById('precioVenta').value);

        // Validaciones básicas
        if (!codigo || !nombre || !categoria) {
            throw new Error('Complete todos los campos obligatorios');
        }

        if (isNaN(cantidadDisponible) || isNaN(stockMinimo) || isNaN(precioCosto) || isNaN(precioVenta)) {
            throw new Error('Los valores numéricos no son válidos');
        }

        if (precioVenta < precioCosto) {
            const continuar = confirm('⚠️ El precio de venta es menor al precio de costo. ¿Desea continuar?');
            if (!continuar) throw new Error('Operación cancelada');
        }

        // JSON alineado con tu API
        const nuevoArticulo = {
            codigo,
            nombre,
            descripcion: descripcion || "",
            categoria,
            cantidadDisponible,
            stockMinimo,
            precioCosto,
            precioVenta,
            fechaActualizacion: new Date().toISOString()
        };

        console.log('Enviando artículo:', nuevoArticulo);

        // Llamada al backend
        const result = await apiRequest(API_CONFIG.ENDPOINTS.INVENTARIO, {
            method: 'POST',
            body: JSON.stringify(nuevoArticulo)
        });

        // Feedback al usuario
        alert('✅ Artículo creado exitosamente');
        console.log('Artículo creado:', result);

        closeModal();
        loadInventario(); // refresca la lista

    } catch (error) {
        alert('❌ Error: ' + error.message);
        console.error('Error completo:', error);
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = originalText;
    }
}
// ============================
//   CERRAR MODAL AL CLICK FUERA
// ============================
window.onclick = function(event) {
    const modal = document.getElementById('modalNuevoArticulo');
    if (event.target === modal) closeModal();
};

// ============================
//   MOSTRAR MODAL AJUSTAR STOCK
// ============================
function mostrarAjustarStock(articuloId) {
    const articulo = inventarioData.find(a => a.id === articuloId);
    if (!articulo) {
        showAlert('Artículo no encontrado', 'danger');
        return;
    }

    document.getElementById('ajustarArticuloId').value = articulo.id;
    document.getElementById('ajustarNombre').textContent = articulo.nombre;
    document.getElementById('ajustarStockActual').textContent = articulo.cantidadDisponible;
    document.getElementById('cantidadAjuste').value = '';
    document.getElementById('tipoAjuste').value = 'entrada';
    document.getElementById('motivoAjuste').value = '';
    
    document.getElementById('modalAjustarStock').classList.add('show');
}

// ============================
//   CERRAR MODAL AJUSTAR STOCK
// ============================
function closeModalAjustar() {
    document.getElementById('modalAjustarStock').classList.remove('show');
}

// ============================
//   AJUSTAR STOCK
// ============================
async function ajustarStock(event) {
    event.preventDefault();

    const submitBtn = event.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = '⏳ Procesando...';

    try {
        const articuloId = parseInt(document.getElementById('ajustarArticuloId').value);
        const cantidad = parseInt(document.getElementById('cantidadAjuste').value);
        const tipoAjuste = document.getElementById('tipoAjuste').value;
        const motivo = document.getElementById('motivoAjuste').value.trim();

        if (!cantidad || cantidad <= 0) {
            throw new Error('Ingrese una cantidad válida mayor a 0');
        }

        if (!motivo) {
            throw new Error('Ingrese el motivo del ajuste');
        }

        // Preparar datos del movimiento
        const movimientoData = {
            articuloId: articuloId,
            cantidad: cantidad,
            tipoMovimiento: tipoAjuste === 'entrada' ? 'Entrada' : 'Salida',
            motivo: motivo,
            observaciones: motivo
        };

        console.log('📦 Ajustando stock del artículo:', articuloId, movimientoData);

        // Usar el endpoint correcto según el tipo
        const endpoint = tipoAjuste === 'entrada' 
            ? `${API_CONFIG.ENDPOINTS.INVENTARIO}/entrada`
            : `${API_CONFIG.ENDPOINTS.INVENTARIO}/salida`;

        await apiRequest(endpoint, {
            method: 'POST',
            body: JSON.stringify(movimientoData)
        });

        const mensaje = tipoAjuste === 'entrada' 
            ? `✅ Se agregaron ${cantidad} unidades al stock`
            : `✅ Se restaron ${cantidad} unidades del stock`;
        
        showAlert(mensaje, 'success');
        closeModalAjustar();
        loadInventario();

    } catch (error) {
        showAlert(`❌ Error: ${error.message}`, 'danger');
        console.error('Error al ajustar stock:', error);
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = originalText;
    }
}

// ============================
//   CERRAR MODAL AL CLICK FUERA
// ============================
window.onclick = function(event) {
    const modalNuevo = document.getElementById('modalNuevoArticulo');
    const modalAjustar = document.getElementById('modalAjustarStock');
    
    if (event.target === modalNuevo) closeModal();
    if (event.target === modalAjustar) closeModalAjustar();
};

// ============================
//   INICIAR
// ============================
loadInventario();