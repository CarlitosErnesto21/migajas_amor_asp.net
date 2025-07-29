using System;
using System.Collections.Generic;

namespace migajas_amor.app.Models;

public partial class Rolesasignado
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int RolId { get; set; }
}
