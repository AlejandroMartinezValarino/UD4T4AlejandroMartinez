using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UD4T4AlejandroMartinez.MVVM.Models
{
    public class Alumno
    {
        public string Id { get; set; }
        public required string Nombre { get; set; }
        public required string Contraseña { get; set; }
        public required string CentroDocente { get; set; }
        public required string ProfesorSeguimiento { get; set; }
        public required string CentroTrabajo { get; set; }
        public required string TutorTrabajo { get; set; }
        public required string CicloFormativo { get; set; }
        public required string Grado { get; set; }
        public required string FotoPath { get; set; }
        public List<Semana> Semanas { get; set; }
    }
}
