using APIRegistro.Models;
using Microsoft.EntityFrameworkCore;

namespace APIRegistro.Data
{
    public class ApplicationDbContext : DbContext
    
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
