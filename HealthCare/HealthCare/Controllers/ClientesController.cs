using HealthCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HealthCare.Controllers
{
    public class ClientesController : Controller
    {
        private BDController db = new BDController();
        private DatosController dc = new DatosController();        

        public ViewResult Login(Cliente cliente)
        {
            Session.Clear();            
            Cliente c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                Session["zona"] = "Zona Clientes";
                Session["cliente"] = c.SS;
                Session["nombrecliente"] = c.Nombre;
                Cesta cesta = new Cesta();
                cesta.cliente = c;
                Session["cesta"] = cesta;
                ViewBag.menu = "Inicio";
                return View("Categorias", dc.cargarCategorias());
            }
            else
            {
                ViewBag.menu = "Acceso";
                if(cliente.SS != 0) ViewBag.error = "El cliente no existe";
                ViewBag.actual = "Clientes";
                return View("Login");
            }            
        }

        public ViewResult Registro(Cliente cliente, string modificar)
        {
            if(modificar != null)
            {
                if (ModelState.IsValid)
                {
                    ViewBag.menu = "Acceso";
                    db.setCliente(cliente);
                    Session["nombrecliente"] = db.getCliente(cliente.SS).Nombre;
                    if (Session["cliente"] != null)
                    {
                        return View("Categorias", dc.cargarCategorias());
                    }
                    else
                    {
                        return View("Login");
                    }                    
                }
                else
                {
                    return View(cliente);
                }
            }
            else
            {
                ViewBag.menu = "Registro";
                if (Session["cliente"] != null)
                {
                    return View(db.getCliente((long)Session["cliente"]));
                }
                else
                {
                    return View(cliente);
                }
            }                              
        }       

        public ViewResult Categorias()
        {
            Session.Remove("categoria");
            Session.Remove("subcategoria");
            Session.Remove("empresa");

            ViewBag.menu = "Inicio";
            return View(dc.cargarCategorias());
        }

        public ViewResult Subcategorias(string id)
        {
            Session["categoria"] = id;
            Session.Remove("subcategoria");
            Session.Remove("empresa");
            ViewBag.menu = "Categoria";
            return View(dc.Subcategorias(id, "false"));                    
        }

        public ViewResult Empresas(string id)
        {
            Session["subcategoria"] = id;
            Session.Remove("empresa");
            ViewBag.menu = "Subcategoria";
            return View(db.getEmpresas(id));
        }

        public ViewResult Items(string id)
        {
            Empresa e = db.getEmpresa(id);
            Session["empresa"] = e.Nombre;
            ViewBag.menu = "Empresa";
            return View(dc.Items(e.IDEmpresa));
        }

        public ViewResult Comprar(string id)
        {
            Cesta cesta = Session["cesta"] as Cesta;
            cesta.listaItems.Add(db.getItem(long.Parse(id)));
            Session["cesta"] = cesta;
            return View();
        } 

        
        public ViewResult Cesta()
        {
            Cesta cesta = Session["cesta"] as Cesta;
            cesta.listaItems.ForEach(x => cesta.listaEmpresas.Add(x.Empresa));
            cesta.listaEmpresas = cesta.listaEmpresas.GroupBy(x => x.Nombre).Select(x => x.First()).ToList();
            Session["cesta"] = cesta;
            return View(cesta);
        }
                
        public RedirectToRouteResult Borrar(string posicion)
        {
            Cesta cesta = Session["cesta"] as Cesta;            
            cesta.listaItems.RemoveAt(int.Parse(posicion));
            cesta.listaEmpresas.RemoveAll(x => cesta.listaItems.Where(y => y.IDEmpresa == x.IDEmpresa).Count() == 0);            
            Session["cesta"] = cesta;
            return RedirectToAction("Cesta");
        }

        public ViewResult Resumen()
        {
            return View(Session["cesta"] as Cesta);
        }

        public ViewResult Finalizar()
        {
            Cesta cesta = Session["cesta"] as Cesta;
            try
            {
                dc.enviarEmail(cesta);
            }
            catch
            {
                ViewBag.error = "No se han enviado los email correctamente";
            }

            try
            {
                foreach(Item item in cesta.listaItems)
                {
                    db.setPedido(cesta.cliente.SS, item.Empresa.IDEmpresa, item.IDItem);                                        
                }
                ViewBag.mensaje = "Sus Pedidos se han enviado correctamente a las empresas seleccionadas.";
            }
            catch
            {
                ViewBag.mensaje = "Ha habido un error mandando alguna de sus Pedidos, sentimos mucho este problema, nuestro equipo se pondra a trabajar en ello cuanto antes.";
            }
            return View(cesta.cliente);
        }
    }
}