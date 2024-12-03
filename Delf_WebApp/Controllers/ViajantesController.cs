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
    [Authorize(Policy = "EsAdministrador")] // Solo los administradores podrán acceder a este controlador
    public class ViajantesController : Controller
    {
        private readonly AppDbContext _context;

        public ViajantesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Viajantes
        public async Task<IActionResult> Index(string searchString, int pagina = 1, int tamanioPagina = 5)
        {
            //Incluir lo clientes y sus pedidos en cada viajante
            var viajantes = from v in _context.Viajantes
                .Include(v => v.Clientes!)
                .ThenInclude(c => c.Pedidos!)
                            select v;

            // Filtrar por el texto de búsqueda si se proporciona
            if (!String.IsNullOrEmpty(searchString))
            {
                viajantes = viajantes.Where(v => v.Nombre!.Contains(searchString) || v.Apellido!.Contains(searchString));
            }

            // Obtener el total de Viajantes (para calcular las páginas)
            var totalViajantes = await viajantes.CountAsync();

            // Aplicar paginación (omitir los registros anteriores y tomar solo los de la página actual)
            var viajantesPaginados = await viajantes
                                         .Skip((pagina - 1) * tamanioPagina)
                                         .Take(tamanioPagina)
                                         .ToListAsync();

            // Crear el paginador con la lista paginada
            var paginador = new Paginador<Viajante>(viajantesPaginados, totalViajantes, pagina, tamanioPagina);

            //Para mantener el valor del campo de búsqueda cuando el usuario cambie de página
            ViewData["searchString"] = searchString;

            return View(paginador);
        }

        // GET: Viajantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viajante = await _context.Viajantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viajante == null)
            {
                return NotFound();
            }

            return View(viajante);
        }

        // GET: Viajantes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Viajantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Cuit,Direccion,Altura,Departamento,Piso,Localidad,Provincia,Pais,Telefono,Email,Estado,PedidoId")] Viajante viajante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viajante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viajante);
        }

        // GET: Viajantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viajante = await _context.Viajantes.FindAsync(id);
            if (viajante == null)
            {
                return NotFound();
            }
            return View(viajante);
        }

        // POST: Viajantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Cuit,Direccion,Altura,Departamento,Piso,Localidad,Provincia,Pais,Telefono,Email,Estado,PedidoId")] Viajante viajante)
        {
            if (id != viajante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viajante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViajanteExists(viajante.Id))
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
            return View(viajante);
        }

        // GET: Viajantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viajante = await _context.Viajantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viajante == null)
            {
                return NotFound();
            }

            return View(viajante);
        }

        // POST: Viajantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viajante = await _context.Viajantes.FindAsync(id);
            if (viajante != null)
            {
                _context.Viajantes.Remove(viajante);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViajanteExists(int id)
        {
            return _context.Viajantes.Any(e => e.Id == id);
        }
    }
}
