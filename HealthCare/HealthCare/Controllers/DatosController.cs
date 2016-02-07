using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class DatosController : Controller
    {
        private BDController db = new BDController();

        public string Categorias(string categoria)
        { 
            Categorias c = db.getCategoria(categoria);
            if(c != null)
            {
                string salida = "{'Nombre':'" + c.Nombre + "','Color':'" + c.Color + "'}";
                return salida.Replace("'", "\"");
            }
            else
            {
                return "null";
            }
            
        }

        public string Subcategorias(string categoria)
        {
            string json = "[";
            List<Subcategorias> lista = db.getSubcategorias(categoria);
            foreach (Subcategorias s in lista)
            {
                json += "{'Nombre':'" + s.Nombre + "','Color':'" + s.Color + "'},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json.Replace("'", "\"");
        }

        public string Empresas(string categoria, string subcategoria)
        {
            string json = "[";
            List<Empresas> lista = db.getEmpresas(categoria, subcategoria).OrderByDescending(x => x.Valoracion).ToList();
            foreach (Empresas e in lista)
            {
                json += "{'Nombre':'" + e.Nombre + "','Valoracion':" + e.Valoracion + ",'IDEmpresa':" + e.IDEmpresa + "},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json.Replace("'", "\"");
        }        

        public string Items(int IDEmpresa)
        {
            string json = "[";
            List<Items> lista = db.getItems(IDEmpresa);
            if(lista == null)
            {
                lista = new List<Items>();
            }
            
            foreach (Items i in lista)
            {
                json += "{'Nombre':'" + i.Nombre + "','IDItem':" + i.IDItem + ",'Precio':'" + i.Precio + "'},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json.Replace("'", "\"");
            
        }

        public int Solicitud(int IDEmpresa, int IDCliente, int IDItem)
        {            
            db.setSolicitud(IDCliente, IDEmpresa, IDItem);
            return 1;
        }

        public string obtenerSolicitudes(int IDEmpresa, int estado)
        {
            string json = "[";

            foreach (Solicitudes s in db.getSolicitudes(IDEmpresa, estado))
            {
                string items = "";
                items += "'";
                foreach (Items i in s.Items)
                {
                    items += i.Nombre + ", ";
                }

                if (s.Items.Count > 0) items = items.Substring(0, items.Length - 2);
                items += "'";
                json += "{'Nombre':'" + s.Clientes.Nombre + "','Direccion':'" + s.Clientes.Direcciones.Calle + " " + s.Clientes.Direcciones.Numero + "','Hora':'" + s.Hora + "','Telefono':" + s.Clientes.Telefono + ",'IDCliente':" + s.IDCliente + ",'Items':" + items + ",'IDSolicitud':" + s.IDSolicitud + "},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json.Replace("'", "\"");
        }

        public int cambiarEstadoSolicitud(int idEmpresa, int idCliente, int estado)
        {
            return db.cambiarEstadoSolicitud(idEmpresa, idCliente, estado);
        }

        public void borrarSolicitud(int IDSolicitud)
        {
            db.borrarSolicitud(IDSolicitud);
        }
    }
}