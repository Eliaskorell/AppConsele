using System;
using System.Collections.Generic;
using System.Text;
using AccesoDatos.Data;

namespace AccesoDatos.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AplicationDbContext _context;

        public GenericRepository()
        {
            _context = new AplicationDbContext();
            _context.Database.EnsureCreated();
        }

        public void Agregar(T entidad)
        {
            // Set() selecciona la tabla correcta dependiendo de la clase que le pases
            _context.Set<T>().Add(entidad);
            _context.SaveChanges(); // Aquí se ejecuta el INSERT
        }

        public List<T> ObtenerTodos()
        {
            return _context.Set<T>().ToList(); // Aquí se ejecuta el SELECT
        }
    }
}