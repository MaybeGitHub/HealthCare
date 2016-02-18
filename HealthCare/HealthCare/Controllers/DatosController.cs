using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class DatosController : Controller
    {
        private BDController db = new BDController();

        public dynamic cargarCategorias()
        {
            return db.getCategorias();            
        }

        public dynamic Subcategorias(string categoria)
        {
            return db.getSubcategorias(categoria);
        }

        public dynamic Empresas(string subcategoria)
        {
            return db.getEmpresas(subcategoria);
        }        

        public Empresas Empresa(string nombre)
        {
            return db.getEmpresa(nombre);
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

        public dynamic Items(int idEmpresa)
        {
            return db.getItems(idEmpresa);
        }

        public Items Item(int idItem)
        {
            return db.getItem(idItem);
        }

        public int cambiarEstadoSolicitud(int idEmpresa, int idCliente, int estado)
        {
            return db.cambiarEstadoSolicitud(idEmpresa, idCliente, estado);
        }

        public void borrarSolicitud(int IDSolicitud)
        {
            db.borrarSolicitud(IDSolicitud);
        }

        public void enviarEmail(Cesta solicitud)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "proyectos.clase.net@gmail.com";
            WebMail.Password = "solosequenosenada";
            WebMail.From = "proyectos.clase.net@gmail.com";
            WebMail.Send(solicitud.cliente.Email, "Solicitud Realizada", "Clientes: Aun por implementar");
            solicitud.listaEmpresas.ForEach(x => WebMail.Send(x.Email, "Solicitud Realizada", "Empresas: Aun por implementar"));
        }

        public void enviarEmail(Empresas empresa)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "proyectos.clase.net@gmail.com";
            WebMail.Password = "solosequenosenada";
            WebMail.From = "proyectos.clase.net@gmail.com";
            WebMail.Send(empresa.Email, "Numero de identificacion de la empresa", "Su empresa ha quedado registrada en la categoria: " + empresa.Categoria + "/" + empresa.Subcategoria + ". Se le ha asignado el siguiente numero de identificacion: " + empresa.IDEmpresa + " \n Debera usar este numero para acceder a su portal. \n Gracias por confiar en HealthCare y Bienvenido");
        }
    }
}