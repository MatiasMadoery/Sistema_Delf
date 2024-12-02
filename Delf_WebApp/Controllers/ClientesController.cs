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
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Clientes
        public async Task<IActionResult> Index(string searchString, int pagina = 1, int tamanioPagina = 5)
        {
            // Obtener los clientes con su respectivo viajante
            var clientes = from c in _context.Clientes
                           .Include(c => c.Viajante)
                           select c;

            // Filtrar por el texto de búsqueda si se proporciona
            if (!String.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(s => s.Apellido!.Contains(searchString) || s.Nombre!.Contains(searchString));
            }

            // Obtener el total de clientes (para calcular las páginas)
            var totalClientes = await clientes.CountAsync();

            // Aplicar paginación (omitir los registros anteriores y tomar solo los de la página actual)
            var clientesPaginados = await clientes
                                         .Skip((pagina - 1) * tamanioPagina)
                                         .Take(tamanioPagina)
                                         .ToListAsync();

            // Crear el paginador con la lista paginada
            var paginador = new Paginador<Cliente>(clientesPaginados, totalClientes, pagina, tamanioPagina);

            //Para mantener el valor del campo de búsqueda cuando el usuario cambie de página
            ViewData["searchString"] = searchString;
            return View(paginador);
        }


        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Viajante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Cuit,Direccion,Altura,Departamento,Piso,Localidad,Provincia,Pais,Telefono,Email,Estado,ViajanteId")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {

                // Verificar que el viajante existe
                var viajante = await _context.Viajantes.FindAsync(cliente.ViajanteId);

                if (viajante == null)
                {
                    ModelState.AddModelError("ViajanteId", "El viajante seleccionado no existe.");
                    ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre", cliente.ViajanteId);
                    return View(cliente);
                }

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre");
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre");
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Cuit,Direccion,Altura,Departamento,Piso,Localidad,Provincia,Pais,Telefono,Email,Estado,ViajanteId")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var viajante = await _context.Viajantes.FindAsync(cliente.ViajanteId);
                    if (viajante == null)
                    {
                        ModelState.AddModelError("ViajanteId", "El viajante seleccionado no existe.");
                        ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre", cliente.ViajanteId);
                    }
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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

            ViewData["ViajanteId"] = new SelectList(_context.Viajantes, "Id", "Nombre", cliente.ViajanteId);

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
