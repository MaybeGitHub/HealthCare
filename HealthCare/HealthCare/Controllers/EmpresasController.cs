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
            Empresas e = db.getEmpresa(empresa.IDEmpresa);
            if (ModelState.IsValid && e != null)
            {
                Session["zona"] = "Zona Empresas";
                dc.cargarCategorias();
                Session["worker"] = e.IDEmpresa;
                return View("SolicitudesEspera", db.getPedidos(e.IDEmpresa, true).Where(x => x.Solicitudes.Estado == 1));
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

        public ViewResult SolicitudesEspera()
        {
            int idEmpresa = (int)Session["worker"];
            return View(db.getPedidos(idEmpresa, true).Where(x => x.Solicitudes.Estado == 1));
        }

        public ViewResult SolicitudesProceso()
        {
            int idEmpresa = (int)Session["worker"];
            return View(db.getPedidos(idEmpresa, true).Where(x => x.Solicitudes.Estado == 2));
        }

        public ViewResult SolicitudesTerminadas()
        {
            int idEmpresa = (int)Session["worker"];
            return View(db.getPedidos(idEmpresa, true).Where(x => x.Solicitudes.Estado == 3));
        }

        public ViewResult SugerirProducto()
        {
            return View();
        }

        public RedirectToRouteResult MoverUna(string estado, string idSolicitud)
        {
            string view = "SolicitudesProceso";
            int est = int.Parse(estado);
            if(est == 2)
            {
                view = "SolicitudesTerminadas";
            }            
            db.cambiarEstadoSolicitud(int.Parse(idSolicitud), est);
            return RedirectToAction(view);
        }

        public RedirectToRouteResult BorrarUna(string estado, string idSolicitud)
        {
            db.borrarSolicitud(int.Parse(idSolicitud));
            string view = "SolicitudesProceso";
            int est = int.Parse(estado);
            if (est == 3)
            {
                view = "SolicitudesTerminadas";
            }else if(est == 1)
            {
                view = "SolicitudesEspera";
            }
            return RedirectToAction(view);
        }

        public RedirectToRouteResult MoverTodas(string estado, string cliente)
        {
            int idEmpresa = (int)Session["worker"];
            string view = "SolicitudesProceso";
            int est = int.Parse(estado);
            if (est == 2)
            {
                view = "SolicitudesTerminadas";
            }

            IEnumerable<Solicitudes> listaSolicitudes = db.getSolicitudes(idEmpresa, est).Where(x => x.IDCliente == int.Parse(cliente));
            foreach (Solicitudes s in listaSolicitudes)
                db.cambiarEstadoSolicitud(s.IDSolicitud, s.Estado);
            return RedirectToAction(view);
        }        

        public RedirectToRouteResult BorrarTodas(string estado, string cliente)
        {
            IEnumerable<Solicitudes> solicitudes = db.getSolicitudes((int)Session["worker"], int.Parse(estado)).Where(x => x.IDCliente == int.Parse(cliente) && x.IDEmpresa == (int)Session["worker"] && x.Estado == int.Parse(estado) && x.Oculto == false);
            foreach (Solicitudes s in solicitudes)
                db.borrarSolicitud(s.IDSolicitud);

            string view = "SolicitudesProceso";
            int est = int.Parse(estado);
            if (est == 3)
            {
                view = "SolicitudesTerminadas";
            }
            else if (est == 1)
            {
                view = "SolicitudesEspera";
            }
            return RedirectToAction(view);
        }
    }
}