using System;
using System.Collections.Generic;

namespace migajas_amor.app.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }
}
