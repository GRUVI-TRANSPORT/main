// Verificar si el usuario ha iniciado sesión
function verificarSesion() {
    const usuario = JSON.parse(localStorage.getItem("usuario"));

    // Obtener los enlaces de "Iniciar Sesión" y "Crear Cuenta"
    const enlaceIniciarSesion = document.querySelector('a[href="signin.html"]');
    const enlaceCrearCuenta = document.querySelector('a[href="signup.html"]');

    if (usuario) {
        // Ocultar los enlaces de "Iniciar Sesión" y "Crear Cuenta"
        if (enlaceIniciarSesion) enlaceIniciarSesion.style.display = "none";
        if (enlaceCrearCuenta) enlaceCrearCuenta.style.display = "none";

        // Mostrar la sección de perfil
        document.getElementById("autenticado").style.display = "flex";

        // Mostrar la información del usuario
        document.getElementById("profile-name").textContent = usuario.nombre;
        document.getElementById("profile-email").textContent = usuario.email;
    } else {
        // Mostrar los enlaces de "Iniciar Sesión" y "Crear Cuenta"
        if (enlaceIniciarSesion) enlaceIniciarSesion.style.display = "inline-block";
        if (enlaceCrearCuenta) enlaceCrearCuenta.style.display = "inline-block";

        // Ocultar la sección de perfil
        document.getElementById("autenticado").style.display = "none";
    }
}

// Cerrar sesión
async function cerrarSesion() {
    try {
        // Llamar al backend para cerrar la sesión
        const response = await fetch("/api/Login/logout", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
        });

        if (response.ok) {
            // Eliminar la información del usuario del localStorage
            localStorage.removeItem("usuario");

            // Redirigir al usuario a la página de inicio de sesión
            window.location.href = "signin.html";
        } else {
            alert("Hubo un error al cerrar la sesión. Inténtalo de nuevo.");
        }
    } catch (error) {
        console.error("Error:", error);
        alert("Hubo un error de conexión. Inténtalo de nuevo.");
    }
}

// Verificar el estado de la sesión al cargar la página
window.onload = verificarSesion;