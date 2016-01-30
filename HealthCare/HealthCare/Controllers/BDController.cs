using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HealthCare.Models;
using System.Configuration;

namespace HealthCare.Controllers
{
    public class BDController : Controller
    {
        private MiDBContextDataContext db { get; set; }

        public BDController()
        {
            db = new MiDBContextDataContext(ConfigurationManager.ConnectionStrings["miConexion"].ConnectionString);
        }

        public Clientes getCliente(int SS)
        {
            try
            {
                return db.Clientes.Single(x => x.SS == SS);
            }
            catch(InvalidOperationException)
            {
                return null;
            }
            
        }

        public Direcciones getDireccion(Clientes cliente)
        {
            return db.Direcciones.Single(x => x.IDDueño == cliente.SS);
        }

        public Direcciones getDireccion(Empresas empresa)
        {
            return db.Direcciones.Single(x => x.IDDueño == empresa.IDEmpresa);
        }

        public List<Prescripciones> getPrescripcion(Clientes cliente)
        {
            return db.Prescripciones.Where(x => x.IDCliente == cliente.SS).ToList();
        }

        public List<Items> getItems(Prescripciones prescripcion)
        {
            return db.Items.Where(x => x.IDPrescripcion == prescripcion.IDPrescripcion).ToList();
        }
    }
}