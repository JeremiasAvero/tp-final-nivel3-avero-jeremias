﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dominio;
using negocio;
using Negocio;

namespace Catalogo
{
    public partial class AltaArticulos : System.Web.UI.Page
    {
        public bool Admin { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

            Admin = false;
            try
            {
                UsuarioNegocio usuarioNegocio = new UsuarioNegocio();
                Seguridad seguridad = new Seguridad();
                Usuario usuario = (Usuario)Session["usuario"];

                if (!seguridad.EsAdmin(usuario))
                {
                    txtCodigo.ReadOnly = true;
                    txtDescripcion.ReadOnly = true;
                    txtImagenUrl.ReadOnly = true;
                    txtNombre.ReadOnly = true;
                    txtPrecio.ReadOnly = true;
                    ddlCategoria.Enabled = false;
                    ddlMarca.Enabled = false;

                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;

                }
                else
                {
                    Admin = true;
                }

                if (!IsPostBack)
                {
                    MarcaNegocio negocioMarca = new MarcaNegocio();
                    List<Marca> listaMarca = negocioMarca.listar();

                    ddlMarca.DataSource = listaMarca;
                    ddlMarca.DataValueField = "Id";
                    ddlMarca.DataTextField = "Descripcion";
                    ddlMarca.DataBind();

                    CategoriaNegocio negocioCategoria = new CategoriaNegocio();
                    List<Categoria> listaCategoria = negocioCategoria.listar();


                    ddlCategoria.DataSource = listaCategoria;
                    ddlCategoria.DataValueField = "Id";
                    ddlCategoria.DataTextField = "Descripcion";
                    ddlCategoria.DataBind();
                }

                // Si estamos modificando
                string id = Request.QueryString["id"] != null ? Request.QueryString["id"].ToString() : "";
                if (id != "" && !IsPostBack)
                {
                    btnAgregar.Text = "Modificar";
                    ArticuloNegocio negocio = new ArticuloNegocio();
                    // List<Articulo> lista = negocio.listar(id);
                    //Articulo articulo = lista[0];
                    Articulo articulo = (negocio.listar(id))[0];
                    txtId.Text = articulo.Id.ToString();
                    txtCodigo.Text = articulo.CodigoArticulo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtImagenUrl.Text = articulo.ImagenUrl;
                    txtPrecio.Text = articulo.Precio.ToString("0");

                    ddlCategoria.SelectedValue = articulo.Categoria.Id.ToString();
                    ddlMarca.SelectedValue = articulo.Categoria.Id.ToString();
                    txtImagenUrl_TextChanged(sender, e);

                }
            }
            catch (Exception ex)
            {
                Session.Add("error", ex);
                Response.Redirect("Error.aspx");
            }

        }

        protected void txtImagenUrl_TextChanged(object sender, EventArgs e)
        {
            imgImagenUrl.ImageUrl = txtImagenUrl.Text;
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                if (!Page.IsValid)
                { return; }

                Articulo articulo = new Articulo();
                ArticuloNegocio negocio = new ArticuloNegocio();

                articulo.CodigoArticulo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.ImagenUrl = txtImagenUrl.Text;

                articulo.Marca = new Marca();
                articulo.Marca.Id = int.Parse(ddlMarca.SelectedValue);

                articulo.Categoria = new Categoria();
                articulo.Categoria.Id = int.Parse(ddlCategoria.SelectedValue);

                if (Request.QueryString["id"] != null)
                {
                    articulo.Id = int.Parse(Request.QueryString["id"]);
                    negocio.Modificar(articulo);
                }

                else
                    negocio.Agregar(articulo);

                Response.Redirect("ListaArticulos.aspx", false);
            }
            catch (Exception ex)
            {
                Session.Add("error", ex);
                Response.Redirect("Error.aspx");
            }

        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            { return; }

            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();

                negocio.Borrar(int.Parse(txtId.Text));
                Response.Redirect("ListaArticulos.aspx", false);

            }
            catch (Exception ex)
            {
                Session.Add("error", ex);
                Response.Redirect("Error.aspx");
            }
        }
    }
}