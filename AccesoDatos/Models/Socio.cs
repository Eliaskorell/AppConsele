using System;
using System.Collections.Generic;
using System.Text;

namespace AccesoDatos.Data
{
    public class Socio
    {
        public string nombre { get; set;}
        public string apellido { get; set;}
        public int id { get; set;}
        public int dni { get; set;}
        public int telefono { get; set; }
        public List<Alquiler> Alquileres { get; set; } = new();
    }
}
