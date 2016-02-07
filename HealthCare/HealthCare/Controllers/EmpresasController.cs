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
    }
}