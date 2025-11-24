using APIRegistro.Data;
using APIRegistro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIRegistro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Usuarios/registro
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Usuario>> RegistrarUsuario([FromBody] Usuario usuario)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                return BadRequest("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(usuario.CorreoElectronico))
            {
                return BadRequest("El correo electrónico es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                return BadRequest("La contraseña es obligatoria");
            }

            // Verificar si el usuario ya existe
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);

            if (usuarioExiste)
            {
                return BadRequest("El correo electrónico ya está registrado");
            }

            // Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuario.Id }, usuario);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> ObtenerUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerTodosLosUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }
    }
}