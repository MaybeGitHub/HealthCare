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

        public dynamic getCategorias()
        {
            return db.Categorias.ToList();
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

        public void setEmpresa(Empresas empresa)
        {
            empresa.IDEmpresa = getIDEmpresa();
            if (empresa.Descripcion == null) empresa.Descripcion = "";
            db.Empresas.InsertOnSubmit(empresa);
            db.SubmitChanges();
        }

        public void setCliente(Clientes cliente)
        {
            db.Clientes.InsertOnSubmit(cliente);
            db.SubmitChanges();
        }

        private int getIDEmpresa()
        {
            int idObtenido = new Random().Next(1, Int32.MaxValue);
            while (db.Empresas.Where(x => x.IDEmpresa == idObtenido).Count() != 0)
            {
                idObtenido = new Random().Next(1, Int32.MaxValue);
            }
            return idObtenido;
        }

        public List<Empresas> getEmpresas(string subcategoria)
        {
            return db.Empresas.Where(x=> x.Subcategoria == subcategoria).ToList();
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
            return db.Solicitudes.Where(x => x.IDEmpresa == IDEmpresa && x.Estado == estado && x.Oculto == false).ToList();
        }

        public Solicitudes getSolicitud(int IDSolicitud)
        {
            return db.Solicitudes.SingleOrDefault(x => x.IDSolicitud == IDSolicitud);
        }

        public void setSolicitud(int ss, int IDEmpresa, int IDItem)
        {                 
            Solicitudes solicitud = new Solicitudes();
            solicitud.IDSolicitud = db.Solicitudes.Count() + 1;
            solicitud.Hora = DateTime.Now.ToString();
            solicitud.Estado = 1;
            solicitud.IDCliente = ss;
            solicitud.IDEmpresa = IDEmpresa;
            
            db.Solicitudes.InsertOnSubmit(solicitud);

            Pedidos pedido = new Pedidos();
            pedido.IDPedido = db.Pedidos.Count() + 1;
            pedido.IDCliente = ss;
            pedido.IDEmpresa = IDEmpresa;
            pedido.IDItem = IDItem;
            pedido.IDSolicitud = solicitud.IDSolicitud;

            db.Pedidos.InsertOnSubmit(pedido);

            db.SubmitChanges();
        }

        public void cambiarEstadoSolicitud(int idSolicitud, int estado)
        {
            Solicitudes s = db.Solicitudes.Single(x => x.IDSolicitud == idSolicitud);
            s.Estado = estado + 1;
            db.SubmitChanges();
        }

        public void borrarSolicitud(int IDSolicitud)
        {
            Solicitudes solicitud = getSolicitud(IDSolicitud);
            solicitud.Oculto = true;            
            db.SubmitChanges();
        }

        public List<Items> getItems(int idEmpresa)
        {
            return db.Items.Where(x => x.IDEmpresa == idEmpresa).ToList();
        }

        public Items getItem(int IDItem)
        {
            return db.Items.SingleOrDefault(x => x.IDItem == IDItem);
        }

        public IEnumerable<Pedidos> getPedidos(int ID, bool empresa)
        {
            if (empresa)
            {
                return db.Pedidos.Where(x => x.IDEmpresa == ID && x.Solicitudes.Oculto == false);
            }
            else
            {
                return null;
            }
        }
    }
}