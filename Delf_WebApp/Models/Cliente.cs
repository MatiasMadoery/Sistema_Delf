namespace Delf_WebApp.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Cuit { get; set; }
        public string? Direccion { get; set; }
        public string? Altura { get; set; }        
        public string? Departamento { get; set; }
        public string? Piso { get; set; }
        public string? Localidad { get; set; }
        public string? Provincia { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Estado {  get; set; }

        //Clave foranea con viajante
        public int ViajanteId  { get; set; }
        public Viajante? Viajante { get; set; }

        //Relacion uno a muchos con pedidos
        public ICollection<Pedido> ?Pedidos { get; set; }

        //Mostrar nombre completo
        public string NombreCompleto => $"{Nombre} {Apellido}";

    }
}
