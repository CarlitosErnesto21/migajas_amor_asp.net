using System;
using System.Collections.Generic;

namespace migajas_amor.app.Models;

public partial class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public decimal Total { get; set; }
}
