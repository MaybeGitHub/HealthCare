using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class Cesta
    {
        public List<Empresa> listaEmpresas { get; set; }
        public List<Item> listaItems { get; set; }
        public Cliente cliente { get; set; }

        public Cesta()
        {
            listaEmpresas = new List<Empresa>();
            listaItems = new List<Item>();
            cliente = null;
        }
    }
}