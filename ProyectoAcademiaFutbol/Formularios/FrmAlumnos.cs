using System;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using ProyectoAcademiaFutbol.Clases;
using ProyectoAcademiaFutbol.Datos;
using ProyectoAcademiaFutbol.Formularios;
using System.Text;
using ExcelDataReader;
using System.Data.OleDb;
using System.Linq;

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
            colTipoSeguro.DataPropertyName = nameof(Alumno.TipoDeSeguro);
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
                dgvAlumnos.DataSource = AlumnoDAO.BuscarPorNombreApellidoODNI(t); // ACTUALIZADO
        }

        private void DgvAlumnos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlumnos.CurrentRow == null) return;
            var a = (Alumno)dgvAlumnos.CurrentRow.DataBoundItem;

            // Foto
            pbFotoAlumno.ImageLocation = string.IsNullOrEmpty(a.FotoRuta) ? null : a.FotoRuta;

            // Vista previa en labels (ajusta los nombres de los labels según tu formulario)
            lblNombre.Text = a.NombreCompleto;
            lblDNI.Text = a.Apellido;
            lblDNI.Text = a.DNI;
            lblCategoria.Text = a.Categoria;
            lblSede.Text = a.Sede;
            lblTurno.Text = a.Turno;
            lblGrupo.Text = a.Grupo;
            lblApoderado.Text = a.Apoderado;
            lblCelular.Text = a.Celular;
            //lblTipoSeguro.Text = a.TipoDeSeguro;
            //lblCompite.Text = a.Compite ? "Sí" : "No";
            //lblEquipo.Text = a.Equipo;
            //lblMensualidad.Text = a.Mensualidad.ToString("C2");
            //lblFechaVencimiento.Text = a.FechaVencimiento.ToShortDateString();
            //lblEstadoPago.Text = a.EstadoPago ? "Pagado" : "Pendiente";
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

        private void btnEliminar_Click(object sender, EventArgs e)
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
        private void ExportarDataGridViewAExcel(DataGridView dgv, string rutaArchivo)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Datos");

                // Escribir encabezados
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    ws.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                }

                // Escribir filas
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        var celda = dgv.Rows[i].Cells[j].Value;
                        ws.Cell(i + 2, j + 1).Value = celda != null ? celda.ToString() : "";
                    }
                }

                wb.SaveAs(rutaArchivo);
                MessageBox.Show("Exportado correctamente a Excel.");
            }
        }

        private void btnExporrtar_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Archivo Excel|*.xlsx";
            guardar.Title = "Guardar como Excel";
            guardar.FileName = "DatosAcademia.xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                ExportarDataGridViewAExcel(dgvAlumnos, guardar.FileName);
            }
        }
        private List<Alumno> LeerAlumnosDesdeExcel(string ruta)
        {
            var alumnos = new List<Alumno>();

            using (var workbook = new XLWorkbook(ruta))
            {
                var hoja = workbook.Worksheet(1); // Puedes cambiar a nombre si es necesario
                var rows = hoja.RangeUsed().RowsUsed().Skip(1); // Saltamos cabecera

                foreach (var row in rows)
                {
                    try
                    {
                        var compiteTexto = row.Cell(10).GetValue<string>().Trim().ToLower();
                        bool compite = compiteTexto == "sí" || compiteTexto == "si" || compiteTexto == "1" || compiteTexto == "true";

                        var estadoPagoTexto = row.Cell(16).GetValue<string>().Trim().ToLower();
                        bool estadoPago = estadoPagoTexto == "sí" || estadoPagoTexto == "si" || estadoPagoTexto == "pagado" || estadoPagoTexto == "1" || estadoPagoTexto == "true";

                        var alumno = new Alumno
                        {
                            Nombre = row.Cell(1).GetValue<string>(),
                            Apellido = row.Cell(2).GetValue<string>(),
                            DNI = row.Cell(3).GetValue<string>(),
                            FechaNacimiento = row.Cell(4).TryGetValue(out DateTime fechaNac) ? fechaNac : DateTime.Today,
                            Sede = row.Cell(5).GetValue<string>(),
                            Turno = row.Cell(6).GetValue<string>(),
                            Grupo = row.Cell(7).GetValue<string>(),
                            Apoderado = row.Cell(8).GetValue<string>(),
                            Celular = row.Cell(9).GetValue<string>(),
                            Compite = compite,
                            TipoDeSeguro = row.Cell(11).GetValue<string>(),
                            Categoria = row.Cell(12).GetValue<string>(),
                            Equipo = row.Cell(13).GetValue<string>(),
                            Mensualidad = row.Cell(14).TryGetValue(out decimal mensualidad) ? mensualidad : 0,
                            FechaInicioMatricula = row.Cell(15).TryGetValue(out DateTime fechaMat) ? fechaMat : DateTime.Today,
                            EstadoPago = estadoPago,
                            FotoRuta = "" // Puedes ajustar si usas imágenes
                        };

                        alumnos.Add(alumno);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error en una fila: " + ex.Message, "Error al importar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue; // Saltar fila problemática
                    }
                }
            }

            return alumnos;
        }


        private void btnImportar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var alumnos = LeerAlumnosDesdeExcel(ofd.FileName);

                    int count = 0;
                    foreach (var alumno in alumnos)
                    {
                        AlumnoDAO.Insertar(alumno);
                        count++;
                    }

                    MessageBox.Show($"Se importaron {count} alumnos correctamente.", "Importación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al importar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
