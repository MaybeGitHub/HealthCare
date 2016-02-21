using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HealthCare.Models;
using System.Configuration;
using System.Data.Linq;
using System.Globalization;

namespace HealthCare.Controllers
{
    public class BDController : Controller
    {
        private MiDBContextDataContext db { get; set; }

        public BDController()
        {
            db = new MiDBContextDataContext(ConfigurationManager.ConnectionStrings["miConexion"].ConnectionString);
        }

        public dynamic getCategorias()
        {
            return db.Categorias.ToList();
        }

        public Cliente getCliente(long SS)
        {            
            return db.Clientes.SingleOrDefault(x => x.SS == SS);                      
        }

        public Empresa getEmpresa(long iDEmpresa)
        {            
            return db.Empresas.SingleOrDefault(x => x.IDEmpresa == iDEmpresa);                      
        }

        public Empresa getEmpresa(string nombreEmpresa)
        {
            return db.Empresas.SingleOrDefault(x => x.Nombre == nombreEmpresa);
        }

        public void setEmpresa(Empresa empresa)
        {
            Empresa empresaExistente = db.Empresas.SingleOrDefault(x => x.IDEmpresa == empresa.IDEmpresa);
            if (empresaExistente == null)
            {
                empresa.IDEmpresa = getIDEmpresa();
                if (empresa.Descripcion == null) empresa.Descripcion = "";
                empresa.Valoracion = 4;
                db.Empresas.InsertOnSubmit(empresa);

                Categoria cat = db.Categorias.Single(x => x.Nombre == empresa.Categoria);
                cat.Empresas++;

                Subcategoria subcat = db.Subcategorias.Single(x => x.Nombre == empresa.Subcategoria);
                subcat.Empresas++;

                db.SubmitChanges();
            }
            else
            {
                Categoria cat = db.Categorias.Single(x => x.Nombre == empresaExistente.Categoria);
                cat.Empresas--;

                Subcategoria subcat = db.Subcategorias.Single(x => x.Nombre == empresaExistente.Subcategoria);
                subcat.Empresas--;

                db.SubmitChanges();

                cat = db.Categorias.Single(x => x.Nombre == empresa.Categoria);
                cat.Empresas++;

                subcat = db.Subcategorias.Single(x => x.Nombre == empresa.Subcategoria);
                subcat.Empresas++;

                empresaExistente.Nombre = empresa.Nombre;
                empresaExistente.Categoria = empresa.Categoria;
                empresaExistente.Subcategoria = empresa.Subcategoria;
                empresaExistente.Email = empresa.Email;
                empresaExistente.Telefono = empresa.Telefono;
                if (empresa.Descripcion != null)
                {
                    empresaExistente.Descripcion = empresa.Descripcion;
                }
                else
                {
                    empresaExistente.Descripcion = "";
                }
                empresaExistente.Valoracion = empresa.Valoracion;
                empresaExistente.Direccions.Calle = empresa.Direccions.Calle;
                empresaExistente.Direccions.Numero = empresa.Direccions.Numero;
                empresaExistente.Direccions.Piso = empresa.Direccions.Piso;
                empresaExistente.Direccions.Portal = empresa.Direccions.Portal;
                empresaExistente.Direccions.Puerta = empresa.Direccions.Puerta;
                empresaExistente.Direccions.Codigo_Postal = empresa.Direccions.Codigo_Postal;
                empresaExistente.Direccions.Detalles = empresa.Direccions.Detalles;

                db.SubmitChanges();
            }
                  
        }

        public void setCliente(Cliente cliente)
        {
            Cliente clienteExistente = db.Clientes.SingleOrDefault(x => x.SS == cliente.SS);
            if (clienteExistente == null){
                db.Clientes.InsertOnSubmit(cliente);
            }
            else
            {
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Apellidos = cliente.Apellidos;
                clienteExistente.Edad = cliente.Edad;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Telefono = cliente.Telefono;
                clienteExistente.Direccions.Calle = cliente.Direccions.Calle;
                clienteExistente.Direccions.Numero = cliente.Direccions.Numero;
                clienteExistente.Direccions.Piso = cliente.Direccions.Piso;
                clienteExistente.Direccions.Portal = cliente.Direccions.Portal;
                clienteExistente.Direccions.Puerta = cliente.Direccions.Puerta;
                clienteExistente.Direccions.Codigo_Postal = cliente.Direccions.Codigo_Postal;
                clienteExistente.Direccions.Detalles = cliente.Direccions.Detalles;
            } 
                     
            db.SubmitChanges();
        }

        private int getIDEmpresa()
        {
            int idObtenido = new Random().Next(1, Int32.MaxValue);
            while (db.Empresas.Where(x => x.IDEmpresa == idObtenido).Count() != 0)
            {
                idObtenido = new Random().Next(1, Int32.MaxValue);
            }
            return idObtenido;
        }

        public List<Empresa> getEmpresas(string subcategoria)
        {
            return db.Empresas.Where(x=> x.Subcategoria == subcategoria).ToList();
        }

        public Categoria getCategoria(string categoria)
        {
            return db.Categorias.SingleOrDefault(x => x.Nombre == categoria);
        }

        public List<Subcategoria> getSubcategorias(string categoria)
        {
            return db.Subcategorias.Where(x => x.Categoria == categoria).OrderBy(x => x.Nombre).ToList();
        }

        public bool setItem(Item item, long idEmpresa)
        {
            if(item.Precio > 0)
            {
                Item itemExistente = db.Items.Where(x => x.Nombre.ToLower() == item.Nombre.ToLower() && x.IDEmpresa == idEmpresa).SingleOrDefault();
                if (itemExistente == null)
                {
                    item.IDItem = db.Items.Count() + 1;
                    item.IDEmpresa = idEmpresa;
                    item.Precio = Math.Round(Convert.ToSingle(item.Precio, CultureInfo.InvariantCulture.NumberFormat), 2);
                    db.Items.InsertOnSubmit(item);
                    db.SubmitChanges();
                    return true;
                }
            }

            return false;
            
        }

        public Direccion getDireccion(Cliente cliente)
        {
            return db.Direccions.Single(x => x.IDDueño == cliente.SS);
        }

        public Direccion getDireccion(Empresa empresa)
        {
            return db.Direccions.Single(x => x.IDDueño == empresa.IDEmpresa);
        }

        public void setPedido(long ss, long IDEmpresa, long IDItem)
        {
            Pedido pedido = new Pedido();
            pedido.IDPedido = db.Pedidos.Count() + 1;
            pedido.IDCliente = ss;
            pedido.IDEmpresa = IDEmpresa;
            pedido.IDItem = IDItem;

            db.Pedidos.InsertOnSubmit(pedido);

            db.SubmitChanges();
        }

        public void cambiarEstadoPedido(long IDPedido, int estado)
        {
            Pedido pedido = db.Pedidos.Single(x => x.IDPedido == IDPedido);
            pedido.Estado = estado + 1;
            db.SubmitChanges();
        }

        public Pedido getPedido(long IDPedido)
        {
            return db.Pedidos.Single(x => x.IDPedido == IDPedido);
        }

        public void borrarPedido(long IDPedido)
        {
            Pedido pedido = getPedido(IDPedido);
            pedido.Oculto = true;            
            db.SubmitChanges();
        }

        public List<Item> getItems(long idEmpresa)
        {
            return db.Items.Where(x => x.IDEmpresa == idEmpresa).ToList();
        }

        public Item getItem(long IDItem)
        {
            return db.Items.SingleOrDefault(x => x.IDItem == IDItem);
        }

        public IEnumerable<Pedido> getPedidos(long ID, int est)
        {
            return db.Pedidos.Where(x => x.IDEmpresa == ID && x.Oculto == false && x.Estado == est);           
        }
    }
}