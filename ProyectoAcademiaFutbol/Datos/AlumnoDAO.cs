using ProyectoAcademiaFutbol.Clases;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;

namespace ProyectoAcademiaFutbol.Datos
{
    public static class AlumnoDAO
    {
        public static void CrearTablaSiNoExiste()
        {
            ConexionDB.VerificarCrearBD();
        }

        public static List<Alumno> ObtenerTodos()
        {
            var lista = new List<Alumno>();
            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                string sql = "SELECT * FROM Alumnos";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(MapearAlumno(reader));
                }
            }
            return lista;
        }

        public static bool ExistePorDNI(string dni)
        {
            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Alumnos WHERE Dni = @Dni";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Dni", dni);
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public static void Insertar(Alumno alumno)
        {
            ValidarAlumno(alumno);

            if (ExistePorDNI(alumno.DNI))
            {
                ActualizarPorDNI(alumno);
                return;
            }

            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                string sql = @"INSERT INTO Alumnos (
                    Nombre, Apellido, Dni, TipoSeguro, FechaNacimiento, Categoria, Compite,
                    Apoderado, Celular, Sede, Turno, Grupo, Equipo,
                    Mensualidad, FechaVencimiento, EstadoPago, FotoRuta)
                VALUES (
                    @Nombre, @Apellido, @Dni, @TipoSeguro, @FechaNacimiento, @Categoria, @Compite,
                    @Apoderado, @Celular, @Sede, @Turno, @Grupo, @Equipo,
                    @Mensualidad, @FechaVencimiento, @EstadoPago, @FotoRuta);";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    SetParametrosAlumno(cmd, alumno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ActualizarPorDNI(Alumno alumno)
        {
            ValidarAlumno(alumno);

            string sql = @"UPDATE Alumnos SET 
                Nombre = @Nombre,
                Apellido = @Apellido,
                TipoSeguro = @TipoSeguro,
                FechaNacimiento = @FechaNacimiento,
                Categoria = @Categoria,
                Compite = @Compite,
                Apoderado = @Apoderado,
                Celular = @Celular,
                Sede = @Sede,
                Turno = @Turno,
                Grupo = @Grupo,
                Equipo = @Equipo,
                Mensualidad = @Mensualidad,
                FechaVencimiento = @FechaVencimiento,
                EstadoPago = @EstadoPago,
                FotoRuta = @FotoRuta
                WHERE Dni = @Dni";

            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    SetParametrosAlumno(cmd, alumno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Actualizar(Alumno alumno)
        {
            ValidarAlumno(alumno);

            string sql = @"UPDATE Alumnos SET 
                Nombre = @Nombre,
                Apellido = @Apellido,
                Dni = @Dni,
                TipoSeguro = @TipoSeguro,
                FechaNacimiento = @FechaNacimiento,
                Categoria = @Categoria,
                Compite = @Compite,
                Apoderado = @Apoderado,
                Celular = @Celular,
                Sede = @Sede,
                Turno = @Turno,
                Grupo = @Grupo,
                Equipo = @Equipo,
                Mensualidad = @Mensualidad,
                FechaVencimiento = @FechaVencimiento,
                EstadoPago = @EstadoPago,
                FotoRuta = @FotoRuta
                WHERE Id = @Id";

            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    SetParametrosAlumno(cmd, alumno);
                    cmd.Parameters.AddWithValue("@Id", alumno.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Eliminar(int id)
        {
            string sql = "DELETE FROM Alumnos WHERE Id = @Id";
            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Alumno> BuscarPorNombreApellidoODNI(string texto)
        {
            var lista = new List<Alumno>();
            string sql = "SELECT * FROM Alumnos WHERE Nombre LIKE @filtro OR Apellido LIKE @filtro OR Dni LIKE @filtro";
            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@filtro", $"%{texto}%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearAlumno(reader));
                    }
                }
            }
            return lista;
        }

        public static Alumno ObtenerPorDNI(string dni)
        {
            using (var conn = new SQLiteConnection(ConexionDB.CadenaConexion))
            {
                conn.Open();
                string sql = "SELECT * FROM Alumnos WHERE Dni = @dni LIMIT 1;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapearAlumno(reader);
                    }
                }
            }
            return null;
        }

        public static bool ExisteDni(string dni, int idAlumnoActual)
        {
            using (var conn = ConexionDB.ObtenerConexion())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Alumnos WHERE Dni = @dni AND Id <> @id", conn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@id", idAlumnoActual);
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
        }

        // ================== MÉTODOS PRIVADOS ====================

        private static void ValidarAlumno(Alumno alumno)
        {
            // Validamos el DNI. Aseguramos que no sea nulo, esté en blanco y que la longitud sea 8.
            if (string.IsNullOrWhiteSpace(alumno.DNI) || alumno.DNI.Length != 8)
                throw new Exception("El DNI debe tener 8 dígitos.");

            // Validamos el celular.
            if (string.IsNullOrWhiteSpace(alumno.Celular) || !alumno.Celular.StartsWith("9") || alumno.Celular.Length != 9)
                throw new Exception("El celular debe comenzar con 9 y tener 9 dígitos.");

            // Normalizamos la sede y validamos.
            alumno.Sede = NormalizarSede(alumno.Sede);
            if (string.IsNullOrWhiteSpace(alumno.Sede) || (alumno.Sede != "Yecilú" && alumno.Sede != "Misti" && alumno.Sede != "El Bosque"))
                throw new Exception("Sede no válida. Debe ser Yecilú, Misti o El Bosque.");
        }

        private static void SetParametrosAlumno(SQLiteCommand cmd, Alumno alumno)
        {
            cmd.Parameters.AddWithValue("@Nombre", alumno.Nombre ?? "");
            cmd.Parameters.AddWithValue("@Apellido", alumno.Apellido ?? "");
            cmd.Parameters.AddWithValue("@Dni", alumno.DNI ?? "");
            cmd.Parameters.AddWithValue("@TipoSeguro", alumno.TipoSeguro ?? "");
            cmd.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Categoria", alumno.Categoria ?? "");
            cmd.Parameters.AddWithValue("@Compite", alumno.Compite ? 1 : 0);
            cmd.Parameters.AddWithValue("@Apoderado", alumno.Apoderado ?? "");
            cmd.Parameters.AddWithValue("@Celular", alumno.Celular ?? "");
            cmd.Parameters.AddWithValue("@Sede", alumno.Sede ?? "");
            cmd.Parameters.AddWithValue("@Turno", alumno.Turno ?? "");
            cmd.Parameters.AddWithValue("@Grupo", alumno.Grupo ?? "");
            cmd.Parameters.AddWithValue("@Equipo", alumno.Equipo ?? "");
            cmd.Parameters.AddWithValue("@Mensualidad", alumno.Mensualidad);
            cmd.Parameters.AddWithValue("@FechaVencimiento", alumno.FechaInicioMatricula.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@EstadoPago", alumno.EstadoPago ? 1 : 0);
            cmd.Parameters.AddWithValue("@FotoRuta", alumno.FotoRuta ?? "");
        }

        private static Alumno MapearAlumno(SQLiteDataReader reader)
        {
            return new Alumno
            {
                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                Nombre = reader["Nombre"]?.ToString() ?? "",
                Apellido = reader["Apellido"]?.ToString() ?? "",
                DNI = reader["Dni"]?.ToString() ?? "",
                TipoSeguro = reader["TipoSeguro"]?.ToString() ?? "",
                FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? DateTime.Parse(reader["FechaNacimiento"].ToString()) : DateTime.MinValue,
                Categoria = reader["Categoria"]?.ToString() ?? "",
                Compite = reader["Compite"] != DBNull.Value && Convert.ToInt32(reader["Compite"]) == 1,
                Apoderado = reader["Apoderado"]?.ToString() ?? "",
                Celular = reader["Celular"]?.ToString() ?? "",
                Sede = reader["Sede"]?.ToString() ?? "",
                Turno = reader["Turno"]?.ToString() ?? "",
                Grupo = reader["Grupo"]?.ToString() ?? "",
                Equipo = reader["Equipo"]?.ToString() ?? "",
                Mensualidad = reader["Mensualidad"] != DBNull.Value ? Convert.ToDecimal(reader["Mensualidad"]) : 0,
                FechaInicioMatricula = reader["FechaVencimiento"] != DBNull.Value ? DateTime.Parse(reader["FechaVencimiento"].ToString()) : DateTime.MinValue,
                EstadoPago = reader["EstadoPago"] != DBNull.Value && Convert.ToInt32(reader["EstadoPago"]) == 1,
                FotoRuta = reader["FotoRuta"]?.ToString() ?? ""
            };
        }

        private static string NormalizarSede(string sede)
        {
            if (string.IsNullOrWhiteSpace(sede))
                return "";

            string sedeLimpia = sede.Trim().ToLowerInvariant();
            switch (sedeLimpia)
            {
                case "yecilú":
                case "yecilu":
                case "yecilú ":
                case "yecilu ":
                case "yecilú.":
                case "yecilu.":
                case "yecilú,":
                case "yecilu,":
                    return "Yecilú";
                case "misti":
                case "misti ":
                    return "Misti";
                case "el bosque":
                case "elbosque":
                case "el bosque ":
                case "elbosque ":
                    return "El Bosque";
                default:
                    // Devolver nulo o el valor original si no coincide con ninguna sede conocida
                    // Se ha cambiado a null para que la validación en ValidarAlumno pueda detectarlo.
                    return null;
            }
        }
    }
}