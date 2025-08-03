using ClosedXML.Excel;
using OfficeOpenXml;
using ProyectoAcademiaFutbol.Clases;
using ProyectoAcademiaFutbol.Datos;
using ProyectoAcademiaFutbol.Formularios;
using ProyectoAcademiaFutbol.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProyectoAcademiaFutbol
{
    public partial class FrmAlumnos : Form
    {
        public FrmAlumnos()
        {
            InitializeComponent();

            // Asignamos DataPropertyName a cada columna para poder usar DataSource
            colNombre.DataPropertyName = nameof(Alumno.Nombre);
            colApellido.DataPropertyName = nameof(Alumno.Apellido);
            colDNI.DataPropertyName = nameof(Alumno.DNI);
            colFechaNacimiento.DataPropertyName = nameof(Alumno.FechaNacimiento);
            colSede.DataPropertyName = nameof(Alumno.Sede);
            colTurno.DataPropertyName = nameof(Alumno.Turno);
            colGrupo.DataPropertyName = nameof(Alumno.Grupo);
            colApoderado.DataPropertyName = nameof(Alumno.Apoderado);
            colCelularApoderado.DataPropertyName = nameof(Alumno.Celular);
            colCompite.DataPropertyName = nameof(Alumno.Compite);
            colTipoSeguro.DataPropertyName = nameof(Alumno.TipoSeguro);
            colCategoria.DataPropertyName = nameof(Alumno.Categoria);
            colEquipo.DataPropertyName = nameof(Alumno.Equipo);
            colMensualidad.DataPropertyName = nameof(Alumno.Mensualidad);
            colFechaVencimiento.DataPropertyName = nameof(Alumno.FechaInicioMatricula);
            colEstadoPago.DataPropertyName = nameof(Alumno.EstadoPago);

            // Configuraciones adicionales
            dgvAlumnos.AutoGenerateColumns = false;
            dgvAlumnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlumnos.MultiSelect = false;

            dgvAlumnos.SelectionChanged += DgvAlumnos_SelectionChanged;
        }

        private void FrmAlumnos_Load(object sender, EventArgs e)
        {
            ConexionDB.VerificarCrearBD();
            CargarDatos();

            cbCategoria.Items.AddRange(new object[] { "Sub-6", "Sub-8", "Sub-10", "Sub-12", "Sub-14", "Sub-16" });
            cbSede.Items.AddRange(new object[] { "El Bosque", "Misti", "Yecilú" });
            cbEquipo.Items.AddRange(new object[] { "A", "B", "C" });
        }

        private void CargarDatos()
        {
            List<Alumno> lista = AlumnoDAO.ObtenerTodos();
            dgvAlumnos.DataSource = lista;
        }

        private void LimpiarFiltros()
        {
            cbSede.SelectedIndex = -1;
            cbCategoria.SelectedIndex = -1;
            cbEquipo.SelectedIndex = -1;
            nudEdadMin.Value = nudEdadMin.Minimum;
            nudEdadMax.Value = nudEdadMax.Maximum;
            txtBuscar.Text = "";
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            CargarDatos();
            LimpiarFiltros();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmAgregarAlumno())
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarDatos();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarAlumno();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BuscarAlumno()
        {
            string t = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(t))
                CargarDatos();
            else
                dgvAlumnos.DataSource = AlumnoDAO.BuscarPorNombreApellidoODNI(t);
        }

        private void DgvAlumnos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlumnos.CurrentRow == null) return;
            var a = (Alumno)dgvAlumnos.CurrentRow.DataBoundItem;

            // Foto
            pbFotoAlumno.ImageLocation = string.IsNullOrEmpty(a.FotoRuta) ? null : a.FotoRuta;

            // Vista previa en labels (ajusta los nombres de los labels según tu formulario)
            lblNombre.Text = a.NombreCompleto;
            lblDNI.Text = a.DNI;
            lblCategoria.Text = a.Categoria;
            lblSede.Text = a.Sede;
            lblTurno.Text = a.Turno;
            lblGrupo.Text = a.Grupo;
            lblApoderado.Text = a.Apoderado;
            lblCelular.Text = a.Celular;
            lblEdad.Text = a.Edad.ToString();
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            var list = AlumnoDAO.ObtenerTodos();
            if (cbCategoria.SelectedIndex != -1)
                list = list.FindAll(a => a.Categoria == cbCategoria.Text);
            if (cbSede.SelectedIndex != -1)
                list = list.FindAll(a => a.Sede == cbSede.Text);
            if (cbEquipo.SelectedIndex != -1)
                list = list.FindAll(a => a.Equipo == cbEquipo.Text);
            if (nudEdadMin.Value <= nudEdadMax.Value)
                list = list.FindAll(a => a.Edad >= nudEdadMin.Value && a.Edad <= nudEdadMax.Value);

            dgvAlumnos.DataSource = list;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.CurrentRow == null) return;

            var alumno = (Alumno)dgvAlumnos.CurrentRow.DataBoundItem;
            using (var frm = new FrmAgregarAlumno(alumno))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarDatos();
            }
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (dgvAlumnos.CurrentRow == null) return;
            var alumno = (Alumno)dgvAlumnos.CurrentRow.DataBoundItem;
            if (MessageBox.Show($"¿Eliminar a {alumno.Nombre} {alumno.Apellido}?", "Confirmar",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AlumnoDAO.Eliminar(alumno.Id);
                CargarDatos();
            }
        }

        private void btnExporrtar_Click_1(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
                sfd.Title = "Guardar como Excel";
                sfd.FileName = "Alumnos.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExcelHelper.ExportarAlumnosAExcel(dgvAlumnos, sfd.FileName);
                        MessageBox.Show("Exportado correctamente a Excel.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("No se puede exportar. El archivo está abierto en otro programa. Ciérralo e intenta de nuevo.",
                                         "Archivo en uso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error al exportar: " + ex.Message,
                                         "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImportar_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<string> erroresImportacion;

                    List<Alumno> alumnos = ExcelHelper.ImportarAlumnosDesdeExcel(ofd.FileName, out erroresImportacion);

                    int countNuevos = 0;
                    int countActualizados = 0;

                    foreach (Alumno alumno in alumnos)
                    {
                        if (AlumnoDAO.ExistePorDNI(alumno.DNI))
                        {
                            AlumnoDAO.ActualizarPorDNI(alumno);
                            countActualizados++;
                        }
                        else
                        {
                            AlumnoDAO.Insertar(alumno);
                            countNuevos++;
                        }
                    }

                    MessageBox.Show(
                        $"Importación completada.\nNuevos: {countNuevos}\nActualizados: {countActualizados}",
                        "Importación exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    if (erroresImportacion.Count > 0)
                    {
                        MessageBox.Show(
                            "Algunos alumnos no se importaron por errores:\n\n" + string.Join("\n", erroresImportacion),
                            "Importación con Errores", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    CargarDatos();
                }
                catch (IOException)
                {
                    MessageBox.Show("El archivo está abierto en otro programa. Ciérralo e intenta de nuevo.",
                        "Archivo en uso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (InvalidDataException idEx)
                {
                    MessageBox.Show(idEx.Message,
                        "Error en los datos del archivo de Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al importar: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}