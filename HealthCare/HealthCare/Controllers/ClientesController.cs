﻿using HealthCare.Models;
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

        public ViewResult Resumen()
        {
            return View(Session["cesta"] as Cesta);
        }

        public ViewResult Finalizar()
        {            
            try {
                dc.enviarEmail(Session["cesta"] as Cesta);
            }
            catch
            {
                ViewBag.error = "No se han enviado los email correctamente";
            }
            return View();
        }
    }
}