namespace migajas_amor.app.Pdf
{
    public class DetallePedidoPdf
    {
        public DateTime FechaPedido { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Estado { get; set; }
        public string? Producto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
