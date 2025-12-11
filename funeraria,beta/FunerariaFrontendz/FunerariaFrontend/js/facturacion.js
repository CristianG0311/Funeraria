// js/facturacion.js

// Inicialización
document.addEventListener("DOMContentLoaded", () => {
  console.log("🚀 Inicializando módulo de facturación...");
  cargarFacturas();
  setupPagoModalHandlers();
  setupGenerarFacturaHandlers();
  cargarServiciosParaFactura();
});

// ========== CARGAR FACTURAS ==========
async function cargarFacturas() {
  const loading = document.getElementById("loading");
  const container = document.getElementById("facturasContainer");
  
  if (loading) loading.classList.remove("hidden");
  
  try {
    console.log("📋 Cargando facturas...");
    const facturas = await apiRequest("/Facturacion");
    const tbody = document.getElementById("facturasList");
    
    if (!tbody) {
      console.error("❌ No se encontró el elemento facturasList");
      return;
    }
    
    tbody.innerHTML = "";

    if (!Array.isArray(facturas) || facturas.length === 0) {
      tbody.innerHTML = `<tr><td colspan="9" style="text-align:center;">No hay facturas registradas</td></tr>`;
      return;
    }

    console.log(`✅ Se cargaron ${facturas.length} facturas`);

    facturas.forEach(f => {
      const saldo = f.saldoPendiente ?? (f.total - (f.montoPagado || 0));
      const numeroEsc = String(f.numeroFactura || "N/A").replace(/"/g, "&quot;");

      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${f.numeroFactura ?? "-"}</td>
        <td>${f.codigoExpediente ?? "-"}</td>
        <td>${f.nombreDifunto ?? "-"}</td>
        <td>${formatDate(f.fechaEmision)}</td>
        <td>${formatCurrency(f.total)}</td>
        <td>${formatCurrency(f.montoPagado || 0)}</td>
        <td>${formatCurrency(saldo)}</td>
        <td><span class="badge ${f.pagada ? 'badge-success' : 'badge-warning'}">${f.pagada ? "Pagada" : "Pendiente"}</span></td>
        <td>
          ${!f.pagada ? `
            <button class="btn btn-success btn-sm btn-pagar" 
                    data-factura-id="${f.id}"
                    data-factura-numero="${numeroEsc}"
                    data-factura-saldo="${saldo}"
                    title="Registrar pago">
              💵 Pagar
            </button>
          ` : '<span style="color:#888;">Completada</span>'}
        </td>
      `;
      tbody.appendChild(tr);
    });

  } catch (err) {
    console.error("❌ Error cargando facturas:", err);
    showAlert("Error cargando facturas: " + err.message, "danger");
    const tbody = document.getElementById("facturasList");
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan="9" style="text-align:center;color:red;">Error al cargar facturas</td></tr>`;
    }
  } finally {
    if (loading) loading.classList.add("hidden");
  }
}

// ========== MODAL GENERAR FACTURA ==========
function setupGenerarFacturaHandlers() {
  const btnGenerar = document.getElementById("btnGenerarFactura");
  if (btnGenerar) {
    console.log("✅ Botón generar factura encontrado");
    btnGenerar.addEventListener("click", abrirModalGenerarFactura);
  } else {
    console.error("❌ No se encontró el botón btnGenerarFactura");
  }
}

function abrirModalGenerarFactura() {
  console.log("📝 Abriendo modal generar factura");
  const modal = document.getElementById("modalGenerarFactura");
  if (!modal) {
    console.error("❌ No se encontró el modal");
    return;
  }
  
  modal.classList.remove("hidden");
  modal.classList.add("show");
  
  // Resetear formulario
  const form = document.getElementById("formGenerarFactura");
  if (form) form.reset();
  
  const detalles = document.getElementById("detallesServicio");
  if (detalles) detalles.classList.add("hidden");
}

function closeModalFactura() {
  console.log("❌ Cerrando modal generar factura");
  const modal = document.getElementById("modalGenerarFactura");
  if (modal) {
    modal.classList.remove("show");
    modal.classList.add("hidden");
  }
}

// Cargar servicios disponibles para facturar
async function cargarServiciosParaFactura() {
  try {
    console.log("🔍 Cargando servicios para facturar...");
    const servicios = await apiRequest("/Servicios");
    const select = document.getElementById("servicioIdFactura");
    
    if (!select) {
      console.error("❌ No se encontró el select servicioIdFactura");
      return;
    }
    
    select.innerHTML = '<option value="">Seleccione un servicio...</option>';
    
    if (Array.isArray(servicios) && servicios.length > 0) {
      console.log(`✅ Se cargaron ${servicios.length} servicios`);
      servicios.forEach(s => {
        const option = document.createElement("option");
        option.value = s.id;
        option.textContent = `${s.codigoExpediente || 'N/A'} - ${s.nombreDifunto || 'Sin nombre'}`;
        option.dataset.expediente = s.codigoExpediente || "";
        option.dataset.difunto = s.nombreDifunto || "";
        option.dataset.paquete = s.paquete || "No especificado";
        select.appendChild(option);
      });
    } else {
      console.log("⚠️ No hay servicios disponibles");
      select.innerHTML = '<option value="">No hay servicios disponibles</option>';
    }
    
    // Listener para mostrar detalles del servicio seleccionado
    select.addEventListener("change", mostrarDetallesServicio);
    
  } catch (err) {
    console.error("❌ Error cargando servicios:", err);
    showAlert("Error cargando servicios: " + err.message, "danger");
  }
}

function mostrarDetallesServicio() {
  const select = document.getElementById("servicioIdFactura");
  const selectedOption = select.options[select.selectedIndex];
  const detalles = document.getElementById("detallesServicio");
  
  if (!detalles) return;
  
  if (!selectedOption || !selectedOption.value) {
    detalles.classList.add("hidden");
    return;
  }
  
  document.getElementById("nombreDifunto").textContent = selectedOption.dataset.difunto || "-";
  document.getElementById("codigoExpediente").textContent = selectedOption.dataset.expediente || "-";
  document.getElementById("paqueteServicio").textContent = selectedOption.dataset.paquete || "-";
  detalles.classList.remove("hidden");
}

// Generar factura - VERSIÓN CON MÚLTIPLES INTENTOS DE ENDPOINTS
async function generarFactura(event) {
  event.preventDefault();
  
  console.log("💾 Generando factura...");
  
  const servicioId = Number(document.getElementById("servicioIdFactura").value);
  const descuento = Number(document.getElementById("descuento").value) || 0;
  const metodoPago = document.getElementById("metodoPago").value;
  const observaciones = document.getElementById("observacionesFactura").value;
  
  if (!servicioId) {
    showAlert("Seleccione un servicio", "warning");
    return;
  }
  
  if (!metodoPago) {
    showAlert("Seleccione un método de pago", "warning");
    return;
  }
  
  // Preparar el body
  const body = {
    servicioId: servicioId,
    descuento: descuento,
    metodoPago: metodoPago,
    observaciones: observaciones || ""
  };
  
  console.log("📤 Body a enviar:", JSON.stringify(body, null, 2));
  
  // Lista de endpoints posibles que podría usar tu API
  const endpointsToTry = [
    "/Facturacion/generar",
    "/Facturacion/crear",
    "/Facturacion",
    "/Facturacion/nueva"
  ];
  
  let lastError = null;
  
  // Intentar con cada endpoint
  for (const endpoint of endpointsToTry) {
    try {
      console.log(`🔄 Intentando con endpoint: ${endpoint}`);
      
      const result = await apiRequest(endpoint, {
        method: "POST",
        body: JSON.stringify(body)
      });
      
      console.log("✅ Factura generada exitosamente:", result);
      
      closeModalFactura();
      showAlert("Factura generada correctamente", "success");
      
      // Recargar la lista de facturas
      await cargarFacturas();
      return; // Salir si tuvo éxito
      
    } catch (err) {
      console.warn(`⚠️ Falló con ${endpoint}:`, err.message);
      lastError = err;
      
      // Si no es error 405, no intentar más endpoints
      if (!err.message.includes("405")) {
        break;
      }
    }
  }
  
  // Si llegamos aquí, todos los endpoints fallaron
  console.error("❌ Todos los endpoints fallaron. Último error:", lastError);
  
  showAlert(
    `Error generando factura: ${lastError.message}\n\n` +
    `Por favor verifica:\n` +
    `1. Que el endpoint correcto sea POST /Facturacion/generar\n` +
    `2. Que el servidor esté ejecutándose\n` +
    `3. Los logs del servidor para más detalles`,
    "danger"
  );
}

// ========== MODAL PAGO ==========
function setupPagoModalHandlers() {
  const tbody = document.getElementById("facturasList");
  if (!tbody) {
    console.error("❌ No se encontró facturasList para delegación de eventos");
    return;
  }
  
  // Delegación: escucha clicks en la tabla para botones con clase .btn-pagar
  tbody.addEventListener("click", function (ev) {
    const btn = ev.target.closest(".btn-pagar");
    if (!btn) return;
    
    const id = btn.dataset.facturaId;
    const numero = btn.dataset.facturaNumero;
    const saldo = Number(btn.dataset.facturaSaldo);
    
    console.log("💵 Abrir modal pago:", { id, numero, saldo });
    abrirModalPago(id, numero, saldo);
  });

  // Formularios y botones del modal
  const formPago = document.getElementById("formPago");
  if (formPago) {
    formPago.addEventListener("submit", registrarPago);
  }

  const closeBtn = document.getElementById("closeModalPago");
  if (closeBtn) {
    closeBtn.addEventListener("click", cerrarModalPago);
  }

  const cancelBtn = document.getElementById("cancelPago");
  if (cancelBtn) {
    cancelBtn.addEventListener("click", cerrarModalPago);
  }

  // Prevención de teclado: ESC cierra el modal
  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") {
      const modal = document.getElementById("modalPago");
      if (modal && !modal.classList.contains("hidden")) {
        cerrarModalPago();
      }
    }
  });
}

function abrirModalPago(id, numero, saldo) {
  console.log("📝 Abriendo modal pago");
  
  document.getElementById("facturaPagoId").value = id;
  document.getElementById("facturaNumeroPago").textContent = numero;
  document.getElementById("facturaSaldoPago").textContent = formatCurrency(saldo);
  
  const montoInput = document.getElementById("montoPago");
  montoInput.value = "";
  montoInput.max = saldo;
  
  const modal = document.getElementById("modalPago");
  modal.classList.remove("hidden");
  modal.classList.add("show");
  
  setTimeout(() => montoInput.focus(), 120);
}

function cerrarModalPago() {
  console.log("❌ Cerrando modal pago");
  const modal = document.getElementById("modalPago");
  modal.classList.remove("show");
  modal.classList.add("hidden");
  
  // Limpiar formulario
  const form = document.getElementById("formPago");
  if (form) form.reset();
}

async function registrarPago(e) {
  e.preventDefault();
  
  console.log("💰 Registrando pago...");
  
  const facturaId = Number(document.getElementById("facturaPagoId").value);
  const monto = Number(document.getElementById("montoPago").value);
  const metodoPago = document.getElementById("metodoPagoRegistro").value;
  const observaciones = document.getElementById("obsPago").value;

  if (!facturaId || !monto || monto <= 0) {
    showAlert("Ingrese un monto válido", "warning");
    return;
  }
  
  if (!metodoPago) {
    showAlert("Seleccione un método de pago", "warning");
    return;
  }

  // Validar que no exceda el saldo
  const saldoText = document.getElementById("facturaSaldoPago").textContent
    .replace(/[^\d\.\,]/g, '')
    .replace(',', '.');
  const saldo = parseFloat(saldoText) || 0;
  
  if (monto > saldo) {
    showAlert("El monto ingresado excede el saldo pendiente", "warning");
    return;
  }

  try {
    const body = {
      facturaId: facturaId,
      monto: monto,
      metodoPago: metodoPago,
      numeroReferencia: "",
      observaciones: observaciones || ""
    };
    
    console.log("📤 Enviando pago:", body);

    const result = await apiRequest("/Facturacion/pagos", {
      method: "POST",
      body: JSON.stringify(body)
    });
    
    console.log("✅ Pago registrado:", result);

    cerrarModalPago();
    showAlert("Pago registrado correctamente", "success");
    
    // Recargar facturas
    await cargarFacturas();

  } catch (err) {
    console.error("❌ Error registrando pago:", err);
    showAlert(err.message || "Error registrando pago", "danger");
  }
}

// ========== FUNCIÓN LOGOUT ==========
function logout() {
  console.log("👋 Cerrando sesión...");
  localStorage.clear();
  window.location.href = "index.html";
}