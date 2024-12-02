using System.ComponentModel.DataAnnotations;

namespace Delf_WebApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string? Numero { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }


        //Clave foranea a Cliente
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }      

        //Relacion uno a muchos con articulos        
        public ICollection<ArticuloCantidad>? ArticulosCantidades { get; set; }    
        

    }
}
