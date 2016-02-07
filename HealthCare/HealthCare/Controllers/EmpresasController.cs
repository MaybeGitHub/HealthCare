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
        private DatosController dc = new DatosController();

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
                foreach(Items i in s.Items)
                {
                    items += i.Nombre + ", ";  
                }
               
                if ( s.Items.Count != 0 ) items = items.Substring(0, items.Length - 2);
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