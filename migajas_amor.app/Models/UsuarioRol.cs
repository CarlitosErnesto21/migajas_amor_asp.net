namespace migajas_amor.app.Models
{
    public class UsuarioRol
    {
        public int UsuarioId { get; set; }
        public string Login { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
