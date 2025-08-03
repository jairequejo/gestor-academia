using ClosedXML.Excel;
using OfficeOpenXml;
using ProyectoAcademiaFutbol.Clases;
using ProyectoAcademiaFutbol.Datos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProyectoAcademiaFutbol.Utilidades
{
    public static class ExcelHelper
    {
        public static void ExportarAlumnosAExcel(DataGridView dgv, string rutaArchivo)
        {
            if (string.IsNullOrEmpty(rutaArchivo))
                throw new ArgumentException("La ruta del archivo no puede estar vacía.");

            if (EstaArchivoEnUso(rutaArchivo))
                throw new IOException("El archivo está en uso por otro proceso.");

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Datos");

                for (int i = 0; i < dgv.Columns.Count; i++)
                    ws.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;

                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        var valor = dgv.Rows[i].Cells[j].Value;
                        ws.Cell(i + 2, j + 1).Value = valor?.ToString() ?? string.Empty;
                    }
                }

                wb.SaveAs(rutaArchivo);
            }
        }

        public static List<Alumno> ImportarAlumnosDesdeExcel(string rutaArchivo, out List<string> errores)
        {
            List<Alumno> listaAlumnos = new List<Alumno>();
            errores = new List<string>();

            // SOLUCIÓN PARA EL ERROR DE LICENCIA DE EPPPLUS
            ExcelPackage.License.SetNonCommercialPersonal("Proyecto Academia Futbol");

            using (var package = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.End.Row;

                if (rowCount < 2)
                {
                    throw new InvalidDataException("El archivo Excel no contiene datos de alumnos.");
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        // Se obtiene el DNI de la tercera columna (índice 3)
                        var dniCell = worksheet.Cells[row, 3];

                        if (dniCell == null || dniCell.Value == null)
                            continue;

                        string dniText = dniCell.Text?.Trim() ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(dniText))
                        {
                            errores.Add($"Fila {row}: La celda del DNI está vacía.");
                            continue;
                        }

                        string dniLimpio = new string(dniText.Where(char.IsDigit).ToArray());

                        if (dniLimpio.Length != 8)
                        {
                            errores.Add($"Fila {row}: El DNI original '{dniText}' no es válido. Debe tener 8 dígitos. (La cadena limpia tiene una longitud de: {dniLimpio.Length})");
                            continue;
                        }

                        Alumno alumno = new Alumno
                        {
                            // Mapeo corregido de todas las columnas según tu imagen de Excel
                            Nombre = worksheet.Cells[row, 1]?.Text?.Trim() ?? string.Empty,
                            Apellido = worksheet.Cells[row, 2]?.Text?.Trim() ?? string.Empty,
                            DNI = dniLimpio,
                            FechaNacimiento = DateTime.TryParse(worksheet.Cells[row, 4]?.Text?.Trim(), out DateTime fnac) ? fnac : DateTime.MinValue,
                            Sede = NormalizarSede(worksheet.Cells[row, 5]?.Text?.Trim()),
                            Turno = worksheet.Cells[row, 6]?.Text?.Trim() ?? string.Empty,
                            Grupo = worksheet.Cells[row, 7]?.Text?.Trim() ?? string.Empty,
                            Apoderado = worksheet.Cells[row, 8]?.Text?.Trim() ?? string.Empty,
                            Celular = worksheet.Cells[row, 9]?.Text?.Trim() ?? string.Empty,
                            Compite = InterpretarBool(worksheet.Cells[row, 10]?.Text),
                            Categoria = worksheet.Cells[row, 11]?.Text?.Trim() ?? string.Empty,
                            Equipo = worksheet.Cells[row, 12]?.Text?.Trim() ?? string.Empty,
                            Mensualidad = decimal.TryParse(worksheet.Cells[row, 13]?.Text?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal m) ? m : 0,
                            FechaInicioMatricula = DateTime.TryParse(worksheet.Cells[row, 14]?.Text?.Trim(), out DateTime fv) ? fv : DateTime.MinValue,
                            EstadoPago = InterpretarBool(worksheet.Cells[row, 15]?.Text),
                            TipoSeguro = "", // No está presente en tu Excel, se deja vacío.
                            FotoRuta = ""
                        };

                        listaAlumnos.Add(alumno);
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Fila {row}: {ex.Message}");
                    }
                }
            }
            return listaAlumnos;
        }

        public static bool EstaArchivoEnUso(string rutaArchivo)
        {
            try
            {
                using (FileStream fs = File.Open(rutaArchivo, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        private static bool InterpretarBool(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            var lower = texto.Trim().ToLowerInvariant();
            return lower == "sí" || lower == "si" || lower == "true" || lower == "1" || lower == "pagado";
        }

        private static string NormalizarSede(string sede)
        {
            if (string.IsNullOrWhiteSpace(sede)) return null;

            string sinTildes = QuitarTildes(sede.ToLowerInvariant());

            return sinTildes.Contains("yecilu") ? "Yecilú"
                 : sinTildes.Contains("misti") ? "Misti"
                 : sinTildes.Contains("bosque") ? "El Bosque"
                 : null;
        }

        private static string QuitarTildes(string texto)
        {
            string normalized = texto.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray());
        }
    }
}