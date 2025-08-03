using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ProyectoAcademiaFutbol.Clases;

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
                    {
                        lista.Add(MapearAlumno(reader));
                    }
                }
            }
            return lista;
        }

        public static void Insertar(Alumno alumno)
        {
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
                    cmd.Parameters.AddWithValue("@Nombre", alumno.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@Apellido", alumno.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@Dni", alumno.DNI ?? "");
                    cmd.Parameters.AddWithValue("@TipoSeguro", alumno.TipoDeSeguro ?? "");
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
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Actualizar(Alumno alumno)
        {
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
                    cmd.Parameters.AddWithValue("@Nombre", alumno.Nombre ?? "");
                    cmd.Parameters.AddWithValue("@Apellido", alumno.Apellido ?? "");
                    cmd.Parameters.AddWithValue("@Dni", alumno.DNI ?? "");
                    cmd.Parameters.AddWithValue("@TipoSeguro", alumno.TipoDeSeguro ?? "");
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
                        {
                            lista.Add(MapearAlumno(reader));
                        }
                    }
                }
            }
            return lista;
        }

        private static Alumno MapearAlumno(SQLiteDataReader reader)
        {
            return new Alumno
            {
                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : string.Empty,
                Apellido = reader["Apellido"] != DBNull.Value ? reader["Apellido"].ToString() : string.Empty,
                DNI = reader["Dni"] != DBNull.Value ? reader["Dni"].ToString() : string.Empty,
                TipoDeSeguro = reader["TipoSeguro"] != DBNull.Value ? reader["TipoSeguro"].ToString() : string.Empty,
                FechaNacimiento = reader["FechaNacimiento"] != DBNull.Value ? DateTime.Parse(reader["FechaNacimiento"].ToString()) : DateTime.MinValue,
                Categoria = reader["Categoria"] != DBNull.Value ? reader["Categoria"].ToString() : string.Empty,
                Compite = reader["Compite"] != DBNull.Value && Convert.ToInt32(reader["Compite"]) == 1,
                Apoderado = reader["Apoderado"] != DBNull.Value ? reader["Apoderado"].ToString() : string.Empty,
                Celular = reader["Celular"] != DBNull.Value ? reader["Celular"].ToString() : string.Empty,
                Sede = reader["Sede"] != DBNull.Value ? reader["Sede"].ToString() : string.Empty,
                Turno = reader["Turno"] != DBNull.Value ? reader["Turno"].ToString() : string.Empty,
                Grupo = reader["Grupo"] != DBNull.Value ? reader["Grupo"].ToString() : string.Empty,
                Equipo = reader["Equipo"] != DBNull.Value ? reader["Equipo"].ToString() : string.Empty,
                Mensualidad = reader["Mensualidad"] != DBNull.Value ? Convert.ToDecimal(reader["Mensualidad"]) : 0,
                FechaInicioMatricula = reader["FechaVencimiento"] != DBNull.Value ? DateTime.Parse(reader["FechaVencimiento"].ToString()) : DateTime.MinValue,
                EstadoPago = reader["EstadoPago"] != DBNull.Value && Convert.ToInt32(reader["EstadoPago"]) == 1,
                FotoRuta = reader["FotoRuta"] != DBNull.Value ? reader["FotoRuta"].ToString() : string.Empty
            };
        }
    }
}
