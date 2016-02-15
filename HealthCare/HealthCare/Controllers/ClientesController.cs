using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class ClientesController : Controller
    {
        private BDController db = new BDController();
        private DatosController dc = new DatosController();

        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Login(Clientes cliente)
        {
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                Session["ss"] = c.SS;
                return RedirectToAction("Bienvenido");
            }
            return RedirectToAction("Login");
        }

        public ViewResult Bienvenido()
        {            
            return View(db.getCliente((int)Session["ss"]));
        }

        public ViewResult Registro()
        {
            return View();
        }

        public PartialViewResult Solicitud(string Items, string Empresas)
        {            
            List<string> listaEmpresas = Empresas.Split(':').ToList();
            List<string> listaItems = Items.Split(':').ToList();

            Pre_Solicitud ps = new Pre_Solicitud();
            ps.cliente = db.getCliente((int)Session["ss"]);
            foreach (string s in listaEmpresas) ps.listaEmpresas.Add(db.getEmpresa(s));
            foreach (string s in listaItems) ps.listaItems.Add(db.getItem(int.Parse(s)));
            
            return PartialView(ps);
        }
    }
}