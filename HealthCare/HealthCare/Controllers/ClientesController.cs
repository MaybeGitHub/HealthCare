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

        public ViewResult Login(Clientes cliente)
        {
            Session.Clear();            
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                Session["zona"] = "Zona Clientes";
                Session["cliente"] = cliente.SS;
                Cesta cesta = new Cesta();
                cesta.cliente = c;
                Session["cesta"] = cesta;
                ViewBag.menu = "Inicio";
                return View("Categorias", dc.cargarCategorias());
            }
            else
            {
                ViewBag.menu = "Acceso";
                return View("Login");
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

        public ViewResult Registro(Clientes cliente)
        {
            Session.Remove("cliente");
            Session.Remove("cesta");
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                ViewBag.menu = "Acceso";
                db.setCliente(cliente);
                return View("Login");
            }
            else
            {
                ViewBag.menu = "Registro";
                return View();
            }            
        }        

        public ViewResult Subcategorias(string id)
        {
            Session["categoria"] = id;
            Session.Remove("subcategoria");
            Session.Remove("empresa");
            ViewBag.menu = "Categoria";
            return View(dc.Subcategorias(id));                    
        }

        public ViewResult Empresas(string id)
        {
            Session["subcategoria"] = id;
            Session.Remove("empresa");
            ViewBag.menu = "Subcategoria";
            return View(dc.Empresas(id));
        }

        public ViewResult Items(string id)
        {
            Empresas e = dc.Empresa(id);
            Session["empresa"] = e.Nombre;
            ViewBag.menu = "Empresa";
            return View(dc.Items(e.IDEmpresa));
        }

        public ViewResult Comprar(string id)
        {
            Cesta cesta = Session["cesta"] as Cesta;
            cesta.listaItems.Add(db.getItem(int.Parse(id)));
            Session["cesta"] = cesta;
            return View();
        } 

        
        public ViewResult Cesta()
        {
            Cesta cesta = Session["cesta"] as Cesta;
            cesta.listaItems.ForEach(x => cesta.listaEmpresas.Add(x.Empresas));
            cesta.listaEmpresas = cesta.listaEmpresas.GroupBy(x => x.Nombre).Select(x => x.First()).ToList();
            Session["cesta"] = cesta;
            return View(cesta);
        }
                
        public ViewResult Borrar(string posicion)
        {
            Cesta cesta = Session["cesta"] as Cesta;            
            cesta.listaItems.RemoveAt(int.Parse(posicion));
            cesta.listaEmpresas.RemoveAll(x => cesta.listaItems.Where(y => y.IDEmpresa == x.IDEmpresa).Count() == 0);            
            Session["cesta"] = cesta;
            return View("Cesta", cesta);
        }

        public ViewResult Resumen()
        {
            return View(Session["cesta"] as Cesta);
        }

        public ViewResult Finalizar()
        {
            Cesta cesta = Session["cesta"] as Cesta;
            try {
                dc.enviarEmail(cesta);
            }
            catch
            {
                ViewBag.error = "No se han enviado los email correctamente";
            }

            try
            {
                foreach(Items item in cesta.listaItems)
                {
                    db.setSolicitud(cesta.cliente.SS, item.Empresas.IDEmpresa, item.IDItem);                                        
                }
                ViewBag.mensaje = "Sus solicitudes se han enviado correctamente a las empresas seleccionadas.";
            }
            catch
            {
                ViewBag.mensaje = "Ha habido un error mandando alguna de sus solicitudes, sentimos mucho este problema, nuestro equipo se pondra a trabajar en ello cuanto antes.";
            }
            return View(cesta.cliente);
        }
    }
}