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

        public ViewResult Login(Empresas empresa)
        {
            Session.Clear();
            Session["zona"] = "Zona Empresas";
            Empresas e = db.getEmpresa(empresa.IDEmpresa);
            if (ModelState.IsValid && e != null)
            {
                dc.cargarCategorias();
                Session["Job"] = e.Nombre;
                return View("Main", e);
            }
            else
            {
                ViewBag.menu = "Acceso";
                return View();
            }
        }

        public ViewResult Registro(Empresas empresa)
        {            
            Empresas e = db.getEmpresa(empresa.IDEmpresa);
            if (ModelState.IsValid && e != null)
            {
                ViewBag.menu = "Acceso";
                db.setEmpresa(empresa);
                dc.enviarEmail(empresa);
                return View("Login");
            }
            else
            {
                ViewBag.menu = "Registro";               
                return View();                
            }            
        }   
        
        public ViewResult Solicitudes(string id)
        {
            return View(db.getSolicitudes((int)Session["empresa"], int.Parse(id)));
        }     
    }
}