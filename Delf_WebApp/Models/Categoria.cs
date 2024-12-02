namespace Delf_WebApp.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }

        //Relacion uno a muchos con Articulos
        public ICollection<Articulo>? Articulos { get; set; }    

    }
}
