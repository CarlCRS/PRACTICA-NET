using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica_net1_LogiTrack.Modelo
{
    internal class ProductoFisico : AProducto
    {
        public decimal PesoKg { get; }
        private const decimal Costokg = 2.5m;
        public ProductoFisico(string nombre, decimal precio, decimal pesoKg ) : base(nombre, precio)
        {
            if (pesoKg <= 0)
            {
                throw new ArgumentException("El peso no puede ser 0 o inferior");
            }
            PesoKg = pesoKg;
        }

        public override decimal calcularCosto()
        {
           return Precio + (PesoKg* Costokg);
        }
    }
}
