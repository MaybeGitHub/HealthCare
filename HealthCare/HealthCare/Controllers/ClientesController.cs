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

        [HttpGet]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(Clientes cliente)
        {
            Clientes c = db.getCliente(cliente.SS);
            if (ModelState.IsValid && c != null)
            {
                return View("Bienvenido", c);
            }
            return View();
        }

        public ViewResult Bienvenido(int SS)
        {
            return View(db.getCliente(SS));
        }

        public string obtenerEmpresas(string sector, string especializacion)
        {
            string json = "[";
            foreach (Empresas e in db.getEmpresas(sector, especializacion))
            {
                json += "{\"Nombre\":\"" + e.Nombre + "\",\"Valoracion\":" + e.Valoracion + ",\"IDEmpresa\":" + e.IDEmpresa + "},";
            }

            if (json.Length > 1)
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
        public ViewResult solicitudServicio(int IDCliente, int IDEmpresa, bool prescripcion)
        {
            Solicitudes solicitud = new Solicitudes { IDCliente = IDCliente, IDEmpresa = IDEmpresa, Hora = DateTime.Now.TimeOfDay };
            solicitud.Clientes = db.getCliente(IDCliente);
            solicitud.Empresas = db.getEmpresa(IDEmpresa);
            if (prescripcion)
            {
                solicitud.Prescripciones.AddRange(db.getPrescripciones(db.getCliente(IDCliente)));
            }
            return View(solicitud);
        }
    }
}