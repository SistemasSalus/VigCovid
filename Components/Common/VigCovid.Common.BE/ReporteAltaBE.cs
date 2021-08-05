using System;
using System.Collections.Generic;

namespace VigCovid.Common.BE
{
    public class ReporteAltaBE
    {
        public string Trabajador { get; set; }
        public int Edad { get; set; }
        public string Dni { get; set; }
        public string Empresa { get; set; }
        public string PuestoTrabajo { get; set; }
        public string ModoIngreso { get; set; }
        public string ViaIngreso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Sexo { get; set; }
        public string DiasCuarentena { get; set; }
        public string DatosDoctor { get; set; }
        public int DoctorId { get; set; }
        public string Colegiatura { get; set; }
        public List<ExamenBE> Examenes { get; set; }
        public string ComentarioAlta { get; set; }

        public string Receta { get; set; }

        public string CorreoSede { get; set; }
        public string FechaAltaMedica { get; set; }
        public string CorreosChampios { get; set; }
        public string CorreosTrabajador { get; set; }

        public string Diagnostico { get; set; }
        public string CorreosPeople { get; set; }
        public string CorreosBP { get; set; }
        public string CorreosSeguridad { get; set; }
        public string CorreosMedicoZona { get; set; }
        public string CorreosMedicoCoord { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string NombreSede { get; set; }

        public string CorreosTodaslasSedes { get; set; }

        public string CorreosSedesLima { get; set; }

        public string CorreosSedesProvincia { get; set; }

        public string MedicoEncargado { get; set; }

        public string FechaAislaminetoCuarentena { get; set; }
        public string FechaPosibleAlta { get; set; }

        public int DiasTotalDescanso { get; set; }


    }
}