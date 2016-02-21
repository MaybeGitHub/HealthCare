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

        public ViewResult Login(Empresa empresa)
        {
            Session.Clear();            
            Empresa e = db.getEmpresa(empresa.IDEmpresa);
            if (ModelState.IsValid && e != null)
            {
                Session["zona"] = "Zona Empresas";
                dc.cargarCategorias();
                Session["worker"] = e.IDEmpresa;
                Session["nombreEmpresa"] = db.getEmpresa(e.IDEmpresa).Nombre;
                return View("PedidosEspera", db.getPedidos(e.IDEmpresa, 1));
            }
            else
            {
                ViewBag.menu = "Acceso";
                if (empresa.IDEmpresa != 0) ViewBag.error = "La empresa no existe";
                return View();
            }
        }

        public ViewResult Registro(Empresa empresa, string modificar)
        {

            if (Session["worker"] != null)
            {
                if (empresa.Nombre == null)
                {
                    empresa = db.getEmpresa((long)Session["worker"]);
                }
                else
                {
                    empresa.IDEmpresa = (long)Session["worker"];
                }
            }


            if (modificar != null)
            {
                if (ModelState.IsValid)
                {
                    ViewBag.menu = "Acceso";
                    db.setEmpresa(empresa);
                    Session["nombreEmpresa"] = db.getEmpresa(empresa.IDEmpresa).Nombre;
                    if (Session["worker"] != null)
                    {
                        return View("PedidosEspera", db.getPedidos(empresa.IDEmpresa, 1));
                    }
                    else
                    {
                        dc.enviarEmail(empresa);
                        return View("Login");
                    }
                }
                else
                {
                    return View(empresa);
                }
            }
            else
            {
                ViewBag.menu = "Registro";
                if (Session["worker"] != null)
                {
                    return View(empresa);
                }
                else
                {
                    return View(empresa);
                }
            }           
        }

        public ViewResult PedidosEspera()
        {
            long idEmpresa = (long)Session["worker"];
            return View(db.getPedidos(idEmpresa, 1));
        }

        public ViewResult PedidosProceso()
        {
            long idEmpresa = (long)Session["worker"];
            return View(db.getPedidos(idEmpresa, 2));
        }

        public ViewResult PedidosTerminadas()
        {
            long idEmpresa = (long)Session["worker"];
            return View(db.getPedidos(idEmpresa, 3));
        }

        public ViewResult SugerirProducto()
        {
            return View();
        }

        public ViewResult HacerPeticion(Item item)
        {
            if (!db.setItem(item, (long)Session["worker"])){
                ViewBag.error = "No se ha completado su sugerencia debido a errores en su proceso";
            }
            return View(item);            
        }

        public RedirectToRouteResult MoverUna(string estado, string IDPedido)
        {
            string view = "PedidosProceso";
            int est = int.Parse(estado);
            if(est == 2)
            {
                view = "PedidosTerminadas";
            }            
            db.cambiarEstadoPedido(long.Parse(IDPedido), est);
            return RedirectToAction(view);
        }

        public RedirectToRouteResult BorrarUna(string estado, string IDPedido)
        {
            db.borrarPedido(long.Parse(IDPedido));
            string view = "PedidosProceso";
            int est = int.Parse(estado);
            if (est == 3)
            {
                view = "PedidosTerminadas";
            }else if(est == 1)
            {
                view = "PedidosEspera";
            }
            return RedirectToAction(view);
        }

        public RedirectToRouteResult MoverTodas(string estado, string cliente)
        {
            long idEmpresa = (long)Session["worker"];
            string view = "PedidosProceso";
            int est = int.Parse(estado);
            if (est == 2)
            {
                view = "PedidosTerminadas";
            }

            IEnumerable<Pedido> listaPedidos = db.getPedidos(idEmpresa, est).Where(x => x.IDCliente == long.Parse(cliente));
            foreach (Pedido s in listaPedidos)
                db.cambiarEstadoPedido(s.IDPedido, s.Estado);
            return RedirectToAction(view);
        }        

        public RedirectToRouteResult BorrarTodas(string estado, string cliente)
        {
            IEnumerable<Pedido> Pedidos = db.getPedidos((long)Session["worker"], int.Parse(estado)).Where(x => x.IDCliente == long.Parse(cliente) && x.IDEmpresa == (long)Session["worker"] && x.Estado == int.Parse(estado) && x.Oculto == false);
            foreach (Pedido s in Pedidos)
                db.borrarPedido(s.IDPedido);

            string view = "PedidosProceso";
            int est = int.Parse(estado);
            if (est == 3)
            {
                view = "PedidosTerminadas";
            }
            else if (est == 1)
            {
                view = "PedidosEspera";
            }
            return RedirectToAction(view);
        }
    }
}