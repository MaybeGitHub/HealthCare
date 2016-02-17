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
            Session["zona"] = "Zona Clientes";
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                Session["cliente"] = cliente.SS;
                Session["cesta"] = new List<string>();
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
            List<string> cesta = Session["cesta"] as List<string>;
            cesta.Add(id);
            Session["cesta"] = cesta;
            return View();
        }

        public ViewResult Cesta()
        {
            List<string> cesta = Session["cesta"] as List<string>;
            List<Items> items = new List<Items>();
            cesta.ForEach(x => items.Add(dc.Item(int.Parse(x))));
            return View(items);
        }

        public ViewResult Solicitud(string Items, string Empresas, int ss)
        {
            List<string> listaEmpresas = Empresas.Split(':').ToList();
            List<string> listaItems = Items.Split(':').ToList();

            Pre_Solicitud ps = new Pre_Solicitud();
            ps.cliente = db.getCliente(ss);
            foreach (string s in listaEmpresas) ps.listaEmpresas.Add(db.getEmpresa(s));
            foreach (string s in listaItems) ps.listaItems.Add(db.getItem(int.Parse(s)));

            return View(ps);
        }
    }
}