namespace Delf_WebApp.Models
{
    public class Viajante
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
        public bool Estado { get; set; }


        //Relacion de uno a muchos con viajantes
        public ICollection<Cliente>? Clientes { get; set; }               
    }
}
