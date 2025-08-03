using System;
using System.Data.SQLite;
using System.IO;

namespace ProyectoAcademiaFutbol.Datos
{
    public static class ConexionDB
    {
        private static readonly string NombreDB = "academia.db";

        // Ruta absoluta al archivo de la base de datos
        private static readonly string RutaDb = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString() ?? AppDomain.CurrentDomain.BaseDirectory,
            NombreDB
        );

        // Ruta pública para acceder desde otras clases
        public static string RutaCompletaBD => RutaDb;

        // Cadena de conexión para SQLite
        public static string CadenaConexion =>
            $"Data Source={RutaDb};Version=3;";

        /// <summary>
        /// Crea la base de datos y la tabla de Alumnos si no existen.
        /// </summary>
        public static void VerificarCrearBD()
        {
            try
            {
                // Crea el archivo si no existe
                if (!File.Exists(RutaDb))
                {
                    SQLiteConnection.CreateFile(RutaDb);
                    Console.WriteLine("Base de datos creada en: " + RutaDb);
                }

                using (var conn = new SQLiteConnection(CadenaConexion))
                {
                    conn.Open();

                    string sql = @"
                    CREATE TABLE IF NOT EXISTS Alumnos (
                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Apellido TEXT,
                        Dni TEXT UNIQUE,
                        TipoSeguro TEXT,
                        FechaNacimiento TEXT NOT NULL,
                        Categoria TEXT NOT NULL,
                        Compite INTEGER NOT NULL,
                        Apoderado TEXT,
                        Celular TEXT,
                        Sede TEXT NOT NULL,
                        Turno TEXT,
                        Grupo TEXT,
                        Equipo TEXT,
                        Mensualidad REAL,
                        FechaVencimiento TEXT,
                        EstadoPago INTEGER,
                        FotoRuta TEXT
                    );";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar o crear la base de datos: " + ex.Message);
            }
        }
        public static SQLiteConnection ObtenerConexion()
        {
            return new SQLiteConnection(CadenaConexion);
        }
    }
}
