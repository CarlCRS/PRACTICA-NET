using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_net1_LogiTrack.Modelo
{
    public abstract class AProducto
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }

        public AProducto(string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del producto nopuede estar vacio");
            }
            if (precio < 0) {
                throw new ArgumentException("El precio del producto no puede ser menor o igual a 0");
            }

            ID = ID + 1;
            Nombre = nombre;
            PrecioBase = precio;
        }

        public abstract decimal calcularCosto();
        public decimal ObtenerTotalCosto() => PrecioBase + calcularCosto();


    }
}
