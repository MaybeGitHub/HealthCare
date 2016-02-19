using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
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

        public void enviarEmail(Cesta solicitud)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "proyectos.clase.net@gmail.com";
            WebMail.Password = "solosequenosenada";
            WebMail.From = "proyectos.clase.net@gmail.com";           
            WebMail.Send(solicitud.cliente.Email, "Solicitud Realizada", bodyEmail(solicitud, null, false));
            solicitud.listaEmpresas.ForEach(x => WebMail.Send(x.Email, "Solicitud Realizada", bodyEmail(solicitud, x, true)));            
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

        public string bodyEmail(Cesta cesta, Empresas empresa, bool isEmpresa)
        {
            string ret = "";
            if (isEmpresa)
            {
                ret += "<p>Un cliente ha solicitado a su empresa los siguientes productos mediante nuestra pagina web: </p>";
                ret += "<ul>";
                foreach(Items item in cesta.listaItems.Where(x => x.IDEmpresa == empresa.IDEmpresa))
                {
                    ret += "<li>" + item.Nombre + "</li>";
                }
                ret += "</ul>";
                ret += "<p>Rogamos que se ponga en contacto con su cliente lo antes posible via telefono al numero: " + cesta.cliente.Telefono + "</p>";                
                ret += "<p>Le recordamos que completar este pedido le subira valoracion a su empresa, al igual que rechazarlo, hara que disminuya</p>";
                ret += "<p>Muchas gracias por ser parte de nuestras empresas de confianza</p>";
                ret += "<p>Equipo de control de calidad de HealthCare</p>";
            }
            else
            {
                ret += "<p>Has solicitado los siguientes productos mediante nuestra web a las empresas que listamos a continuacion: </p>";                
                foreach (Empresas e in cesta.listaEmpresas)
                {
                    ret += "Empresa: " + e.Nombre;
                    ret += "<ul>";
                    foreach (Items item in cesta.listaItems.Where(x => x.IDEmpresa == e.IDEmpresa))
                    {
                        ret += "<li>" + item.Nombre + "</li>";
                    }
                    ret += "</ul>";
                    ret += "<br />";
                }
                
                ret += "<p>Se pondran en contacto con usted lo antes posible al telefono: " + cesta.cliente.Telefono + "</p>";
                ret += "<p>Le recordamos que debera concretar la hora de la entrega con la empresa directamente para que el producto le llegue lo antes posible</p>";
                ret += "<p>Muchas gracias por utilizar nuestros servicios y por confiar en nuestra pagina web</p>";
                ret += "<p>Equipo de control de calidad de HealthCare</p>";
            }
            return ret;
        }
    }
}