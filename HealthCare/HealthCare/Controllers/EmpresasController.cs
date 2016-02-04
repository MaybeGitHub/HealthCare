using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class EmpresasController : Controller
    {
        private BDController db = new BDController();

        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(Empresas empresa)
        {
            Empresas e = db.getEmpresa(empresa.IDEmpresa);
            if (ModelState.IsValid && e != null)
            {

                return View("Bienvenido", e);
            }
            else
            {
                return View();
            }
        }
        
        public string obtenerSolicitudes(int IDEmpresa, int estado)
        {
            string json = "[";
            
            foreach (Solicitudes s in db.getSolicitudes(IDEmpresa, estado))
            {
                string items = "";
                items += "\"";
                foreach (Prescripciones p in db.getPrescripciones(s.Clientes, s.Empresas))
                {
                    foreach(Items i in p.Items)
                    {
                        items += i.Nombre + " ";  
                    }
                }
                items += "\"";
                json += "{\"Nombre\":\"" + s.Clientes.Nombre + "\",\"Direccion\":\"" + s.Clientes.Direcciones.Calle + " " + s.Clientes.Direcciones.Numero + "\",\"Hora\":\"" + s.Hora + "\",\"Telefono\":" + s.Clientes.Telefono + ",\"IDCliente\":" + s.IDCliente + ",\"Items\":" + items + "},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json;
        }    
        
        public int cambiarEstadoSolicitud(int idEmpresa, int idCliente, int estado)
        {
            return db.cambiarEstadoSolicitud(idEmpresa,idCliente, estado);
        }
    }
}