using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delf_WebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Delf_WebApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar la vista de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Acción para manejar el POST del login
        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            var usuario = _context.Usuarios!.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena);
            if (usuario != null)
            {
                // Crear los claims (información del usuario)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario!),
                    new Claim(ClaimTypes.Role, usuario.Rol!) // Asignar el rol
                };

                // Crear la identidad del usuario
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Autenticar al usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirigir dependiendo del rol
                if (usuario.Rol == "Administrador")
                {
                    return RedirectToAction("Index", "Home"); // Redirige a una acción de administrador
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirige a una acción de usuario
                }
            }
            else
            {
                // Si el login falla
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                ViewData["Error"] = "Usuario o Contraseña incorrectos!";
            }           
            return View();
        }

        // Acción para cerrar sesión
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Vista de acceso denegado
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
