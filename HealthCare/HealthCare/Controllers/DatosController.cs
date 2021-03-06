﻿using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace HealthCare.Controllers
{
    public class DatosController : Controller
    {
        private BDController db = new BDController();
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        public string existe(string ss)
        {
            string json = "{'valido':";

            if (db.getCliente(long.Parse(ss)) == null)
            {
                json += 1;
            }
            else
            {
                json += 0 + ", 'mensaje':'Este numero de la Segurida Social ya esta registrado'";
            } 

            return (json + "}").Replace('\'', '"');
        }

        public dynamic cargarCategorias()
        {
            return db.getCategorias();            
        }

        public dynamic Subcategorias(string categoria, string returnJson)
        {
            List<Subcategoria> lista = db.getSubcategorias(categoria);
            if (bool.Parse(returnJson))
            {
                string json = "[";
                foreach(Subcategoria s in lista)
                {
                    json += "{'Nombre':'" + s.Nombre + "'},";
                }

                if(json.Length > 0)
                {
                    json = json.Substring(0, json.Length - 1);
                }
                return  (json + "]").Replace('\'', '"');
            }
            else
                return lista;
        }

        public dynamic Empresas(string subcategoria)
        {
            return db.getEmpresas(subcategoria);            
        }        

        public dynamic Items(long idEmpresa)
        {
            return db.getItems(idEmpresa);
        }

        public Item Item(long idItem)
        {
            return db.getItem(idItem);
        }

        public void enviarEmail(Cesta cesta)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "proyectos.clase.net@gmail.com";
            WebMail.Password = "solosequenosenada";
            WebMail.From = "proyectos.clase.net@gmail.com";           
            WebMail.Send(cesta.cliente.Email, "Pedido Realizada", bodyEmail(cesta, null, false));
            cesta.listaEmpresas.ForEach(x => WebMail.Send(x.Email, "Pedido Realizada", bodyEmail(cesta, x, true)));            
        }

        public void enviarEmail(Empresa empresa, bool recuperacion)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "proyectos.clase.net@gmail.com";
            WebMail.Password = "solosequenosenada";
            WebMail.From = "proyectos.clase.net@gmail.com";

            string hash = EncryptionHelper.Encrypt(empresa.Nombre);


            if (recuperacion)
            {
                WebMail.Send(empresa.Email, "Recuperacion de Cuenta", "Hemos recibido su peticion para recuperar su cuenta</p><br /><p>Si es usted consciente de la peticion, haz click <a href='http://localhost:18500/Empresas/NuevaID/" + hash + "'>Aqui</a><br/><br/><p>Si no es asi, porfavor pongase en contacto con nosotros cuanto antes para solucionar esto</p><p>Muchas gracias por utilizar nuestros servicios</p><br /><br /><p>Atentamente, Equipo de Proteccion de Datos de HealthCare");
            }
            else
            {
                WebMail.Send(empresa.Email, "Numero de identificacion de la empresa", "<p>Su empresa ha quedado registrada en la categoria: " + empresa.Categoria + "/" + empresa.Subcategoria + "</p></p>Se le ha asignado el siguiente numero de identificacion: " + empresa.IDEmpresa + " </p><p>Debera usar este numero para acceder a su portal</p><br /><p>Gracias por confiar en HealthCare y bienvenido</p>");
            }
            
        }

        public string bodyEmail(Cesta cesta, Empresa empresa, bool isEmpresa)
        {
            string ret = "";
            if (isEmpresa)
            {
                ret += "<p>Un cliente ha solicitado a su empresa los siguientes productos mediante nuestra pagina web: </p>";
                ret += "<ul>";
                foreach(Item item in cesta.listaItems.Where(x => x.IDEmpresa == empresa.IDEmpresa))
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
                foreach (Empresa e in cesta.listaEmpresas)
                {
                    ret += "Empresa: " + e.Nombre;
                    ret += "<ul>";
                    foreach (Item item in cesta.listaItems.Where(x => x.IDEmpresa == e.IDEmpresa))
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

        public int cuantosPedidos(string id)
        {
            if(Session["worker"] != null)
            {
                return db.getPedidos((long)Session["worker"], int.Parse(id)).Count();
            }
            else
            {
                return 0;
            }            
        }

        public int valoracion(string id)
        {
            return db.getEmpresa(id).Valoracion;
        }
    }    
}