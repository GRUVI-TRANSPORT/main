
  document.getElementById("recovery-form").addEventListener("submit", function(event) {
    event.preventDefault(); // Evita que el formulario se envíe inmediatamente

    // Muestra la alerta de que se ha enviado el enlace
    document.getElementById("alert-message").style.display = "block";

    // Opcional: Esconde la alerta después de 5 segundos (5000 milisegundos)
    setTimeout(function() {
      document.getElementById("alert-message").style.display = "none";
    }, 5000);

    // Aquí puedes agregar el código para enviar el formulario, por ejemplo, mediante AJAX o cualquier otro método
    // Para propósitos de demostración, vamos a suponer que el formulario se envía correctamente
    console.log("Formulario enviado");
  });

