using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using migajas_amor.app.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<IActionResult> ListUsuarioRol()
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

            return View(usuarios);
        }

        public async Task<IActionResult> ListPedidos()
        {
            return View(await _context.Pedidos.ToListAsync());
        }
    }
}