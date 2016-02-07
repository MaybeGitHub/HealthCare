using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HealthCare.Models;
using System.Configuration;
using System.Data.Linq;

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
            return db.Clientes.SingleOrDefault(x => x.SS == SS);                      
        }

        public Empresas getEmpresa(int iDEmpresa)
        {            
            return db.Empresas.SingleOrDefault(x => x.IDEmpresa == iDEmpresa);                      
        }

        public Empresas getEmpresa(string nombreEmpresa)
        {
            return db.Empresas.SingleOrDefault(x => x.Nombre == nombreEmpresa);
        }

        public List<Empresas> getEmpresas(string categoria, string subcategoria)
        {
            return db.Empresas.Where(x => x.Categoria == categoria && x.Subcategoria == subcategoria).ToList();
        }

        public Categorias getCategoria(string categoria)
        {
            return db.Categorias.SingleOrDefault(x => x.Nombre == categoria);
        }

        public List<Subcategorias> getSubcategorias(string categoria)
        {
            return db.Subcategorias.Where(x => x.Categoria == categoria).OrderBy(x => x.Nombre).ToList();
        }

        public Direcciones getDireccion(Clientes cliente)
        {
            return db.Direcciones.Single(x => x.IDDueño == cliente.SS);
        }

        public Direcciones getDireccion(Empresas empresa)
        {
            return db.Direcciones.Single(x => x.IDDueño == empresa.IDEmpresa);
        }        

        public List<Solicitudes> getSolicitudes(int IDEmpresa, int estado)
        {
            return db.Solicitudes.Where(x => x.IDEmpresa == IDEmpresa && x.Estado == estado).ToList();
        }

        public void setSolicitud(int ss, int IDEmpresa)
        {            
            Solicitudes solicitud = new Solicitudes();            
            solicitud.Hora = DateTime.Now.ToString();            
            solicitud.Estado = 1;
            solicitud.IDCliente = ss;
            solicitud.IDEmpresa = IDEmpresa;
            solicitud.IDSolicitud = db.Solicitudes.Count();
            db.Solicitudes.InsertOnSubmit(solicitud);
            db.SubmitChanges();
        }

        public int cambiarEstadoSolicitud(int idEmpresa, int idCliente, int estado)
        {
            Solicitudes s = db.Solicitudes.Single(x => x.IDEmpresa == idEmpresa && x.Estado == estado && x.IDCliente == idCliente);
            if (s.Estado < 3) s.Estado = estado + 1;
            try
            {
                db.SubmitChanges();
                return 1;
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        public List<Items> getItems(int IDEmpresa)
        {
            return db.Items.Where(x => x.IDEmpresa == IDEmpresa).ToList();
        }

        public Items getItem(int IDItem)
        {
            return db.Items.SingleOrDefault(x => x.IDItem == IDItem);
        }
    }
}