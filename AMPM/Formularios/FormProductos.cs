﻿using AMPM.clases;
using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMPM.Formularios
{
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
        }
        ClassProduct productos = new ClassProduct();
        public static void SoloLetras(KeyPressEventArgs v)
        {
            if (char.IsLetter(v.KeyChar))
            {

                v.Handled = false;
            }
            else if (char.IsSeparator(v.KeyChar))
            {
                v.Handled = false;
                MessageBox.Show("Espacios no permitidos");
            }
            else if (char.IsControl(v.KeyChar))
            {
                v.Handled = false;
            }
            else
            {

                v.Handled = true;
                MessageBox.Show("Digite solo letras porfavor");

            }
        }
        public void CargarDatos()
        {

            List<products> listarProductos = productos.Listar();
            var productosFiltrados = from p in listarProductos
                                     select new { p.code, p.nombre, p.stock, p.estado, p.proveedor, p.img };
            DgvProducts.DataSource = productosFiltrados.ToList();

        }
        private new products Update()
        {
            int id = int.Parse(txtBuscar.Text);
            products prod = productos.Buscar(id);
            if (id != 0)
                return prod;
            else
                return null;
        }



        public static void SoloNumeros(KeyPressEventArgs v)
        {

            if (char.IsDigit(v.KeyChar))
            {
                v.Handled = false;
            }
            else if (char.IsSeparator(v.KeyChar))
            {

                v.Handled = false;
            }
            else if (char.IsControl(v.KeyChar))
            {
                v.Handled = false;
            }
            else
            {

                v.Handled = true;
                MessageBox.Show("Digite solo números porfavor");
            }
        }

        private void txtExistencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            SoloNumeros(e);
        }

        private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private void btnImportar_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pbxPerfil.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            products prod = new products();
            prod.code = int.Parse(txtCodigo.Text);
            prod.nombre = txtNombre.Text;
            prod.stock = int.Parse(txtExistencia.Text);
            prod.estado = cmbEstado.SelectedItem.ToString();
            prod.proveedor = txtProveedor.Text;

            if (openFileDialog1.FileName != "")
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog1.FileName);
                prod.img = imageBytes;
            }

            if (productos.Guardar(prod) == true)
            {
                MessageBox.Show("Producto guardado con éxito");
                CargarDatos();
            }
            else
            {
                MessageBox.Show("Error al guardar el producto");
            }

        }


        private void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Update() != null)
                {
                    products product = Update();
                    product.nombre = txtNombre.Text;
                    product.stock = int.Parse(txtExistencia.Text);
                    product.estado = cmbEstado.SelectedItem.ToString();
                    product.proveedor = txtProveedor.Text;

                    // Agregar la imagen
                    if (pbxPerfil.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pbxPerfil.Image.Save(ms, ImageFormat.Jpeg);
                            product.img = ms.ToArray();
                        }
                    }

                    if (productos.Modificar(product) == true)
                    {
                        MessageBox.Show("modificado");
                        txtNombre.Text = "";
                        txtExistencia.Text = "";
                        cmbEstado.Items.Clear();
                        txtProveedor.Text = "";
                        pbxPerfil.Image = null; // limpiar la imagen
                        CargarDatos();
                    }
                    else
                    {
                        // mostrar mensaje de error
                    }
                }
            }
            catch
            {
                // manejar excepciones
            }
        }
    

        private void FormProductos_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                DataGridViewRow selectedRow = DgvProducts.Rows[rowIndex];
                byte[] imageBytes = (byte[])selectedRow.Cells["img"].Value;
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Image image = Image.FromStream(ms);
                    pbxPerfil.Image = image;
                }
                txtCodigo.Text = selectedRow.Cells["code"].Value.ToString();
                txtNombre.Text = selectedRow.Cells["nombre"].Value.ToString();
                txtExistencia.Text = selectedRow.Cells["stock"].Value.ToString();
                cmbEstado.SelectedItem = selectedRow.Cells["estado"].Value.ToString();
                txtProveedor.Text = selectedRow.Cells["proveedor"].Value.ToString();
            }
        }
     
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                products prod = new products();
                prod.code = Convert.ToInt32(txtBuscar.Text);

                int code = Convert.ToInt32(txtBuscar.Text);

                prod = productos.Buscar(code);
                if (productos.Eliminar(prod) == true)
                {
                    MessageBox.Show("borrado");
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show(" no borrado");
                }
               
            }
            catch
            {
              
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            products pr = productos.Buscar(int.Parse(txtBuscar.Text));
            if (pr != null)
            {
                //Mostrar información del producto
                txtCodigo.Text = pr.code.ToString();
                txtNombre.Text = pr.nombre;
                txtExistencia.Text = pr.stock.ToString();
                cmbEstado.Text = pr.estado;
                txtProveedor.Text = pr.proveedor;
            }
            else
            {
                MessageBox.Show("no encontrado");
            }

        }
          
        
    }
}
    
