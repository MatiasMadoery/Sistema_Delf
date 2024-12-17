
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using Delf_WebApp.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Delf_WebApp.Controllers
//{
//    public class ExportController : Controller
//    {
//        private readonly AppDbContext _context;

//        public ExportController(AppDbContext context)
//        {
//            _context = context;
//        }

//        public FileResult ExportPedido(int id)
//        {
//            var pedido = _context.Pedidos
//                .Include(p => p.Cliente)
//                .Include(p => p.ArticulosCantidades)
//                .ThenInclude(ac => ac.Articulo)
//                .FirstOrDefault(p => p.Id == id);

//            if (pedido == null)
//            {
//                return null;
//            }

//            using (var memoryStream = new System.IO.MemoryStream())
//            {
//                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
//                var writer = PdfWriter.GetInstance(document, memoryStream);
//                writer.CloseStream = false;
//                document.Open(); 

//                // Fuentes
//                var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
//                var subTituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
//                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

//                // Título
//                var titulo = new Paragraph("Detalle del Pedido", tituloFont)
//                {
//                    Alignment = Element.ALIGN_CENTER,
//                    SpacingAfter = 20f
//                };
//                document.Add(titulo);

//                // Información del pedido
//                var infoPedido = new Paragraph($"Pedido Número: {pedido.Numero}\nFecha: {pedido.Fecha:dd/MM/yyyy}\nCliente: {pedido.Cliente?.Nombre} {pedido.Cliente?.Apellido}", normalFont)
//                {
//                    SpacingAfter = 20f
//                };
//                document.Add(infoPedido);

//                // Título de los artículos
//                var tituloArticulos = new Paragraph("Artículos:", subTituloFont)
//                {
//                    SpacingAfter = 10f
//                };
//                document.Add(tituloArticulos);

//                // Tabla de artículos
//                PdfPTable table = new PdfPTable(3);
//                table.WidthPercentage = 100;
//                table.SetWidths(new float[] { 1f, 3f, 1f });

//                // Encabezados de la tabla
//                table.AddCell(new PdfPCell(new Phrase("Código", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
//                table.AddCell(new PdfPCell(new Phrase("Descripción", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
//                table.AddCell(new PdfPCell(new Phrase("Cantidad", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

//                // Datos de los artículos
//                foreach (var articuloCantidad in pedido.ArticulosCantidades!)
//                {
//                    if (articuloCantidad.Articulo != null)
//                    {
//                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Articulo.Codigo, normalFont)));
//                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Articulo.Descripcion, normalFont)));
//                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Cantidad.ToString(), normalFont)));
//                    }
//                }

//                document.Add(table);
//                document.Close();
//                writer.Close();

//                memoryStream.Position = 0;
//                return File(memoryStream.ToArray(), "application/pdf", "Pedido.pdf");
//            }
//        }
//    }
//}





using iTextSharp.text;
using iTextSharp.text.pdf;
using Delf_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Delf_WebApp.Controllers
{
    public class ExportController : Controller
    {
        private readonly AppDbContext _context;

        public ExportController(AppDbContext context)
        {
            _context = context;
        }

        public FileResult ExportPedido(int id)
        {
            var pedido = _context.Pedidos!
                .Include(p => p.Cliente!)
                .Include(p => p.ArticulosCantidades!)
                .ThenInclude(ac => ac.Articulo)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return null!;
            }

            using (var memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, memoryStream);
                writer.CloseStream = false;
                document.Open();

                // Fuentes
                var tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var subTituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Información de la empresa
                var empresaInfo = new Paragraph("Razón Social: Filtros DELF\nDomicilio Comercial: Almafuerte 486, San Francisco, Córdoba, Argentina\nTelefono: 3564 370727\nCUIT: 30-12345678-9\nCondición frente al IVA: Responsable Inscripto", normalFont)
                {
                    SpacingAfter = 20f
                };
                document.Add(empresaInfo);

                // Título
                var titulo = new Paragraph("Detalle del Pedido", tituloFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                 document.Add(titulo);

                // Información del pedido
                var infoPedido = new Paragraph($"Pedido Número: {pedido.Numero}\nFecha: {pedido.Fecha:dd/MM/yyyy}\nCliente: {pedido.Cliente?.Nombre} {pedido.Cliente?.Apellido}", normalFont)
                {
                    SpacingAfter = 20f
                };
                document.Add(infoPedido);

                // Tabla de artículos
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 1f, 1f });

                // Encabezados de la tabla
                table.AddCell(new PdfPCell(new Phrase("Código", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Descripción", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Cantidad", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", subTituloFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                // Datos de los artículos
                foreach (var articuloCantidad in pedido.ArticulosCantidades!)
                {
                    if (articuloCantidad.Articulo != null)
                    {
                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Articulo.Codigo, normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Articulo.Descripcion, normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Cantidad.ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(articuloCantidad.Articulo.Precio.ToString("C"), normalFont))); // Formato de precio
                    }
                }

                document.Add(table);

                // Total
                var total = pedido.ArticulosCantidades!.Sum(ac => ac.Cantidad * ac.Articulo!.Precio);
                var totalParagraph = new Paragraph($"Total: {total:C}", subTituloFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 10f
                };
                document.Add(totalParagraph);

                document.Close();
                writer.Close();

                memoryStream.Position = 0;
                return File(memoryStream.ToArray(), "application/pdf", "Pedido.pdf");
            }
        }
    }
}








