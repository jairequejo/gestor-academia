using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;

namespace ProyectoAcademiaFutbol.Datos
{
    public static class ConexionDB
    {
        private static readonly string NombreDB = "academia.db";

        private static readonly string RutaDb = Path.Combine(
            AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString() ?? AppDomain.CurrentDomain.BaseDirectory,
            NombreDB
        );

        public static string RutaCompletaBD => RutaDb;

        public static string CadenaConexion =>
            $"Data Source={RutaDb};Version=3;";

        /// <summary>
        /// Verifica si existe la base de datos y la crea con su tabla principal si no existe.
        /// </summary>
        public static void VerificarCrearBD()
        {
            try
            {
                if (!File.Exists(RutaDb))
                {
                    SQLiteConnection.CreateFile(RutaDb);
                    Console.WriteLine("Base de datos creada: " + RutaDb);
                }

                using (var conn = new SQLiteConnection(CadenaConexion))
                {
                    conn.Open();

                    string sql = @"
                    CREATE TABLE IF NOT EXISTS Alumnos (
                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Apellido TEXT,
                        Dni TEXT,
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
    }
}
