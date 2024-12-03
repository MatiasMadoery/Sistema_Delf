using ClosedXML.Excel;
using Delf_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delf_WebApp.Controllers
{
    [Authorize(Policy = "EsAdministrador")] // Solo los administradores podrán acceder a este controlador
    public class ImportarExController : Controller
    {
        private readonly AppDbContext _context;


        public ImportarExController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ImportarExController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ImportarExController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ImportarExController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImportarExController/Create  

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Importar(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var clientes = LeerArchivoExcel(file);

                foreach (var cliente in clientes)
                {
                    _context.Clientes?.Add(cliente);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Clientes");
        }

        private List<Cliente> LeerArchivoExcel(IFormFile file)
        {
            var clientes = new List<Cliente>();

            using (var stream = file.OpenReadStream())
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1)) // saltar la fila de encabezado
                {
                    var cliente = new Cliente
                    {
                        Nombre = row.Cell(1).GetValue<string>(),
                        Apellido = row.Cell(2).GetValue<string>(),
                        Cuit = row.Cell(3).GetValue<string>(),
                        Direccion = row.Cell(4).GetValue<string>(),
                        Altura = row.Cell(5).GetValue<string>(),
                        Departamento = row.Cell(6).GetValue<string>(),
                        Piso = row.Cell(7).GetValue<string>(),
                        Localidad = row.Cell(8).GetValue<string>(),
                        Provincia = row.Cell(9).GetValue<string>(),
                        Pais = row.Cell(10).GetValue<string>(),
                        Telefono = row.Cell(11).GetValue<string>(),
                        Email = row.Cell(12).GetValue<string>(),
                        Estado = row.Cell(13).GetValue<bool>(),
                        ViajanteId = row.Cell(14).GetValue<int>()
                    };

                    clientes.Add(cliente);
                }
            }

            return clientes;
        }

        // GET: ImportarExController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ImportarExController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImportarExController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImportarExController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
