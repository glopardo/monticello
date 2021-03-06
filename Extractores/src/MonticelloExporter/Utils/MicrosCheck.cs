﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MicrosCheck
    {
        public string Pcws { get; set; }
        public string CheckNumber { get; set; }
        public string EdiNumber { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Alimentos { get; set; }
        public int BebidasCAlcohol { get; set; }
        public int BebidasSAlcohol { get; set; }
        public int Tabacos { get; set; }
        public int Otros { get; set; }
        public int TotalNeto { get; set; }
        public int TotalExento { get; set; }
        public int Iva { get; set; }
        public int Descuentos { get; set; }
        public int Propinas { get; set; }
        public int VentasTotales { get; set; }
        public Dictionary<int, int> Payments { get; set; }
        public MicrosCheck()
        {
            Alimentos = BebidasCAlcohol = BebidasSAlcohol = Tabacos = Otros = 
                TotalNeto = TotalExento = Iva = VentasTotales = 0;
        }
    }
}
