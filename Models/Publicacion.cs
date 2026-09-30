namespace TP07.Models;

public class Publicacion
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string titulo { get; set; }
    public string descripcion { get; set; }
    public string imagen { get; set; }
    public string fechaPublicacion { get; set; }
}
