using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delf_WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Delf_WebApp.Controllers
{
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Pedidos
        [Authorize(Policy = "EsAdministrador")]
        public async Task<IActionResult> Index(string searchString, int pagina = 1, int tamanioPagina = 5)
        {
            // Obtener los clientes del pedido
            var pedidos = from p in _context.Pedidos
                    .Include(p => p.Cliente)
                          select p;

            // Filtrar por el texto de búsqueda si se proporciona
            if (!String.IsNullOrEmpty(searchString))
            {
                pedidos = pedidos.Where(p => p.Numero!.Contains(searchString));
            }

            //Ordenar los pedidos por numero descendente
            pedidos = pedidos.OrderByDescending(p => p.Numero);

            // Obtener el total de pedidos (para calcular las páginas)
            var totalPedidos = await pedidos.CountAsync();

            // Aplicar paginación (omitir los registros anteriores y tomar solo los de la página actual)
            var pedidosPaginados = await pedidos
                                         .Skip((pagina - 1) * tamanioPagina)
                                         .Take(tamanioPagina)
                                         .ToListAsync();

            // Crear el paginador con la lista paginada
            var paginador = new Paginador<Pedido>(pedidosPaginados, totalPedidos, pagina, tamanioPagina);

            //Para mantener el valor del campo de búsqueda cuando el usuario cambie de página
            ViewData["searchString"] = searchString;

            return View(paginador);
        }


        // GET: Pedidos/Details/5
        [Authorize(Policy = "EsAdministrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente) // Incluir el cliente
                .Include(p => p.ArticulosCantidades!) // Incluir los artículos asociados
                .ThenInclude(ac => ac.Articulo)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidos/Create

        [Authorize(Policy = "EsUsuario")]
        public IActionResult Create()
        {
            // Generar el número de pedido aquí
            var ultimoPedido = _context.Pedidos.OrderByDescending(p => p.Id).FirstOrDefault();
            string numeroPedido = ultimoPedido != null ? (int.Parse(ultimoPedido.Numero!) + 1).ToString("D6") : "000001";

            // Crear un nuevo pedido con el número generado
            var pedido = new Pedido
            {
                Numero = numeroPedido
            };

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "NombreCompleto");
            ViewData["ArticuloId"] = new SelectList(_context.Articulos, "Id", "Descripcion");
            return View();
        }



        //Metodo http para con Json y Ajax poder acceder al listado de articulos en tiempo real. 
        [HttpGet]
        [Authorize(Policy = "EsUsuario")]
        public IActionResult GetArticulos()
        {
            var articulos = _context.Articulos.Select(a => new { a.Id, a.Descripcion }).ToList();
            return Json(articulos);
        }



        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EsUsuario")]

        public async Task<IActionResult> Create([Bind("Id,Numero,Fecha,ClienteId")] Pedido pedido, int[] articuloIds, int[] cantidades)
        {
            if (ModelState.IsValid)
            {
                //Numero de pedido auto numerico
                //-----------------------------------------------------------------------------------------------
                // Obtener el último pedido por número
                var ultimoPedido = await _context.Pedidos
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();

                // Generar el nuevo número incremental
                if (ultimoPedido != null)
                {
                    // Incrementar el número basado en el último pedido
                    int nuevoNumero = int.Parse(ultimoPedido.Numero!) + 1;
                    pedido.Numero = nuevoNumero.ToString("D6");  // Formato con ceros a la izquierda
                }
                else
                {
                    // Establecer el número inicial
                    pedido.Numero = "000001";
                }
                //------------------------------------------------------------------------------------------------

                _context.Add(pedido);
                await _context.SaveChangesAsync();

                for (int i = 0; i < articuloIds.Length; i++)
                {
                    var articuloCantidad = new ArticuloCantidad
                    {
                        PedidoId = pedido.Id,
                        ArticuloId = articuloIds[i],
                        Cantidad = cantidades[i]
                    };
                    _context.Add(articuloCantidad);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //Mostrar nombre de cliente
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "NombreCompleto", pedido.ClienteId);
            ViewData["ArticuloId"] = new SelectList(_context.Articulos, "Id", "Descripcion");
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                    .Include(p => p.Cliente!)
                    .Include(p => p.ArticulosCantidades!)
                    .ThenInclude(ac => ac.Articulo)
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "NombreCompleto", pedido.ClienteId);
            ViewData["ArticuloId"] = new SelectList(_context.Articulos, "Id", "Descripcion");
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Fecha,ClienteId")] Pedido pedido, int[] articuloIds, int[] cantidades)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                    // Actualizar artículos
                    var existingArticuloCantidades = _context.ArticuloCantidades.Where(ac => ac.PedidoId == id);
                    _context.ArticuloCantidades.RemoveRange(existingArticuloCantidades);
                    for (int i = 0; i < articuloIds.Length; i++)
                    {
                        var articuloCantidad = new ArticuloCantidad
                        {
                            PedidoId = pedido.Id,
                            ArticuloId = articuloIds[i],
                            Cantidad = cantidades[i]
                        };
                        _context.Add(articuloCantidad);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "NombreCompleto", pedido.ClienteId);
            ViewData["ArticuloId"] = new SelectList(_context.Articulos, "Id", "Descripcion");
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
