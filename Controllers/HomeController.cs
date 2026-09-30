using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TP07.Models;

namespace TP07.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string nombreUsuario, string contraseña)
    {
        Usuario usuario = BD.Login(nombreUsuario, contraseña);

        if (usuario != null)
        {
            HttpContext.Session.SetString("nombreUsuario", usuario.nombreUsuario);
            return RedirectToAction("RedSocial");
        }
        else
        {
            ViewBag.Error = "El usuario o la contraseña son incorrectos.";
            return View("Index");
        }
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(Usuario usuario)
    {
        if (usuario == null)
        {
            ViewBag.Error = "Datos inválidos.";
            return View();
        }

        if (BD.ExisteUsuario(usuario.nombreUsuario))
        {
            ViewBag.Error = "El nombre de usuario ya está en uso.";
            return View(usuario);
        }

        BD.RegistrarUsuario(usuario);

        return RedirectToAction("Login");
    }

    public IActionResult RedSocial()
    {
        string nombreUsuario = HttpContext.Session.GetString("nombreUsuario");

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            return RedirectToAction("Index");
        }

        Usuario usuarioActual = BD.ObtenerUsuario(nombreUsuario);
        ViewBag.usuarioActual = usuarioActual;
        ViewBag.nombreUsuario = nombreUsuario;

        List<Publicacion> publicaciones = BD.ObtenerPublicacionesRecientes(10);
        return View(publicaciones);
    }

    public IActionResult CrearPublicacion()
    {
        string nombreUsuario = HttpContext.Session.GetString("nombreUsuario");

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            return RedirectToAction("Index");
        }

        ViewBag.nombreUsuario = nombreUsuario;
        return View();
    }

    [HttpPost]
    public IActionResult CrearPublicacion(Publicacion publicacion, IFormFile imagenArchivo)
    {
        string nombreUsuario = HttpContext.Session.GetString("nombreUsuario");

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            return RedirectToAction("Index");
        }

        Usuario usuario = BD.ObtenerUsuario(nombreUsuario);

        if (usuario == null)
        {
            return RedirectToAction("Index");
        }

        if (imagenArchivo == null || imagenArchivo.Length == 0)
        {
            ViewBag.Error = "Debes seleccionar una imagen para la publicación.";
            return View(publicacion);
        }

        if (string.IsNullOrWhiteSpace(publicacion.titulo) ||
            string.IsNullOrWhiteSpace(publicacion.descripcion))
        {
            ViewBag.Error = "Todos los campos son obligatorios.";
            return View(publicacion);
        }

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
        Directory.CreateDirectory(uploadsFolder);

        string nombreArchivo = Path.GetFileName(imagenArchivo.FileName);
        string rutaArchivo = Path.Combine(uploadsFolder, nombreArchivo);

        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
        {
            imagenArchivo.CopyTo(stream);
        }

        publicacion.imagen = "/imagenes/" + nombreArchivo;
        publicacion.IdUsuario = usuario.Id;
        publicacion.fechaPublicacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        BD.CrearPublicacion(publicacion);

        ViewBag.Mensaje = "Publicación creada correctamente.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AgregarComentario(int idPublicacion, string texto)
    {
        string nombreUsuario = HttpContext.Session.GetString("nombreUsuario");

        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrWhiteSpace(texto))
        {
            return Json(new { success = false, message = "Datos inválidos" });
        }

        Usuario usuario = BD.ObtenerUsuario(nombreUsuario);
        if (usuario == null)
        {
            return Json(new { success = false, message = "Usuario no autenticado" });
        }

        BD.AgregarComentario(idPublicacion, usuario.Id, texto);

        var usuarioComentario = BD.ObtenerUsuarioPorId(usuario.Id);
        var fechaActual = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

        return Json(new
        {
            success = true,
            comentario = new
            {
                nombreUsuario = usuarioComentario.nombreUsuario,
                nombre = usuarioComentario.nombre,
                apellido = usuarioComentario.apellido,
                texto = texto,
                fechaComentario = fechaActual
            }
        });
    }

    [HttpPost]
    public IActionResult AgregarLike(int idPublicacion)
    {
        string nombreUsuario = HttpContext.Session.GetString("nombreUsuario");

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            return Json(new { success = false, message = "Usuario no autenticado" });
        }

        Usuario usuario = BD.ObtenerUsuario(nombreUsuario);
        if (usuario == null)
        {
            return Json(new { success = false, message = "Usuario no encontrado" });
        }

        bool yaLike = BD.VerificarLike(idPublicacion, usuario.Id);

        if (yaLike)
        {
            BD.RemoverLike(idPublicacion, usuario.Id);
        }
        else
        {
            BD.AgregarLike(idPublicacion, usuario.Id);
        }

        int cantidadLikes = BD.ObtenerCantidadLikes(idPublicacion);
        bool nuevoEstado = !yaLike;

        return Json(new
        {
            success = true,
            yaLike = nuevoEstado,
            cantidadLikes = cantidadLikes
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
