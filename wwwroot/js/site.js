// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Validar formulario de Registro
function validarFormulario() {
    var nombreUsuario = document.getElementById("nombreUsuario").value.trim();
    var contraseña = document.getElementById("contraseña").value;
    var nombre = document.getElementById("nombre").value.trim();
    var apellido = document.getElementById("apellido").value.trim();
    var tipoUsuario = document.querySelector('input[name="tipoUsuario"]:checked');

    // Campos obligatorios
    if (!nombreUsuario || !contraseña || !nombre || !apellido || !tipoUsuario) {
        alert("Todos los campos son obligatorios.");
        return false;
    }

    // Nombre de usuario: mínimo 3 caracteres
    if (nombreUsuario.length < 3) {
        alert("El nombre de usuario debe tener mínimo 3 caracteres.");
        return false;
    }

    // Nombre: solo letras y espacios
    if (!/^[a-záéíóúñA-ZÁÉÍÓÚÑ\s]+$/.test(nombre)) {
        alert("El nombre solo puede contener letras.");
        return false;
    }
    // Nombre: mínimo 3 caracteres
    if (nombre.length < 3) {
        alert("El nombre debe tener mínimo 3 caracteres.");
        return false;
    }

    // Apellido: solo letras y espacios
    if (!/^[a-záéíóúñA-ZÁÉÍÓÚÑ\s]+$/.test(apellido)) {
        alert("El apellido solo puede contener letras.");
        return false;
    }
    // Apellido: mínimo 3 caracteres
    if (apellido.length < 3) {
        alert("El apellido debe tener mínimo 3 caracteres.");
        return false;
    }

    // Contraseña: mínimo 4 caracteres
    if (contraseña.length < 4) {
        alert("La contraseña debe tener mínimo 4 caracteres.");
        return false;
    }

    // Contraseña: debe tener al menos una mayúscula
    if (!/[A-Z]/.test(contraseña)) {
        alert("La contraseña debe contener al menos una mayúscula.");
        return false;
    }


    // Contraseña: debe tener al menos un número
    if (!/[0-9]/.test(contraseña)) {
        alert("La contraseña debe contener al menos un número.");
        return false;
    }

    return true;
}

// Red Social - Funciones para Likes y Comentarios
function toggleComments(publicacionId) {
    const commentsSection = document.getElementById('comments-' + publicacionId);
    if (commentsSection.style.display === 'none') {
        commentsSection.style.display = 'block';
    } else {
        commentsSection.style.display = 'none';
    }
}

function toggleLike(publicacionId) {
    const btn = document.getElementById('like-btn-' + publicacionId);
    const likesCountSpan = document.getElementById('likes-count-' + publicacionId);

    const formData = new FormData();
    formData.append('idPublicacion', publicacionId);

    fetch(urlAgregarLike, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        console.log('Response status:', response.status);
        return response.json();
    })
    .then(data => {
        console.log('Response data:', data);
        if (data.success) {
            // Actualizar el botón
            if (data.yaLike) {
                btn.classList.add('liked');
                btn.textContent = '❤️ Me Encanta';
            } else {
                btn.classList.remove('liked');
                btn.textContent = '🤍 Me Gusta';
            }

            // Actualizar la cantidad de likes
            const likeWord = data.cantidadLikes !== 1 ? 's' : '';
            likesCountSpan.textContent = `❤️ ${data.cantidadLikes} Me Gusta${likeWord}`;
        } else {
            console.error('Error:', data.message);
        }
    })
    .catch(error => console.error('Error:', error));
}

function agregarComentario(event, publicacionId) {
    event.preventDefault();

    const textInput = document.getElementById('comment-input-' + publicacionId);
    const texto = textInput.value.trim();

    if (!texto) {
        alert('El comentario no puede estar vacío');
        return;
    }

    const formData = new FormData();
    formData.append('idPublicacion', publicacionId);
    formData.append('texto', texto);

    fetch(urlAgregarComentario, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        console.log('Comentario response:', data);
        if (data.success) {
            const comentarioHTML = `
                <div class="comment">
                    <div class="comment-header">
                        <strong>${data.comentario.nombre} ${data.comentario.apellido}</strong>
                        <small>@${data.comentario.nombreUsuario}</small>
                    </div>
                    <p class="comment-text">${data.comentario.texto}</p>
                    <small class="comment-date">${formatearFecha(data.comentario.fechaComentario)}</small>
                </div>
            `;

            const commentsList = document.getElementById('comments-list-' + publicacionId);
            commentsList.insertAdjacentHTML('beforeend', comentarioHTML);
            textInput.value = '';
            actualizarContadorComentarios(publicacionId);
        } else {
            alert(data.message || 'Error al agregar el comentario');
        }
    })
    .catch(error => {
        console.error('Error en comentario:', error);
        alert('Error al agregar el comentario');
    });
}

function formatearFecha(fechaStr) {
    // Entrada: "09/20/2026 17:30:00"
    // Salida: "20/09/2026 17:30"
    const [fecha, hora] = fechaStr.split(' ');
    const [mes, dia, año] = fecha.split('/');
    const [horas, minutos] = hora.split(':');
    return `${dia}/${mes}/${año} ${horas}:${minutos}`;
}

function actualizarContadorComentarios(publicacionId) {
    const commentsList = document.getElementById('comments-list-' + publicacionId);
    const comentarios = commentsList.querySelectorAll('.comment').length;
    const commentsCountSpan = document.getElementById('comments-count-' + publicacionId);
    const comentarioWord = comentarios !== 1 ? 's' : '';
    commentsCountSpan.textContent = `💬 ${comentarios} Comentario${comentarioWord}`;
}

