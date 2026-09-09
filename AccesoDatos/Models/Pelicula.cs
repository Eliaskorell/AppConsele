using System;
using System.Collections.Generic;
using System.Text;
using AccesoDatos.Data;

namespace AccesoDatos.Models
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public int CantidadDisponible { get; set; }

        // Navegación: una película puede estar en varios detalles de alquiler
        public List<DetalleAlquiler> DetallesAlquiler { get; set; } = new();
    }
}   