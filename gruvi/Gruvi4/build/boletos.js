document.addEventListener("DOMContentLoaded", function () {
    const listaBoletos = document.getElementById("lista-boletos");
    const boletosDisponibles = document.getElementById("boletos-disponibles");
    const seleccionAsientos = document.getElementById("seleccion-asientos");
    const infoBoleto = document.getElementById("info-boleto");
    const asientosContainer = document.getElementById("asientos");
    const tipoBoletoRadios = document.querySelectorAll('input[name="tipo-boleto"]');
    const fechaRegresoContainer = document.getElementById("fecha-regreso-container");
    const fechaRegresoInput = document.getElementById("fecha-regreso");
    const fechaIdaInput = document.getElementById("fecha-ida");
    const buscarBtn = document.getElementById("buscar");

    // Inicializar Flatpickr
    flatpickr("#fecha-ida", {
        minDate: "today",
        dateFormat: "Y-m-d",
        locale: "es",
        onChange: function (selectedDates, dateStr) {
            if (document.querySelector('input[name="tipo-boleto"]:checked').value === "redondo") {
                fechaRegresoContainer.style.display = "block";
                fechaRegresoInput.disabled = false;
                flatpickr("#fecha-regreso", {
                    minDate: dateStr,
                    dateFormat: "Y-m-d",
                    locale: "es"
                });
            }
        }
    });

    // Manejo de tipo de boleto (sencillo o redondo)
    tipoBoletoRadios.forEach(radio => {
        radio.addEventListener("change", function () {
            if (this.value === "redondo") {
                fechaRegresoContainer.style.display = "block";
                fechaRegresoInput.disabled = false;
            } else {
                fechaRegresoContainer.style.display = "none";
                fechaRegresoInput.value = "";
                fechaRegresoInput.disabled = true;
            }
        });
    });

    // Boletos de ejemplo
    const boletos = [
        { hora: "20:00-22:00", precio: "$1,500", duracion: "2 Hrs Directo" },
        { hora: "22:00-24:00", precio: "$1,500", duracion: "2 Hrs Directo" },
        { hora: "13:00-15:00", precio: "$1,500", duracion: "2 Hrs Directo" }
    ];

    function mostrarBoletos() {
        listaBoletos.innerHTML = "";

        boletos.forEach((boleto, index) => {
            const div = document.createElement("div");
            div.classList.add("boleto");
            div.innerHTML = `
                <span>${boleto.hora}</span>
                <span>${boleto.duracion}</span>
                <span>${boleto.precio}</span>
                <button class="elegir" id="boton-elegir" data-index="${index}">Elegir</button>
            `;
            listaBoletos.appendChild(div);
        });

        document.querySelectorAll(".elegir").forEach(boton => {
            boton.addEventListener("click", function () {
                const index = this.getAttribute("data-index");
                mostrarSeleccionAsientos(boletos[index]);
            });
        });
    }

    function mostrarSeleccionAsientos(boleto) {
        boletosDisponibles.style.display = "none";
        seleccionAsientos.style.display = "block";

        infoBoleto.textContent = `Boleto seleccionado: ${boleto.hora}, ${boleto.precio}, ${boleto.duracion}`;

        // Generar asientos
        asientosContainer.innerHTML = "";
        for (let i = 1; i <= 40; i++) {
            const asiento = document.createElement("div");
            asiento.classList.add("asiento");
            asiento.textContent = i;

            if (Math.random() > 0.7) {
                asiento.classList.add("ocupado");
            } else {
                asiento.classList.add("disponible");
                asiento.addEventListener("click", function () {
                    seleccionarAsiento(asiento);
                });
            }

            asientosContainer.appendChild(asiento);
        }
    }

    function seleccionarAsiento(asiento) {
        if (asiento.classList.contains("ocupado")) return;

        document.querySelectorAll(".asiento").forEach(a => a.classList.remove("seleccionado"));
        asiento.classList.add("seleccionado");
    }

    // Evento de búsqueda de boletos
    buscarBtn.addEventListener("click", function () {
        const origen = document.getElementById("origen").value;
        const destino = document.getElementById("destino").value;
        const fechaIda = fechaIdaInput.value;
        const tipoBoleto = document.querySelector('input[name="tipo-boleto"]:checked').value;
        const fechaRegreso = tipoBoleto === "redondo" ? fechaRegresoInput.value : "N/A";

        if (!origen || !destino || !fechaIda || (tipoBoleto === "redondo" && !fechaRegreso)) {
            alert("Por favor, completa todos los campos.");
            return;
        }

        boletosDisponibles.style.display = "block";
        mostrarBoletos();
    });

    // Evento de confirmación del boleto
    document.getElementById("formulario-pasajero").addEventListener("submit", function (e) {
        e.preventDefault();
        alert("Boleto confirmado.");
    });
});

