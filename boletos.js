document.addEventListener("DOMContentLoaded", function() {
    // Elementos del DOM
    const listaBoletosIda = document.getElementById("lista-boletos-ida");
    const listaBoletosRegreso = document.getElementById("lista-boletos-regreso");
    const boletosDisponiblesContainer = document.getElementById("boletos-disponibles");
    const seleccionAsientos = document.getElementById("seleccion-asientos");
    const datosPasajero = document.getElementById("datos-pasajero");
    const infoBoleto = document.getElementById("info-boleto");
    const asientosContainer = document.getElementById("asientos");
    const tipoBoletoRadios = document.querySelectorAll('input[name="tipo-boleto"]');
    const fechaRegresoContainer = document.getElementById("fecha-regreso-container");
    const fechaRegresoInput = document.getElementById("fecha-regreso");
    const fechaIdaInput = document.getElementById("fecha-ida");
    const buscarBtn = document.getElementById("buscar");
    const origenInput = document.getElementById("origen");
    const destinoInput = document.getElementById("destino");
    const confirmarAsientoBtn = document.getElementById("confirmar-asiento");
    const cantidadBoletosSelect = document.getElementById("cantidad-boletos");
    const numeroPasajeroSpan = document.getElementById("numero-pasajero");
    const formularioBoletos = document.getElementById("formulario-boletos");
    const tituloCompra = document.querySelector(".rj");
    const resumenPagoContainer = document.getElementById("resumen-pago");
    const resumenDatos = document.getElementById("resumen-datos");
    const finalizarCompraBtn = document.getElementById("finalizar-compra");
    const numeroTarjetaInput = document.getElementById("numero-tarjeta");
    const nombreTarjetaInput = document.getElementById("nombre-tarjeta");
    const cvvInput = document.getElementById("cvv");
    const fechaExpiracionInput = document.getElementById("fecha-expiracion");

    // Variables de estado
    let boletoIdaSeleccionado = null;
    let boletoRegresoSeleccionado = null;
    let asientosSeleccionados = [];
    let pasajeros = [];
    let pasajeroActual = 0;
    let cantidadBoletos = 1;

    // Datos de ejemplo (boletos disponibles)
    const boletos = [
        { id: 1, origen: "Monterrey", destino: "Saltillo", fecha: "2025-03-24", salida: "08:00", precio: 250, asientos: 54 },
        { id: 2, origen: "Monterrey", destino: "Saltillo", fecha: "2025-03-25", salida: "12:00", precio: 250, asientos: 54 },
        { id: 3, origen: "Monterrey", destino: "Saltillo", fecha: "2025-03-24", salida: "16:00", precio: 250, asientos: 54 },
        { id: 4, origen: "Saltillo", destino: "Monterrey", fecha: "2025-03-25", salida: "09:00", precio: 250, asientos: 54 },
        { id: 5, origen: "Saltillo", destino: "Monterrey", fecha: "2025-03-25", salida: "13:00", precio: 250, asientos: 54 },
        { id: 6, origen: "Saltillo", destino: "Monterrey", fecha: "2025-03-25", salida: "17:00", precio: 250, asientos: 54 },
        { id: 7, origen: "Monterrey", destino: "Linares", fecha: "2025-03-24", salida: "07:00", precio: 350, asientos: 54 },
        { id: 8, origen: "Linares", destino: "Monterrey", fecha: "2025-03-25", salida: "15:00", precio: 350, asientos: 54 }
    ];

    // Inicialización
    function init() {
        actualizarVisibilidadFechaRegreso();
        setupFlatpickr();
        setupEventListeners();
        
        // Ocultar secciones que no son la inicial
        boletosDisponiblesContainer.style.display = "none";
        seleccionAsientos.style.display = "none";
        datosPasajero.style.display = "none";
        resumenPagoContainer.style.display = "none";
    }

    // Configuración de Flatpickr para fechas
    function setupFlatpickr() {
        flatpickr("#fecha-ida", {
            minDate: "today",
            dateFormat: "Y-m-d",
            locale: "es",
            onChange: function(selectedDates, dateStr) {
                if (document.querySelector('input[name="tipo-boleto"]:checked').value === "redondo") {
                    fechaRegresoInput._flatpickr.set("minDate", dateStr);
                }
            }
        });

        flatpickr("#fecha-regreso", {
            minDate: "today",
            dateFormat: "Y-m-d",
            locale: "es",
            disable: [
                function(date) {
                    const fechaIda = new Date(fechaIdaInput.value);
                    return date < fechaIda;
                }
            ]
        });
    }

    // Event listeners
    function setupEventListeners() {
        tipoBoletoRadios.forEach(radio => {
            radio.addEventListener("change", actualizarVisibilidadFechaRegreso);
        });

        buscarBtn.addEventListener("click", buscarBoletos);
        confirmarAsientoBtn.addEventListener("click", confirmarDatosPasajero);
        cantidadBoletosSelect.addEventListener("change", function() {
            cantidadBoletos = parseInt(this.value);
        });
        
        finalizarCompraBtn.addEventListener("click", procesarPago);
        
        // Nuevos event listeners para validación de pago
        numeroTarjetaInput.addEventListener("input", formatNumeroTarjeta);
        numeroTarjetaInput.addEventListener("blur", validarNumeroTarjeta);
        nombreTarjetaInput.addEventListener("blur", validarNombreTarjeta);
        cvvInput.addEventListener("blur", validarCVV);
        fechaExpiracionInput.addEventListener("blur", validarFechaExpiracion);
    }

    // Función para formatear el número de tarjeta (agrupar cada 4 dígitos)
    function formatNumeroTarjeta(e) {
        let value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
        let formatted = '';
        
        for (let i = 0; i < value.length; i++) {
            if (i > 0 && i % 4 === 0) {
                formatted += ' ';
            }
            formatted += value[i];
        }
        
        e.target.value = formatted;
    }

    // Función para validar número de tarjeta (16 dígitos)
    function validarNumeroTarjeta() {
        const value = numeroTarjetaInput.value.replace(/\s+/g, '');
        const isValid = /^\d{16}$/.test(value);
        
        if (!isValid) {
            mostrarError(numeroTarjetaInput, "El número de tarjeta debe tener 16 dígitos");
            return false;
        }
        
        limpiarError(numeroTarjetaInput);
        return true;
    }

    // Función para validar nombre en tarjeta (nombre completo)
    function validarNombreTarjeta() {
        const value = nombreTarjetaInput.value.trim();
        const isValid = value.split(' ').length >= 2 && /^[a-zA-Z\s]+$/.test(value);
        
        if (!isValid) {
            mostrarError(nombreTarjetaInput, "Ingrese el nombre completo como aparece en la tarjeta");
            return false;
        }
        
        limpiarError(nombreTarjetaInput);
        return true;
    }

    // Función para validar CVV (3 dígitos)
    function validarCVV() {
        const value = cvvInput.value;
        const isValid = /^\d{3}$/.test(value);
        
        if (!isValid) {
            mostrarError(cvvInput, "El CVV debe tener exactamente 3 dígitos");
            return false;
        }
        
        limpiarError(cvvInput);
        return true;
    }

    // Función para validar fecha de expiración
    function validarFechaExpiracion() {
        const value = fechaExpiracionInput.value;
        const [month, year] = value.split('/').map(Number);
        
        if (!/^\d{2}\/\d{2}$/.test(value)) {
            mostrarError(fechaExpiracionInput, "Formato inválido (MM/YY)");
            return false;
        }
        
        const currentDate = new Date();
        const currentYear = currentDate.getFullYear() % 100;
        const currentMonth = currentDate.getMonth() + 1;
        
        // Validar mes (1-12)
        if (month < 1 || month > 12) {
            mostrarError(fechaExpiracionInput, "Mes inválido");
            return false;
        }
        
        // Verificar si la tarjeta está expirada
        if (year < currentYear || (year === currentYear && month < currentMonth)) {
            mostrarError(fechaExpiracionInput, "La tarjeta está expirada");
            return false;
        }
        
        limpiarError(fechaExpiracionInput);
        return true;
    }

    // Funciones auxiliares para mostrar/limpiar errores
    function mostrarError(input, mensaje) {
        limpiarError(input);
        
        const error = document.createElement('div');
        error.className = 'error-mensaje';
        error.textContent = mensaje;
        error.style.color = 'red';
        error.style.fontSize = '0.8rem';
        error.style.marginTop = '5px';
        
        input.parentNode.appendChild(error);
        input.classList.add('error-input');
    }

    function limpiarError(input) {
        const error = input.parentNode.querySelector('.error-mensaje');
        if (error) {
            input.parentNode.removeChild(error);
        }
        input.classList.remove('error-input');
    }

    // Actualizar visibilidad de fecha de regreso
    function actualizarVisibilidadFechaRegreso() {
        const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
        fechaRegresoContainer.style.display = tipoBoleto === "redondo" ? "block" : "none";
        fechaRegresoInput.disabled = tipoBoleto !== "redondo";
        if (tipoBoleto !== "redondo") fechaRegresoInput.value = "";
    }

    // Búsqueda de boletos
    function buscarBoletos() {
        const origen = origenInput.value;
        const destino = destinoInput.value;
        const fechaIda = fechaIdaInput.value;
        const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
        const fechaRegreso = tipoBoleto === "redondo" ? fechaRegresoInput.value : null;
        cantidadBoletos = parseInt(cantidadBoletosSelect.value);

        // Validaciones
        if (!origen || !destino || !fechaIda) {
            alert("Complete todos los campos obligatorios");
            return;
        }

        if (tipoBoleto === "redondo" && !fechaRegreso) {
            alert("Seleccione fecha de regreso para viaje redondo");
            return;
        }

        if (origen === destino) {
            alert("El origen y destino no pueden ser iguales");
            return;
        }

        // Resetear selecciones anteriores
        boletoIdaSeleccionado = null;
        boletoRegresoSeleccionado = null;
        asientosSeleccionados = [];
        pasajeros = [];
        pasajeroActual = 0;

        // Filtrar boletos de ida
        const boletosIda = boletos.filter(b => 
            b.origen === origen && 
            b.destino === destino && 
            b.fecha === fechaIda
        );

        if (boletosIda.length === 0) {
            alert(`No hay boletos disponibles de ${origen} a ${destino} en esa fecha`);
            return;
        }

        // Mostrar resultados
        formularioBoletos.style.display = "none";
        tituloCompra.style.display = "none";
        boletosDisponiblesContainer.style.display = "block";
        listaBoletosRegreso.parentElement.style.display = "none";
        seleccionAsientos.style.display = "none";
        datosPasajero.style.display = "none";
        resumenPagoContainer.style.display = "none";
        mostrarBoletos(boletosIda, listaBoletosIda);
    }

    // Mostrar boletos en contenedor
    function mostrarBoletos(boletos, contenedor, esRegreso = false) {
        contenedor.innerHTML = "";

        if (boletos.length === 0) {
            contenedor.innerHTML = `<p class="no-boletos">No hay boletos disponibles</p>`;
            return;
        }

        boletos.forEach(boleto => {
            const div = document.createElement("div");
            div.classList.add("boleto");
            div.innerHTML = `
                <div class="boleto-info">
                    <h4>${boleto.origen} → ${boleto.destino}</h4>
                    <p><strong>Fecha:</strong> ${formatearFecha(boleto.fecha)}</p>
                    <p><strong>Salida:</strong> ${boleto.salida}</p>
                    <p class="precio"><strong>Precio:</strong> $${boleto.precio}</p>
                    <p><strong>Asientos disponibles:</strong> ${boleto.asientos}</p>
                </div>
                <button class="elegir" 
                        data-id="${boleto.id}"
                        data-precio="${boleto.precio}"
                        data-fecha="${boleto.fecha}"
                        data-salida="${boleto.salida}"
                        data-origen="${boleto.origen}"
                        data-destino="${boleto.destino}"
                        data-es-regreso="${esRegreso}">
                    Elegir
                </button>
            `;
            contenedor.appendChild(div);
        });

        // Eventos para botones "Elegir"
        document.querySelectorAll(".elegir").forEach(boton => {
            boton.addEventListener("click", function() {
                const boletoData = {
                    id: this.getAttribute("data-id"),
                    precio: this.getAttribute("data-precio"),
                    fecha: this.getAttribute("data-fecha"),
                    salida: this.getAttribute("data-salida"),
                    origen: this.getAttribute("data-origen"),
                    destino: this.getAttribute("data-destino"),
                    esRegreso: this.getAttribute("data-es-regreso") === "true"
                };

                if (boletoData.esRegreso) {
                    boletoRegresoSeleccionado = boletoData;
                    mostrarSeleccionAsientos();
                } else {
                    boletoIdaSeleccionado = boletoData;
                    const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
                    
                    if (tipoBoleto === "redondo") {
                        // Buscar boletos de regreso (origen/destino invertidos)
                        const boletosRegreso = boletos.filter(b => 
                            b.origen === boletoIdaSeleccionado.destino &&
                            b.destino === boletoIdaSeleccionado.origen &&
                            b.fecha === fechaRegresoInput.value
                        );
                        
                        if (boletosRegreso.length === 0) {
                            alert("No hay boletos disponibles para el viaje de regreso en la fecha seleccionada");
                            return;
                        }
                        
                        listaBoletosRegreso.parentElement.style.display = "block";
                        mostrarBoletos(boletosRegreso, listaBoletosRegreso, true);
                    } else {
                        mostrarSeleccionAsientos();
                    }
                }
            });
        });
    }

    // Mostrar selección de asientos
    function mostrarSeleccionAsientos() {
        boletosDisponiblesContainer.style.display = "none";
        seleccionAsientos.style.display = "block";
        datosPasajero.style.display = "none";
        resumenPagoContainer.style.display = "none";

        const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
        numeroPasajeroSpan.textContent = `(${pasajeroActual + 1} de ${cantidadBoletos})`;
        
        let mensaje = `
            <strong>Viaje de ida:</strong><br>
            ${boletoIdaSeleccionado.origen} → ${boletoIdaSeleccionado.destino}<br>
            Fecha: ${formatearFecha(boletoIdaSeleccionado.fecha)}<br>
            Salida: ${boletoIdaSeleccionado.salida}<br>
            Precio: $${boletoIdaSeleccionado.precio}<br>
            <strong>Pasajero ${pasajeroActual + 1} de ${cantidadBoletos}</strong>
        `;
        
        if (tipoBoleto === "redondo" && boletoRegresoSeleccionado) {
            mensaje += `<br><br><strong>Viaje de regreso:</strong><br>
            ${boletoRegresoSeleccionado.origen} → ${boletoRegresoSeleccionado.destino}<br>
            Fecha: ${formatearFecha(boletoRegresoSeleccionado.fecha)}<br>
            Salida: ${boletoRegresoSeleccionado.salida}<br>
            Precio: $${boletoRegresoSeleccionado.precio}`;
        }

        infoBoleto.innerHTML = mensaje;
        generarAsientos();
    }

    function generarAsientos() {
        asientosContainer.innerHTML = "";
        
        // Generar 40 asientos
        for (let i = 1; i <= 40; i++) {
            const asiento = document.createElement("div");
            asiento.className = "asiento";
            asiento.textContent = i;
            asiento.dataset.numero = i;
            
            // Verificar si el asiento ya está seleccionado para otro pasajero
            const yaSeleccionado = asientosSeleccionados.includes(i.toString());
            
            // Ejemplo: 25% de asientos ocupados
            if (i % 4 === 0 || i % 7 === 0 || yaSeleccionado) {
                asiento.classList.add(yaSeleccionado ? "seleccionado" : "ocupado");
            } else {
                asiento.classList.add("disponible");
                asiento.addEventListener("click", function() {
                    // Deseleccionar primero
                    document.querySelectorAll(".asiento.seleccionado-actual").forEach(a => {
                        a.classList.remove("seleccionado-actual");
                    });
                    // Seleccionar nuevo
                    this.classList.add("seleccionado-actual");
                    asientosSeleccionados[pasajeroActual] = this.dataset.numero;
                    document.getElementById("datos-pasajero").style.display = "block";
                });
            }
            asientosContainer.appendChild(asiento);
        }
    }

    // Confirmar datos del pasajero
    function confirmarDatosPasajero() {
        const nombre = document.getElementById("nombre").value;
        const apellidos = document.getElementById("apellidos").value;
        
        if (!nombre || !apellidos) {
            alert("Por favor complete todos los datos del pasajero");
            return;
        }

        // Guardar datos del pasajero
        pasajeros[pasajeroActual] = {
            nombre,
            apellidos,
            asientoIda: asientosSeleccionados[pasajeroActual],
            asientoRegreso: boletoRegresoSeleccionado ? asientosSeleccionados[pasajeroActual] : null
        };

        // Limpiar formulario
        document.getElementById("nombre").value = "";
        document.getElementById("apellidos").value = "";

        // Pasar al siguiente pasajero o mostrar resumen
        pasajeroActual++;
        
        if (pasajeroActual < cantidadBoletos) {
            // Mostrar selección de asientos para el siguiente pasajero
            mostrarSeleccionAsientos();
        } else {
            // Todos los pasajeros han sido procesados
            mostrarResumenPago();
        }
    }

    // Mostrar resumen y formulario de pago
    function mostrarResumenPago() {
        seleccionAsientos.style.display = "none";
        datosPasajero.style.display = "none";
        resumenPagoContainer.style.display = "block";
        
        const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
        
        let html = `
            <p><strong>Tipo de boleto:</strong> ${tipoBoleto === 'redondo' ? 'Redondo' : 'Sencillo'}</p>
            <h3>Detalles del viaje</h3>
            <p><strong>Viaje de ida:</strong></p>
            <p>${boletoIdaSeleccionado.origen} → ${boletoIdaSeleccionado.destino}</p>
            <p>Fecha: ${formatearFecha(boletoIdaSeleccionado.fecha)}</p>
            <p>Salida: ${boletoIdaSeleccionado.salida}</p>
            <p>Precio unitario: $${boletoIdaSeleccionado.precio}</p>
        `;
        
        if (tipoBoleto === "redondo" && boletoRegresoSeleccionado) {
            html += `
                <p><strong>Viaje de regreso:</strong></p>
                <p>${boletoRegresoSeleccionado.destino} → ${boletoRegresoSeleccionado.origen}</p>
                <p>Fecha: ${formatearFecha(boletoRegresoSeleccionado.fecha)}</p>
                <p>Salida: ${boletoRegresoSeleccionado.salida}</p>
                <p>Precio unitario: $${boletoRegresoSeleccionado.precio}</p>
            `;
        }
        
        // Mostrar información de pasajeros
        html += `<h3>Pasajeros</h3>`;
        pasajeros.forEach((pasajero, index) => {
            html += `
                <div class="pasajero">
                    <p><strong>Pasajero ${index + 1}:</strong> ${pasajero.nombre} ${pasajero.apellidos}</p>
                    <p><strong>Asiento ida:</strong> ${pasajero.asientoIda}</p>
                    ${pasajero.asientoRegreso ? `<p><strong>Asiento regreso:</strong> ${pasajero.asientoRegreso}</p>` : ''}
                </div>
            `;
        });
        
        // Calcular total
        let total = parseInt(boletoIdaSeleccionado.precio) * cantidadBoletos;
        if (tipoBoleto === "redondo" && boletoRegresoSeleccionado) {
            total += parseInt(boletoRegresoSeleccionado.precio) * cantidadBoletos;
        }
        
        html += `
            <div class="total">
                <strong>Total a pagar:</strong> $${total}
            </div>
        `;
        
        resumenDatos.innerHTML = html;
    }

    // Procesar pago y finalizar reserva 
    function procesarPago() {
        // Validar todos los campos de pago primero
        const isValid = [
            validarNumeroTarjeta(),
            validarNombreTarjeta(),
            validarCVV(),
            validarFechaExpiracion()
        ].every(valid => valid === true);
        
        if (!isValid) {
            alert("Por favor corrija los errores en el formulario de pago");
            return;
        }
        
        // Mostrar mensaje de procesamiento
        finalizarCompraBtn.disabled = true;
        finalizarCompraBtn.textContent = "Procesando...";
        
        // Simular procesamiento de pago (2 segundos)
        setTimeout(() => {
            const usuario = JSON.parse(localStorage.getItem("usuario"));
            if (!usuario) {
                alert('Error de sesión. Por favor inicie sesión nuevamente.');
                window.location.href = 'signin.html';
                return;
            }
    
            const reservaData = {
                id: Date.now(),
                fechaReserva: new Date().toISOString(),
                tipoBoleto: document.querySelector('input[name="tipo-boleto"]:checked').value,
                boletoIda: boletoIdaSeleccionado,
                boletoRegreso: boletoRegresoSeleccionado,
                pasajeros: pasajeros,
                cantidadBoletos: cantidadBoletos,
                estado: 'Pagado',
                numeroReserva: 'RSV-' + Math.floor(Math.random() * 1000000),
                metodoPago: document.getElementById("tipo-tarjeta").value,
                // Agregar datos de pago
                pago: {
                    ultimos4digitos: numeroTarjetaInput.value.slice(-4),
                    tipoTarjeta: document.getElementById("tipo-tarjeta").value,
                    fechaExpiracion: fechaExpiracionInput.value
                }
            };
    
            // Guardar reserva (código existente)
            if (!usuario.reservas) {
                usuario.reservas = [];
            }
            usuario.reservas.unshift(reservaData);
            localStorage.setItem("usuario", JSON.stringify(usuario));
            localStorage.setItem('ultimaReserva', JSON.stringify(reservaData));
    
            // Mostrar confirmación
            mostrarConfirmacion(reservaData);
            
            // Redirigir después de 3 segundos
            setTimeout(() => {
                window.location.href = "reservas.html";
            }, 3000);
            
        }, 2000);
    }

    // Función para mostrar confirmación temporal
    function mostrarConfirmacion(reserva) {
        // Crear elemento de confirmación
        const confirmacion = document.createElement('div');
        confirmacion.className = 'confirmacion-pago';
        confirmacion.innerHTML = `
            <div class="confirmacion-contenido">
                <h3>¡Pago exitoso!</h3>
                <p>Reserva #${reserva.numeroReserva} confirmada</p>
                <p>Redirigiendo a Mis Reservas...</p>
                <div class="spinner"></div>
            </div>
        `;
        
        // Estilos para la confirmación
        const estilos = document.createElement('style');
        estilos.textContent = `
            .confirmacion-pago {
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0,0,0,0.8);
                display: flex;
                justify-content: center;
                align-items: center;
                z-index: 1000;
            }
            .confirmacion-contenido {
                background: white;
                padding: 2rem;
                border-radius: 8px;
                text-align: center;
                max-width: 400px;
            }
            .spinner {
                border: 4px solid rgba(0,0,0,0.1);
                border-radius: 50%;
                border-top: 4px solid #29F2A9;
                width: 30px;
                height: 30px;
                animation: spin 1s linear infinite;
                margin: 20px auto;
            }
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        `;
        
        document.head.appendChild(estilos);
        document.body.appendChild(confirmacion);
    }

    // Función para formatear fechas
    function formatearFecha(fechaStr) {
        const opciones = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        return new Date(fechaStr).toLocaleDateString('es-ES', opciones);
    }

    // Iniciar la aplicación
    init();
});