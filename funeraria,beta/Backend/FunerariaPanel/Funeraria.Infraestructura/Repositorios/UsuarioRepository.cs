using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Funeraria.Dominio.Entidades;
using Funeraria.Dominio.Interfaces;
using Funeraria.Infraestructura.Persistencia;

namespace Funeraria.Infraestructura.Repositorios
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// Maneja el acceso a datos para usuarios (CU-001)
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FunerariaDbContext _context;

        public UsuarioRepository(FunerariaDbContext context)
        {
            _context = context;
        }

        // CU-001: Obtener usuario por nombre de usuario (para login)
        public async Task<Usuario> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
            }
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
            }
        }

        public async Task<int> RegistrarAsync(Usuario usuario)
        {
            try
            {
                usuario.FechaCreacion = DateTime.Now;
                usuario.Activo = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return usuario.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}", ex);
            }
        }

        // Actualizar último acceso cuando el usuario inicia sesión
        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            try
            {
                var usuario = await ObtenerPorIdAsync(usuarioId);

                if (usuario != null)
                {
                    usuario.UltimoAcceso = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar último acceso: {ex.Message}", ex);
            }
        }

        public async Task<List<Usuario>> ListarAsync()
        {
            try
            {
                return await _context.Usuarios
                    .Where(u => u.Activo)
                    .OrderBy(u => u.NombreCompleto)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar usuarios: {ex.Message}", ex);
            }
        }
    }
}
