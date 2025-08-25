// ===== CARRITO.JS - Gestión del carrito con localStorage y modales profesionales =====

const CARRITO_KEY = 'superprecios_carrito';
const CARRITO_EXPIRY_HOURS = 2;

// Variable global para almacenar el ID del producto a eliminar
let productoAEliminar = null;

// ===== OPERACIONES DEL CARRITO =====

function agregarAlCarrito(productoId, cantidad = 1) {
    // Mostrar indicador de carga
    mostrarCargando('Agregando producto...');

    $.post('/Carrito/Agregar', {
        productoId: productoId,
        cantidad: cantidad
    })
        .done(function (response) {
            ocultarCargando();

            if (response.exito) {
                // Actualizar localStorage con datos del servidor
                guardarCarritoEnLocalStorage(response.carrito);

                // Actualizar UI desde localStorage
                actualizarMiniCarrito();
                mostrarNotificacion(response.mensaje, 'success');
            } else {
                mostrarNotificacion(response.mensaje, 'error');
            }
        })
        .fail(function () {
            ocultarCargando();
            mostrarNotificacion('Error de conexión', 'error');
        });
}

function modificarCantidad(productoId, nuevaCantidad) {
    if (nuevaCantidad < 1) {
        quitarLinea(productoId);
        return;
    }

    // Mostrar indicador de carga
    mostrarCargando('Actualizando cantidad...');

    $.post('/Carrito/ModificarCantidad', {
        productoId: productoId,
        cantidad: nuevaCantidad
    })
        .done(function (response) {
            ocultarCargando();

            if (response.exito) {
                // Actualizar localStorage con datos del servidor
                guardarCarritoEnLocalStorage(response.carrito);

                // Actualizar UI desde localStorage
                if (window.location.pathname === '/Carrito') {
                    mostrarCarritoDesdeLocalStorage();
                }
                actualizarMiniCarrito();
                mostrarNotificacion(response.mensaje, 'success');
            } else {
                mostrarNotificacion(response.mensaje, 'error');
            }
        })
        .fail(function () {
            ocultarCargando();
            mostrarNotificación('Error de conexión', 'error');
        });
}

// ===== FUNCIONES CON MODALES PROFESIONALES =====

function quitarLinea(productoId) {
    // Guardar el ID del producto para usar en la confirmación
    productoAEliminar = productoId;

    // Mostrar modal de confirmación
    $('#modal-eliminar-producto').modal('show');
}

function limpiarCarrito() {
    // Mostrar modal de confirmación
    $('#modal-vaciar-carrito').modal('show');
}

// ===== FUNCIONES QUE EJECUTAN LAS ACCIONES REALES =====

function ejecutarEliminarProducto(productoId) {
    mostrarCargando('Eliminando producto...');

    $.post('/Carrito/Quitar', { productoId: productoId })
        .done(function (response) {
            ocultarCargando();

            if (response.exito) {
                // Actualizar localStorage con datos del servidor
                guardarCarritoEnLocalStorage(response.carrito);

                // Actualizar UI desde localStorage
                if (window.location.pathname === '/Carrito') {
                    mostrarCarritoDesdeLocalStorage();
                }
                actualizarMiniCarrito();
                mostrarNotificacion(response.mensaje, 'info');
            } else {
                mostrarNotificacion(response.mensaje, 'error');
            }
        })
        .fail(function () {
            ocultarCargando();
            mostrarNotificacion('Error de conexión', 'error');
        });
}

function ejecutarVaciarCarrito() {
    mostrarCargando('Vaciando carrito...');

    $.post('/Carrito/Limpiar')
        .done(function (response) {
            ocultarCargando();

            if (response.exito) {
                // Limpiar localStorage
                localStorage.removeItem(CARRITO_KEY);

                // Actualizar UI
                if (window.location.pathname === '/Carrito') {
                    mostrarCarritoDesdeLocalStorage();
                }
                actualizarMiniCarrito();
                mostrarNotificacion(response.mensaje, 'info');
            } else {
                mostrarNotificacion(response.mensaje, 'error');
            }
        })
        .fail(function () {
            ocultarCargando();
            mostrarNotificacion('Error de conexión', 'error');
        });
}

// ===== MANEJO DE LOCALSTORAGE =====

function guardarCarritoEnLocalStorage(carrito) {
    try {
        const carritoConExpiry = {
            ...carrito,
            timestamp: new Date().getTime()
        };
        localStorage.setItem(CARRITO_KEY, JSON.stringify(carritoConExpiry));
        console.log('Carrito guardado en localStorage:', carritoConExpiry);
    } catch (error) {
        console.error('Error guardando en localStorage:', error);
    }
}

function obtenerCarritoDeLocalStorage() {
    try {
        const carritoStr = localStorage.getItem(CARRITO_KEY);
        if (!carritoStr) return null;

        const carrito = JSON.parse(carritoStr);

        // Verificar expiración
        const ahora = new Date().getTime();
        const expiry = CARRITO_EXPIRY_HOURS * 60 * 60 * 1000;

        if (carrito.timestamp && (ahora - carrito.timestamp > expiry)) {
            localStorage.removeItem(CARRITO_KEY);
            console.log('Carrito expirado, removido del localStorage');
            return null;
        }

        return carrito;
    } catch (error) {
        console.error('Error leyendo localStorage:', error);
        localStorage.removeItem(CARRITO_KEY);
        return null;
    }
}

function limpiarCarritoLocalStorage() {
    localStorage.removeItem(CARRITO_KEY);
    console.log('Carrito limpiado del localStorage');
}

// ===== ACTUALIZACIÓN DE UI =====

function mostrarCarritoDesdeLocalStorage() {
    const carrito = obtenerCarritoDeLocalStorage();

    if (!carrito) {
        mostrarCarritoVacio();
        return;
    }

    if (carrito.tieneProductos) {
        mostrarCarritoConProductos(carrito);
    } else {
        mostrarCarritoVacio();
    }
}

function mostrarCarritoConProductos(carrito) {
    const html = `
        <div class="row">
            <div class="col-lg-8">
                <div class="card">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h5 class="mb-0">Productos (${carrito.cantidadTotal} items)</h5>
                        <button type="button" class="btn btn-outline-danger btn-sm" onclick="limpiarCarrito()">
                            <i class="bi bi-trash me-1"></i>Vaciar Carrito
                        </button>
                    </div>
                    <div class="card-body p-0">
                        ${carrito.lineas.map(linea => generarHtmlLinea(linea)).join('')}
                    </div>
                </div>
            </div>
            <div class="col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Resumen de Compra</h5>
                    </div>
                    <div class="card-body">
                        <div class="d-flex justify-content-between mb-2">
                            <span>Subtotal:</span>
                            <span>$${carrito.total.toFixed(2)}</span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Productos:</span>
                            <span>${carrito.cantidadTotal} items</span>
                        </div>
                        <hr>
                        <div class="d-flex justify-content-between mb-3">
                            <strong>Total:</strong>
                            <strong class="text-success">$${carrito.total.toFixed(2)}</strong>
                        </div>

                        <a href="/Recomendaciones" class="btn btn-warning w-100 mb-2">
                            <i class="bi bi-trophy me-1"></i>Ver Mejores Precios
                        </a>

                        <button class="btn btn-success w-100" disabled>
                            <i class="bi bi-credit-card me-1"></i>Proceder al Pago
                        </button>
                        <small class="text-muted d-block mt-2 text-center">
                            Funcionalidad próximamente
                        </small>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#carrito-contenido').html(html);
}

function mostrarCarritoVacio() {
    const html = `
        <div class="text-center py-5">
            <i class="bi bi-cart-x display-1 text-muted"></i>
            <h3 class="mt-3">Tu carrito está vacío</h3>
            <p class="text-muted">Agrega algunos productos para empezar a comprar</p>
            <a href="/Productos" class="btn btn-primary">
                <i class="bi bi-shop me-1"></i>Ir a Productos
            </a>
        </div>
    `;

    $('#carrito-contenido').html(html);
}

function generarHtmlLinea(linea) {
    const imagenHtml = linea.imagenUrl
        ? `<img src="${linea.imagenUrl}" alt="${linea.nombre}" class="img-fluid rounded" style="max-height: 80px; object-fit: contain;">`
        : `<div class="bg-light rounded d-flex align-items-center justify-content-center" style="width: 80px; height: 80px;">
             <i class="bi bi-image text-muted"></i>
           </div>`;

    return `
        <div class="border-bottom p-3" data-producto-id="${linea.productoId}">
            <div class="row align-items-center">
                <div class="col-md-2 text-center">
                    ${imagenHtml}
                </div>
                <div class="col-md-4">
                    <h6 class="mb-1">${linea.nombre}</h6>
                    ${linea.supermercadoMenorPrecio ? `<small class="text-muted">Mejor precio en: ${linea.supermercadoMenorPrecio}</small>` : ''}
                </div>
                <div class="col-md-2 text-center">
                    <strong>$${linea.precioUnitario.toFixed(2)}</strong>
                </div>
                <div class="col-md-2">
                    <div class="input-group input-group-sm">
                        <button type="button" class="btn btn-outline-secondary" onclick="modificarCantidad(${linea.productoId}, ${linea.cantidad - 1})">
                            <i class="bi bi-dash"></i>
                        </button>
                        <input type="number" class="form-control text-center" value="${linea.cantidad}" readonly>
                        <button type="button" class="btn btn-outline-secondary" onclick="modificarCantidad(${linea.productoId}, ${linea.cantidad + 1})">
                            <i class="bi bi-plus"></i>
                        </button>
                    </div>
                </div>
                <div class="col-md-2 text-end">
                    <div class="d-flex flex-column align-items-end">
                        <strong class="text-success">$${linea.subtotal.toFixed(2)}</strong>
                        <button type="button" class="btn btn-outline-danger btn-sm mt-1" onclick="quitarLinea(${linea.productoId})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
}

function actualizarMiniCarrito() {
    const carrito = obtenerCarritoDeLocalStorage();

    if (!carrito || !carrito.tieneProductos) {
        $('#mini-carrito-container').html(`
        <a class="nav-link d-flex align-items-center p-0" href="/Carrito">
            <i class="bi bi-cart3 fs-4"></i>
            <span class="ms-2 d-none d-lg-inline">0 items</span>
        </a>
    `);
        return;
    }

    const productosHtml = carrito.lineas.slice(0, 3).map(linea => {
        const imagenHtml = linea.imagenUrl
            ? `<img src="${linea.imagenUrl}" alt="${linea.nombre}" class="me-2 rounded" style="width: 40px; height: 40px; object-fit: contain;">`
            : `<div class="me-2 bg-light rounded d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
                 <i class="bi bi-image text-muted"></i>
               </div>`;

        return `
            <div class="dropdown-item-text">
                <div class="d-flex align-items-center">
                    ${imagenHtml}
                    <div class="flex-grow-1">
                        <div class="fw-semibold small">${linea.nombre}</div>
                        <div class="text-muted small">Cantidad: ${linea.cantidad}</div>
                    </div>
                </div>
            </div>
        `;
    }).join('');

    const miniCarritoHtml = `
        <div class="nav-item dropdown">
            <a class="nav-link dropdown-toggle d-flex align-items-center p-0" 
               href="#" id="carritoDropdown" role="button" 
               data-bs-toggle="dropdown" aria-expanded="false">
                <div class="position-relative">
                    <i class="bi bi-cart3 fs-4"></i>
                    <span class="badge bg-danger position-absolute top-0 start-100 translate-middle rounded-pill">
                        ${carrito.cantidadTotal}
                    </span>
                </div>
                <span class="ms-2 d-none d-lg-inline">${carrito.cantidadTotal} items</span>
            </a>
            
            <div class="dropdown-menu dropdown-menu-end shadow p-0" style="min-width: 350px;">
                <div class="dropdown-header">
                    <strong>Mi Carrito (${carrito.cantidadTipos} productos)</strong>
                </div>
                
                ${productosHtml}
                
                <div class="dropdown-divider"></div>
                <div class="dropdown-item-text">
                    <div class="d-flex justify-content-between">
                        <strong>${carrito.cantidadTotal} productos en total</strong>
                    </div>
                </div>
                <div class="dropdown-divider"></div>
                <div class="p-2">
                    <a href="/Carrito" class="btn btn-primary w-100 btn-sm">
                        <i class="bi bi-cart-check me-1"></i>Ver Carrito Completo
                    </a>
                </div>
            </div>
        </div>
    `;

    $('#mini-carrito-container').html(miniCarritoHtml);
}

// ===== SINCRONIZACIÓN CON SERVIDOR =====

function sincronizarConServidor() {
    $.get('/Carrito/Estado')
        .done(function (response) {
            if (response.exito) {
                // Actualizar localStorage con datos frescos del servidor
                guardarCarritoEnLocalStorage(response.carrito);

                // Actualizar UI si estamos en la página del carrito
                if (window.location.pathname === '/Carrito') {
                    mostrarCarritoDesdeLocalStorage();
                }

                // Actualizar mini-carrito
                actualizarMiniCarrito();

                console.log('Sincronización con servidor completada');
            }
        })
        .fail(function () {
            console.warn('No se pudo sincronizar con el servidor');
            // Si hay error, seguir usando localStorage
            actualizarMiniCarrito();
        });
}

// ===== INDICADORES DE CARGA =====

function mostrarCargando(mensaje = 'Procesando...') {
    // Remover indicador previo si existe
    $('#loading-indicator').remove();

    const loadingHtml = `
        <div id="loading-indicator" class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" 
             style="background-color: rgba(0,0,0,0.5); z-index: 9999;">
            <div class="card">
                <div class="card-body text-center">
                    <div class="spinner-border text-primary mb-2" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <div>${mensaje}</div>
                </div>
            </div>
        </div>
    `;

    $('body').append(loadingHtml);
}

function ocultarCargando() {
    $('#loading-indicator').fadeOut(200, function () {
        $(this).remove();
    });
}

// ===== NOTIFICACIONES =====

function mostrarNotificacion(mensaje, tipo = 'info') {
    const alertClass = tipo === 'success' ? 'alert-success' :
        tipo === 'error' ? 'alert-danger' : 'alert-info';

    const notification = $(`
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            ${mensaje}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);

    $('body').append(notification);
    setTimeout(() => {
        notification.alert('close');
    }, 3000);
}

// ===== EVENT HANDLERS =====

// Agregar al carrito desde botones de productos
$(document).on('click', '.btn-agregar-carrito', function () {
    const productoId = $(this).data('producto-id');
    const cantidad = $(this).data('cantidad') || 1;
    agregarAlCarrito(productoId, cantidad);
});

// Manejar cantidad desde inputs numéricos
$(document).on('change', '.cantidad-input', function () {
    const productoId = $(this).data('producto-id');
    const cantidad = parseInt($(this).val()) || 1;
    modificarCantidad(productoId, cantidad);
});

// ===== INICIALIZACIÓN =====

$(document).ready(function () {
    console.log('Carrito.js inicializado');

    // Cargar mini-carrito desde localStorage inmediatamente
    actualizarMiniCarrito();

    // Si estamos en la página del carrito, cargar contenido
    if (window.location.pathname === '/Carrito') {
        mostrarCarritoDesdeLocalStorage();
    }

    // EVENT HANDLERS PARA LOS MODALES
    $('#btn-confirmar-eliminar').on('click', function () {
        $('#modal-eliminar-producto').modal('hide');
        if (productoAEliminar) {
            ejecutarEliminarProducto(productoAEliminar);
            productoAEliminar = null; // Limpiar la variable
        }
    });

    $('#btn-confirmar-vaciar').on('click', function () {
        $('#modal-vaciar-carrito').modal('hide');
        ejecutarVaciarCarrito();
    });

    // Sincronizar con servidor en background (sin bloquear UI)
    setTimeout(sincronizarConServidor, 500);
});

// Sincronizar periódicamente (cada 5 minutos)
setInterval(sincronizarConServidor, 5 * 60 * 1000);

// Limpiar localStorage al cerrar sesión (opcional)
$(window).on('beforeunload', function () {
    // Comentado - mantener carrito entre sesiones
    // limpiarCarritoLocalStorage();
});