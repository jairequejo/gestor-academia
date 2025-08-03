using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProyectoAcademiaFutbol.Clases;
using ProyectoAcademiaFutbol.Datos;

namespace ProyectoAcademiaFutbol.Formularios
{
    public partial class FrmAgregarAlumno : Form
    {
        private string rutaFoto = string.Empty;
        private Alumno alumnoExistente = null;

        private List<string> sedesValidas = new List<string> { "Yecilú", "Misti", "El Bosque" };

        public FrmAgregarAlumno()
        {
            InitializeComponent();

            cbEquipo.Enabled = false;
            cbEquipo.Items.AddRange(new[] { "A", "B", "C" });
            cbCategoria.Items.AddRange(new[] { "Sub-6", "Sub-8", "Sub-10", "Sub-12", "Sub-14", "Sub-16" });
            cbSede.Items.AddRange(sedesValidas.ToArray());
            cbTipoSeguro.Items.AddRange(new[] { "SIS", "MINSA", "Privado", "Ninguno" });
            cbTurno.Items.AddRange(new[] { "Mañana", "Tarde", "Noche" });
            cbGrupo.Items.AddRange(new[] { "Grupo 1", "Grupo 2", "Grupo 3" });

            chkCompite.CheckedChanged += chkCompite_CheckedChanged;
        }

        public FrmAgregarAlumno(Alumno alumno) : this()
        {
            alumnoExistente = alumno;

            txtNombre.Text = alumno.Nombre;
            txtApellido.Text = alumno.Apellido;
            txtDNI.Text = alumno.DNI;
            dtpNacimiento.Value = alumno.FechaNacimiento;
            cbCategoria.SelectedItem = alumno.Categoria;
            chkCompite.Checked = alumno.Compite;
            cbEquipo.SelectedItem = string.IsNullOrEmpty(alumno.Equipo) ? null : alumno.Equipo;
            txtApoderado.Text = alumno.Apoderado;
            txtCelular.Text = alumno.Celular;
            cbSede.SelectedItem = alumno.Sede;
            cbTurno.SelectedItem = alumno.Turno;
            cbGrupo.SelectedItem = alumno.Grupo;
            cbTipoSeguro.SelectedItem = alumno.TipoSeguro;
            txtMensualidad.Text = alumno.Mensualidad.ToString("0.00");
            dtpInicioMatricula.Value = alumno.FechaInicioMatricula != DateTime.MinValue ? alumno.FechaInicioMatricula : DateTime.Today;
            rutaFoto = alumno.FotoRuta;
            pbFoto.ImageLocation = alumno.FotoRuta;

            this.Text = "Editar Alumno";
        }

        private void chkCompite_CheckedChanged(object sender, EventArgs e)
        {
            cbEquipo.Enabled = chkCompite.Checked;
            if (!chkCompite.Checked)
                cbEquipo.SelectedIndex = -1;
        }

        private void btnSeleccionarFoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos de imagen (*.jpg;*.png)|*.jpg;*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                rutaFoto = openFileDialog1.FileName;
                pbFoto.ImageLocation = rutaFoto;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            try
            {
                // Obtener datos del formulario
                Alumno datosFormulario = ObtenerDatosDesdeFormulario(alumnoExistente ?? new Alumno());

                // Validar si el DNI ya existe (exceptuando si es el mismo ID que se está editando)
                bool dniExiste = AlumnoDAO.ExisteDni(datosFormulario.DNI, alumnoExistente?.Id ?? 0);
                if (dniExiste)
                {
                    MessageBox.Show("Ya existe un alumno con este DNI.", "DNI duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (alumnoExistente != null)
                {
                    // Actualizar alumno existente
                    AlumnoDAO.Actualizar(datosFormulario);
                    MessageBox.Show("Alumno actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Insertar nuevo alumno
                    AlumnoDAO.Insertar(datosFormulario);
                    MessageBox.Show("Alumno agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Alumno ObtenerDatosDesdeFormulario(Alumno alumno)
        {
            alumno.Nombre = txtNombre.Text.Trim();
            alumno.Apellido = txtApellido.Text.Trim();
            alumno.DNI = txtDNI.Text.Trim();
            alumno.FechaNacimiento = dtpNacimiento.Value;
            alumno.Categoria = cbCategoria.Text.Trim();
            alumno.Sede = cbSede.Text.Trim();
            alumno.Equipo = cbEquipo.SelectedItem?.ToString();
            alumno.Grupo = cbGrupo.SelectedItem?.ToString();
            alumno.Turno = cbTurno.SelectedItem?.ToString();
            alumno.Compite = chkCompite.Checked;
            alumno.Apoderado = txtApoderado.Text.Trim();
            alumno.Celular = txtCelular.Text.Trim();
            alumno.Mensualidad = decimal.Parse(txtMensualidad.Text.Trim());
            alumno.FechaInicioMatricula = dtpInicioMatricula.Value;
            //alumno.EstadoPago = cbEstadoPago.SelectedItem?.ToString();
            alumno.TipoSeguro = cbTipoSeguro.SelectedItem?.ToString();

            return alumno;
        }


        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }
            if (cbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione una categoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbCategoria.Focus();
                return false;
            }
            if (chkCompite.Checked && cbEquipo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un equipo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbEquipo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApoderado.Text))
            {
                MessageBox.Show("El apoderado es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApoderado.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCelular.Text))
            {
                MessageBox.Show("El celular es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCelular.Focus();
                return false;
            }
            if (cbSede.SelectedIndex == -1 || !SedeEsValida(cbSede.Text))
            {
                MessageBox.Show("Seleccione una sede válida (Yecilú, Misti, El Bosque).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbSede.Focus();
                return false;
            }
            if (cbTurno.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un turno.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbTurno.Focus();
                return false;
            }
            if (cbGrupo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un grupo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbGrupo.Focus();
                return false;
            }
            if (cbTipoSeguro.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un tipo de seguro.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbTipoSeguro.Focus();
                return false;
            }
            if (!decimal.TryParse(txtMensualidad.Text, out decimal mensualidad) || mensualidad <= 0)
            {
                MessageBox.Show("Ingrese una mensualidad válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMensualidad.Focus();
                return false;
            }

            return true;
        }

        private bool SedeEsValida(string sedeIngresada)
        {
            string normalizada = NormalizarTexto(sedeIngresada);
            return sedesValidas.Any(s => NormalizarTexto(s) == normalizada);
        }

        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            string normalized = texto.ToLower().Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray()).Trim();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbFoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos de imagen (*.jpg;*.png)|*.jpg;*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                rutaFoto = openFileDialog1.FileName;
                pbFoto.ImageLocation = rutaFoto;
            }
        }

        private void cbSede_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {
        }
    }
}
