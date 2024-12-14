using Delf_WebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Inyeccion de depnedencia
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("conexionDb"))
    );

// Agregar servicios de autenticación y autorización
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //Para mitigar el riesgo de ataques de secuestro de sesiones y XSS (Cross-Site Scripting).
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.LoginPath = "/Usuarios/Login"; // La ruta de login
        options.AccessDeniedPath = "/Usuarios/AccesoDenegado"; // Ruta si el acceso es denegado
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EsUsuario", policy => policy.RequireRole("Usuario", "Administrador"));
    options.AddPolicy("EsAdministrador", policy => policy.RequireRole("Administrador"));
    //options.AddPolicy("EsUsuario", policy => policy.RequireRole("Usuario"));
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar autenticación y autorización al pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
