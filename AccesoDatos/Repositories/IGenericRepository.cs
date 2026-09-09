using System.Collections.Generic;

namespace AccesoDatos.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        void Agregar(T entidad);
        void Actualizar(T entidad);
        List<T> ObtenerTodos();
    }
}