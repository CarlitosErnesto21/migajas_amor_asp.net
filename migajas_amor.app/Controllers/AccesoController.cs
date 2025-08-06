using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using migajas_amor.app.Models;
using migajas_amor.app.Pdf;
using migajas_amor.app.Utilidades;
using MySqlX.XDevAPI;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using QuestPDF.Fluent;

namespace migajas_amor.app.Controllers
{
    public class AccesoController : Controller
    {
        private readonly MigajasAmorContext _context;

        public AccesoController(MigajasAmorContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(Usuario infoLogin)
        {
            if (infoLogin != null)
            {
                SHA256 mySHA256 = SHA256.Create();
                byte[] datos = Encoding.UTF8.GetBytes(infoLogin.Password);
                byte[] hashValue = mySHA256.ComputeHash(datos);

                string hash = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
                Usuario? usuario = _context.Usuarios.FromSqlRaw("SELECT Id, Login, Password FROM Usuarios WHERE Login = {0} AND Password = {1}", infoLogin.Login, hash).FirstOrDefault();

                if (usuario != null)
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, usuario.Login) // usuario.Login
                        
                    };

                    List<Role> lista = (from rls in _context.Roles
                                        join rlsa in _context.Rolesasignados
                                        on rls.Id equals rlsa.RolId
                                        where rlsa.UsuarioId == usuario.Id
                                        select rls).ToList();

                    foreach (Role rol in lista)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acceso");
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListUsuarioRol(int pg = 1, string? search = null)
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioRol
                {
                    UsuarioId = u.Id,
                    Login = u.Login,
                    Roles = (from rlsa in _context.Rolesasignados
                             join r in _context.Roles on rlsa.RolId equals r.Id
                             where rlsa.UsuarioId == u.Id
                             select r.Nombre).ToList()
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                usuarios = usuarios
                    .Where(u => u.Login.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || u.Roles.Any(r => r.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var totalRegistros = usuarios.Count();
            var paginacion = new Paginacion(totalRegistros, pg, 2, "ListUsuarioRol");
            var data = usuarios
                .Skip(paginacion.Salto)
                .Take(paginacion.RegistrosPagina)
                .ToList();
            this.ViewBag.Paginacion = paginacion;

            return View(data);
        }

        public async Task<IActionResult> ListPedidos(int pg = 1, string? search = null)
        {
            var pedidosQuery = await _context.Pedidos.ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                pedidosQuery = pedidosQuery.Where(p =>
                    (!string.IsNullOrEmpty(p.Estado) && p.Estado.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.DireccionEntrega) && p.DireccionEntrega.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalRegistros = pedidosQuery.Count();
            var paginacion = new Paginacion(totalRegistros, pg, 5, "ListPedidos");
            var data = pedidosQuery
                .Skip(paginacion.Salto)
                .Take(paginacion.RegistrosPagina)
                .ToList();
            this.ViewBag.Paginacion = paginacion;

            return View(data);
        }

        public async Task<IActionResult> ListaClientes(int pg = 1, string? search = null)
        {
            var clientes = await _context.Clientes.ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                clientes = clientes
                    .Where(c => (c.Nombre?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (c.Apellido?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                             || (c.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            var totalRegistros = clientes.Count();
            var paginacion = new Paginacion(totalRegistros, pg, 5, "ListaClientes");
            var data = clientes
                .Skip(paginacion.Salto)
                .Take(paginacion.RegistrosPagina)
                .ToList();
            this.ViewBag.Paginacion = paginacion;

            return View(data);
        }

        //Acción para generar el PDF de los detalles de pedidos
        [HttpGet(Name = "DetallePedidosPdf")]
        public IResult DetallePedidosPdf(int n)
        {
            string sql = "SELECT p.fecha_pedido, c.nombre, c.email, p.estado, pr.nombre AS producto, pr.precio AS precio_unitario, dp.cantidad, dp.total FROM pedidos p JOIN clientes c ON p.cliente_id = c.id JOIN detalle_pedidos dp ON p.id = dp.pedido_id JOIN productos pr ON dp.producto_id = pr.id ORDER BY p.fecha_pedido DESC;";
            List<DetallePedidoPdf> data = _context.PedidosPdf
                .FromSqlRaw(sql)
                .ToList();
            var document = new DetallePedidoPdfDoc(data);
            var pdfStream = document.GeneratePdf();
            return Results.File(pdfStream, "application/pdf", "detalle_pedidos.pdf");
        }
    }
}