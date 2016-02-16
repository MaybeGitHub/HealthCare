using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HealthCare.Models;

namespace HealthCare.Models
{
    public class Producto
    {
        public Items item { get; set; }
        public Empresas empresa {get; set;} 
    }
}