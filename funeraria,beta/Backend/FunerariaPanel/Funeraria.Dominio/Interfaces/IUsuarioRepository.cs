using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funeraria.Dominio.Entidades;

namespace Funeraria.Dominio.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de usuarios
    /// </summary>
    public interface IUsuarioRepository
    {
        // Obtener usuario por nombre de usuario (para login)
        Task<Usuario> ObtenerPorNombreUsuarioAsync(string nombreUsuario);

        // Obtener usuario por ID
        Task<Usuario> ObtenerPorIdAsync(int id);

        // Registrar usuario
        Task<int> RegistrarAsync(Usuario usuario);

        // Actualizar último acceso
        Task ActualizarUltimoAccesoAsync(int usuarioId);

        // Listar todos los usuarios
        Task<List<Usuario>> ListarAsync();
    }
}
