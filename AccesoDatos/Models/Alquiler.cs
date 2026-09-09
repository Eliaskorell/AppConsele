using System;
using System.Collections.Generic;
using System.Text;

namespace AccesoDatos.Models
{
    public class Alquiler
    {
        public int Id { get; set; }
        public int SocioId { get; set; }
        public Socio Socio { get; set; }
        public DateTime FechaAlquiler { get; set; }
        // Renombrada a FechaDevolucionPactada para coincidir con el uso en Program.cs
        public DateTime FechaDevolucionPactada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public List<DetalleAlquiler> Detalles { get; set; } = new();
    }
}
