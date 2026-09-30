namespace TP07.Models;
using Dapper;
using Microsoft.Data.SqlClient;

public class BD
{
    private static string connectionString = @"Server=localhost;Database=DBRedSocial;Integrated Security=True;TrustServerCertificate=True";

    public static void RegistrarUsuario(Usuario u)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido) VALUES (@nombreUsuario, @contraseña, @nombre, @apellido)";
            connection.Execute(query, (new { u.nombreUsuario, u.contraseña, u.nombre, u.apellido }));
        }
    }

    public static bool ExisteUsuario(string nombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombreUsuario";
            Usuario usuario = connection.QueryFirstOrDefault<Usuario>(query, new { nombreUsuario });
            return usuario != null;
        }
    }

    public static Usuario Login(string nombreUsuario, string contraseña)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombreUsuario AND Contraseña = @contraseña";
            return connection.QueryFirstOrDefault<Usuario>(query, new { nombreUsuario, contraseña });
        }
    }

    public static Usuario ObtenerUsuario(string nombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombreUsuario";
            return connection.QueryFirstOrDefault<Usuario>(query, new { nombreUsuario });
        }
    }

    public static void CrearPublicacion(Publicacion p)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO Publicaciones (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
                             VALUES (@IdUsuario, @titulo, @descripcion, @imagen, @fechaPublicacion)";

            connection.Execute(query, new { p.IdUsuario, p.titulo, p.descripcion, p.imagen, p.fechaPublicacion });
        }
    }

    public static List<Publicacion> ObtenerPublicacionesRecientes(int top = 10)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = $@"SELECT TOP {top} Id, IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion 
                              FROM Publicaciones 
                              ORDER BY FechaPublicacion DESC";
            return connection.Query<Publicacion>(query).ToList();
        }
    }

    public static Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Usuarios WHERE Id = @idUsuario";
            return connection.QueryFirstOrDefault<Usuario>(query, new { idUsuario });
        }
    }

    public static List<Comentario> ObtenerComentarios(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"SELECT Id, IdPublicacion, IdUsuarioComenta, Texto, FechaComentario 
                             FROM Comentarios 
                             WHERE IdPublicacion = @idPublicacion 
                             ORDER BY FechaComentario DESC";
            return connection.Query<Comentario>(query, new { idPublicacion }).ToList();
        }
    }

    public static void AgregarComentario(int idPublicacion, int idUsuario, string texto)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario) 
                             VALUES (@idPublicacion, @idUsuario, @texto, @fechaComentario)";
            connection.Execute(query, new { idPublicacion, idUsuario, texto, fechaComentario = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }
    }

    public static int ObtenerCantidadLikes(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE IdPublicación = @idPublicacion";
            return connection.ExecuteScalar<int>(query, new { idPublicacion });
        }
    }

    public static bool VerificarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE IdPublicación = @idPublicacion AND IdUsuario = @idUsuario";
            int count = connection.ExecuteScalar<int>(query, new { idPublicacion, idUsuario });
            return count > 0;
        }
    }

    public static void AgregarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            if (!VerificarLike(idPublicacion, idUsuario))
            {
                string query = "INSERT INTO PublicacionesMeGusta (IdPublicación, IdUsuario) VALUES (@idPublicacion, @idUsuario)";
                connection.Execute(query, new { idPublicacion, idUsuario });
            }
        }
    }

    public static void RemoverLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM PublicacionesMeGusta WHERE IdPublicación = @idPublicacion AND IdUsuario = @idUsuario";
            connection.Execute(query, new { idPublicacion, idUsuario });
        }
    }
}