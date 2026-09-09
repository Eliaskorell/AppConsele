using System.Collections.Generic;

namespace AccesoDatos.Repositories
{
    // La T significa que acepta cualquier clase (Pelicula, Socio, Alquiler, etc.)
    public interface IGenericRepository<T> where T : class
    {
        void Agregar(T entidad);
        List<T> ObtenerTodos();
    }
}
