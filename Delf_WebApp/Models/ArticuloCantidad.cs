namespace Delf_WebApp.Models
{
    public class ArticuloCantidad
    {
        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public Articulo? Articulo { get; set; }
        public int Cantidad { get; set; }
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
    }
}
