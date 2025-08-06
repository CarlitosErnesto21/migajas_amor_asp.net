using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using migajas_amor.app.Models;
using System.Globalization; //importante para la cultura
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//Esto es para establecer la cultura por defecto de la aplicación
var cultureInfo = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Obtiene la cadena de conexión y lanza una excepción si es nula
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connetion string 'DbContext'" + " not found. ");

builder.Services.AddDbContext<MigajasAmorContext>(o =>
{
    o.UseMySQL(connectionString);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Index";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        option.AccessDeniedPath = "/Home/Privacy";
    });

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "listPedidosDirect",
    pattern: "ListPedidos",
    defaults: new { controller = "Acceso", action = "ListPedidos" });

app.MapControllerRoute(
    name: "listUsuarioRolDirect",
    pattern: "ListUsuarioRol",
    defaults: new { controller = "Acceso", action = "ListUsuarioRol" });

app.MapControllerRoute(
    name: "listClientesDirect",
    pattern: "ListaClientes",
    defaults: new { controller = "Acceso", action = "ListaClientes" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Index}/{id?}") //cambio temporal para que no se rompa la app
    .WithStaticAssets();

app.Run();
