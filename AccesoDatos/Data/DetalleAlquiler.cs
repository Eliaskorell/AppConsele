using System;
using System.Collections.Generic;
using System.Text;

namespace AccesoDatos.Data
{
    public class DetalleAlquiler
    {
        public int Id { get; set; }

        // FK hacia Alquiler
        public int AlquilerId { get; set; }
        public Alquiler Alquiler { get; set; }

        // FK hacia Pelicula
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }

        public int DiasAlquilados { get; set; }
        public decimal MontoBase { get; set; }   // precio por los días pactados
        public decimal Recargo { get; set; }     // 10% por día de demora, se calcula al devolver
        public decimal MontoTotal { get; set; }  // MontoBase + Recargo
    }

}

