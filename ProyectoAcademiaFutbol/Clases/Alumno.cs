using System;

namespace ProyectoAcademiaFutbol.Clases
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string NombreCompleto
        {
            get { return $"{Nombre} {Apellido}"; }
        }
        public string DNI { get; set; }
        public string TipoSeguro { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Categoria { get; set; }              // Categoría (ej: Sub-10, Sub-12)
        public bool Compite { get; set; }                  // ¿Compite en torneos?
        public string Apoderado { get; set; }              // Nombre del apoderado
        public string Celular { get; set; }                // Celular del apoderado
        public string Sede { get; set; }                   // Sede a la que pertenece
        public string Turno { get; set; }                  // Turno (mañana, tarde)
        public string Grupo { get; set; }                  // Grupo al que pertenece (ej: A, B, C)
        public string Equipo { get; set; }                 // Equipo A, B o C
        public decimal Mensualidad { get; set; }           // Monto mensual a pagar
        public DateTime FechaInicioMatricula { get; set; }     // Fecha de vencimiento del pago
        public bool EstadoPago { get; set; }               // ¿Pagó o no?
        public string FotoRuta { get; set; }               // Ruta del archivo de imagen

        // Propiedad calculada para obtener la edad
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }

        // Constructor vacío
        public Alumno() { }

        // Constructor completo
        public Alumno(int id, string nombre, string apellido, string dni, string tipoSeguro,
                      DateTime fechaNacimiento, string categoria, bool compite,
                      string apoderado, string celular, string sede, string turno,
                      string grupo, string equipo, decimal mensualidad,
                      DateTime fechaVencimiento, bool estadoPago, string fotoRuta)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            DNI = dni;
            TipoSeguro = tipoSeguro;
            FechaNacimiento = fechaNacimiento;
            Categoria = categoria;
            Compite = compite;
            Apoderado = apoderado;
            Celular = celular;
            Sede = sede;
            Turno = turno;
            Grupo = grupo;
            Equipo = equipo;
            Mensualidad = mensualidad;
            FechaInicioMatricula = fechaVencimiento;
            EstadoPago = estadoPago;
            FotoRuta = fotoRuta;
        }
    }
}
