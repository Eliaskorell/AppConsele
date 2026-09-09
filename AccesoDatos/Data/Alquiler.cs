using System;
using System.Collections.Generic;
using System.Text;


namespace AccesoDatos.Data
{
    public class Alquiler
    {
        public int id { get; set; }
        public int SocioId { get; set; }
        public Socio Socio { get; set; }
        public DateTime FechaAlquiler { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public List<DetalleAlquiler> Detalles { get; set; } = new();
    }
}
