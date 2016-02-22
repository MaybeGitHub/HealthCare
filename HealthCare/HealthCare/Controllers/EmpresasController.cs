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
                ViewBag.actual = "Empresas";
                if (empresa.IDEmpresa != 0) ViewBag.error = "La empresa no existe";
                return View();
            }
        }

        public ViewResult Registro(Empresa empresa, string modificar)
        {
            ViewBag.actual = "Empresas";
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
                        dc.enviarEmail(empresa, false);
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

        public ViewResult Recuperar(Empresa empresa)
        {
            Empresa emp = db.getEmpresa(empresa.Nombre);
            ViewBag.actual = "Empresas";
            if (emp != null)
            {
                if(emp.Email == empresa.Email)
                {
                    dc.enviarEmail(emp, true);
                    return View("Confirmacion");
                }
                else
                {
                    ViewBag.errorEmail = "El email no es correcto";
                    return View();
                }
            }
            else
            {
                if(empresa.Nombre != null ) ViewBag.errorNombre = "La empresa no existe";
                return View();
            }
        }

        public ViewResult NuevaID(string id)
        {
            string nombreEmpresa = EncryptionHelper.Decrypt(id);
            Empresa empresa = db.getEmpresa(nombreEmpresa);
            db.modificarIDEmpresa(empresa);
            dc.enviarEmail(empresa, false);
            ViewBag.ID = empresa.IDEmpresa;
            return View();
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

        public RedirectToRouteResult MoverUna(string estado, string IDPedido, string ss)
        {
            Pedido pedido = db.getPedido(long.Parse(IDPedido));            
            int status = int.Parse(estado);
            if(status == 1)
            {
                db.cambiarEstadoPedido(pedido);
                return RedirectToAction("PedidosProceso");
            }
            else if(status == 2)
            {                
                if (ss != "" && pedido.Cliente.SS == long.Parse(ss))
                {
                    db.cambiarEstadoPedido(pedido);
                    return RedirectToAction("PedidosTerminadas");
                }
                else
                {
                    TempData["error"] = "El cliente no es correcto";
                    return RedirectToAction("PedidosProceso");
                }
            }
            else
            {
                return RedirectToAction("PedidosEspera");
            }
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

        public RedirectToRouteResult MoverTodas(string estado, string cliente, string ss)
        {
            long IDEmpresa = (long)Session["worker"];
            int status = int.Parse(estado);

            IEnumerable<Pedido> listaPedidos = db.getPedidos(IDEmpresa, status).Where(x => x.IDCliente == long.Parse(cliente));
            foreach (Pedido pedido in listaPedidos)
            {
                if (status == 1)
                {
                    db.cambiarEstadoPedido(pedido);                   
                }
                else if (status == 2)
                {
                    if (ss != "" && pedido.Cliente.SS == long.Parse(ss))
                    {
                        db.cambiarEstadoPedido(pedido);
                    }
                    else
                    {
                        TempData["errorTodos"] = "El cliente no es correcto";
                        return RedirectToAction("PedidosProceso");
                    }                    
                }
                else
                {
                    return RedirectToAction("PedidosEspera");
                }
            }

            if(status == 1)
            {
                return RedirectToAction("PedidosProceso");
            }
            else
            {
                return RedirectToAction("PedidosTerminadas");
            }            
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