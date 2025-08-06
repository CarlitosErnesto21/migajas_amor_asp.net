using System;
using System.Collections.Generic;

namespace migajas_amor.app.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public int? ClienteId { get; set; }

    public int? Cantidad { get; set; }

    public DateTime? FechaPedido { get; set; }

    public string? Estado { get; set; }

    public string? DireccionEntrega { get; set; }

    public string? Telefono { get; set; }

    public string? Comentarios { get; set; }
}
