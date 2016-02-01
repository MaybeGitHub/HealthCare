﻿using HealthCare.Models;
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
        public ViewResult Index(int SS)
        {
            Clientes c = db.getCliente(SS);
            if (ModelState.IsValid && c != null)
            {                                                               
                return View("Bienvenido", c);                              
            }
            return View();
        }

        public string obtenerEmpresas(string sector, string especializacion)
        {            
            string json = "[";
            foreach (Empresas e in db.getEmpresas(sector, especializacion))
            {
                json += "{\"Nombre\":\"" + e.Nombre + "\",\"Valoracion\":" + e.Valoracion  + ",\"IDEmpresa\":" + e.IDEmpresa + "},";
            }

            if(json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json;
        }

        public string obtenerPrescripciones(string ss)
        {
            string json = "[";            
            foreach (Prescripciones p in db.getPrescripciones(db.getCliente(int.Parse(ss))))
            {
                json += "{\"IDPrescripcion\":\"" + p.IDPrescripcion + "\"},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json;
        }

        public string obtenerItems(string id)
        {
            string json = "[";
            foreach (Items i in db.getItems(db.getPrescripcion(int.Parse(id))))
            {
                json += "{\"Nombre\":\"" + i.Nombre + "\",\"Tipo\":\"" + i.Tipo + "\",\"Detalles\":\"" + i.Detalles + "\"},";
            }

            if (json.Length > 1)
            {
                json = json.Substring(0, json.Length - 1);
            }

            json += "]";
            return json;
        }

        [HttpPost]
        public ViewResult solicitudServicio(int IDCliente, int IDEmpresa)
        {
            Solicitudes solicitud = new Solicitudes { IdCliente = IDCliente, idEmpresa = IDEmpresa, hora = DateTime.Now };
            solicitud.Clientes = db.getCliente(IDCliente);
            solicitud.Empresas = db.getEmpresa(IDEmpresa);
            return View("solicitudServicio", solicitud);
        }
    }
}