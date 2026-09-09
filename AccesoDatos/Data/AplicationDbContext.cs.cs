using Microsoft.EntityFrameworkCore;
using AccesoDatos.Models;

namespace AccesoDatos.Data
{
    public class AplicationDbContext : DbContext
    {
        // Cada DbSet representa una tabla en la base de datos
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Alquiler> Alquileres { get; set; }
        public DbSet<DetalleAlquiler> DetallesAlquiler { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Le decimos a EF Core que use SQLite y dónde guardar el archivo
            optionsBuilder.UseSqlite("Data Source=videoclub.db");
        }
    }
}