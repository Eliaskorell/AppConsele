using System;
using System.Collections.Generic;
using System.Text;

namespace AccesoDatos.Models
{
    public class Socio
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Id { get; set; }
        // DNI y Telefono como string para aceptar entradas desde consola
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public List<Alquiler> Alquileres { get; set; } = new();
    }
}
