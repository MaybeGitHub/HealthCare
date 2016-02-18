using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class Cesta
    {
        public List<Empresas> listaEmpresas { get; set; }
        public List<Items> listaItems { get; set; }
        public Clientes cliente { get; set; }

        public Cesta()
        {
            listaEmpresas = new List<Empresas>();
            listaItems = new List<Items>();
            cliente = null;
        }
    }
}