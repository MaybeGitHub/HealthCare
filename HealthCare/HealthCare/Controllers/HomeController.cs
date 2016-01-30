using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class HomeController : Controller
    {
        private BDController db = new BDController();

        [HttpGet]
        public ViewResult Index()
        {           
            return View();
        }

        [HttpPost]
        public ViewResult Index(Clientes cliente)
        {
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {         
                return View("Bienvenido", c);                              
            }
            return View();
        }
    }
}