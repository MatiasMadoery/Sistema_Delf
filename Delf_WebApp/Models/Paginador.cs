namespace Delf_WebApp.Models
{
    public class Paginador<T>
    {
        public List<T> Elementos { get; private set; }
        public int TotalElementos { get; private set; }
        public int ElementosPorPagina { get; private set; }
        public int PaginaActual { get; private set; }
        public int TotalPaginas { get; private set; }

        public Paginador(List<T> elementos, int totalElementos, int paginaActual, int elementosPorPagina)
        {          
            Elementos = elementos;
            TotalElementos = totalElementos;
            PaginaActual = paginaActual;
            ElementosPorPagina = elementosPorPagina;
            TotalPaginas = (int)Math.Ceiling(totalElementos / (double)elementosPorPagina);
        }

        public bool HayPaginaAnterior => PaginaActual > 1;
        public bool HayPaginaSiguiente => PaginaActual < TotalPaginas;
    }

}

