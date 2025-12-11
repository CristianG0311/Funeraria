using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funeraria.Aplicacion.DTOs.Auth
{
    /// <summary>
    /// DTO para iniciar sesión (CU-001)
    /// </summary>
    public class LoginRequest
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
    }
}
