using System;
using System.Collections.Generic;

namespace migajas_amor.app.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;
}
