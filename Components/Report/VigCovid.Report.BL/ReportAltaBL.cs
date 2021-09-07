using System;
using System.Collections.Generic;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;

namespace VigCovid.Report.BL
{
    public class ReportAltaBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ReporteAltaBE ObtenerDatosAlta(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;
            var fechaPrimerSeguiento = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.NroSeguimiento == 1 select A4).FirstOrDefault().Fecha;
            var diasCuarentena = (fechaHoy - fechaPrimerSeguiento).TotalDays.ToString();
            var rDias = Decimal.Parse(diasCuarentena.ToString());
            var dias = Decimal.Round(rDias).ToString();
            var fechaAltaMedica = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaAltaMedica" select AM).FirstOrDefault().Fecha;
            var fechaFormat = fechaAltaMedica.Value.ToString("dd/MM/yyyy");

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos 
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica,
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             FechaAltaMedica = fechaFormat,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord= D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }


        public ReporteAltaBE ObtenerDatosIngreso(int trabajadorId, int usuarioId)
        {
            //var fechaHoy = DateTime.Now;
            //var fechaPrimerSeguiento = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.NroSeguimiento == 1 select A4).FirstOrDefault().Fecha;
            //var diasCuarentena = (fechaHoy - fechaPrimerSeguiento).TotalDays.ToString();
            //var rDias = Decimal.Parse(diasCuarentena.ToString());
            //var dias = Decimal.Round(rDias).ToString();
            //var fechaAltaMedica = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaAltaMedica" select AM).FirstOrDefault().Fecha;
            //var fechaFormat = fechaAltaMedica.Value.ToString("dd/MM/yyyy");

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }


        //Obtener Datos de Contacto Directo - Notificación a BP - Creado por Saul Ramos Vega 28052021

        public ReporteAltaBE ObtenerDatosContactosDirectos(int trabajadorId, int usuarioId)
        {
            
            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId , b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where (A.Id == trabajadorId &&  B.i_ParameterId == 2 && C.i_ParameterId == 3) || (A.Id == trabajadorId && B.i_ParameterId == 4 && C.i_ParameterId == 1)
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }


        public ReporteAltaBE EnviarDocumentoDMex(int Id, int usuarioId, int trabajadorId)
        {


            var fechaHoy = DateTime.Now;


            //var FechaInicio = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId  select AM).OrderByDescending(t => t.FechaInicio).FirstOrDefault();

            var FechaInicio = (from AM in db.FechaImportante where AM.Id == Id select AM).OrderByDescending(x => x.FechaInicio).First().FechaInicio;

            var FechaFin = (from AM in db.FechaImportante where AM.Id == Id select AM).FirstOrDefault().FechaFin;
            var Diag1 = (from AM in db.FechaImportante where AM.Id == Id  select AM).FirstOrDefault().Diagnostico;
            var D1 = "";
            var D2 = "";

            var diasDescanso = (FechaFin - FechaInicio).Value.Days;

            var F1 = diasDescanso + 1;

            var FAC = FechaInicio.Value.ToString("dd/MM/yyyy");
            var FPA = FechaFin.Value.ToString("dd/MM/yyyy");



            if (Diag1 == "1")
            {
                D1 = "U07.1 COVID - 19, virus identificado";
                D2 = "(Caso confirmado con resultado positivo de la prueba)";
            }
            else if (Diag1 == "2")
            {

                D1 = "U07.2 COVID - 19, virus no identificado  ";
                D2 = "(Diagnosticado clínicamente y epidemiológicamente con COVID -19 /Caso probable de COVID -19/Caso sospechoso de COVID -19)";
            }
            else if (Diag1 == "3")
            {

                D1 = "J39.9 Enfermedad del tracto respiratorio superior, no especificada";
                D2 = "(Sintomatología respiratoria que podría tener relación con COVID19)";
            }
            else if (Diag1 == "4")
            {

                D1 = "Z20.828 Contacto y(sospecha de) exposición a otras enfermedades víricas transmisibles";
                D2 = "(Contacto directo de COVID-19)";
            }
            else if (Diag1 == "5")
            {

                D1 = "A09 Diarrea diarreico(a)(de presunto origen infeccioso)";
                D2= "(Sintomatología gastrointestinal que podría tener relación con COVID19)";
            }
            else if (Diag1 == "6")
            {

                D1 = "U12.9 Vacunas COVID-19 que causan efectos adversos en uso terapéutico, no especificado";
                D2 = "";
            }

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             Diagnostico = D1,
                             DetalleDiagnostico = D2,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();


            return query;
        }



        public ReporteAltaBE EnviarDocumentoDM1(int trabajadorId, int usuarioId, string Diagnostico)
        {


            var fechaHoy = DateTime.Now;


            //var FechaInicio = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId  select AM).OrderByDescending(t => t.FechaInicio).FirstOrDefault();

            var FechaInicio = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId select AM).OrderByDescending(x => x.FechaInicio).First().FechaInicio;



            var FechaFin = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId select AM).OrderByDescending(x => x.FechaFin).First().FechaFin;
            //var Diag1 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId select AM).OrderByDescending(x => x.Diagnostico).First().Diagnostico;
            var D1 = "";
            var D2 = "";
            var Diag1 = Diagnostico;

            var diasDescanso = (FechaFin - FechaInicio).Value.Days;

            //var diasDescanso = (FechaFin - FechaInicio).Days;
            var F1 = diasDescanso + 1;

            var FAC = FechaInicio.Value.ToString("dd/MM/yyyy");
            var FPA = FechaFin.Value.ToString("dd/MM/yyyy");

            //var FAC = FechaInicio.Date.ToString("dd/MM/yyyy");
            //var FPA = FechaFin.Date.ToString("dd/MM/yyyy");               


            if (Diag1 == "1")
            {
                D1 = "U07.1 COVID - 19, virus identificado";
                D2 = "(Caso confirmado con resultado positivo de la prueba)";
            }
            else if (Diag1 == "2")
            {

                D1 = "U07.2 COVID - 19, virus no identificado  ";
                D2 = "(Diagnosticado clínicamente y epidemiológicamente con COVID -19 /Caso probable de COVID -19/Caso sospechoso de COVID -19)";
            }
            else if (Diag1 == "3")
            {

                D1 = "J39.9 Enfermedad del tracto respiratorio superior, no especificada";
                D2 = "(Sintomatología respiratoria que podría tener relación con COVID19)";
            }
            else if (Diag1 == "4")
            {

                D1 = "Z20.828 Contacto y(sospecha de) exposición a otras enfermedades víricas transmisibles";
                D2 = "(Contacto directo de COVID-19)";
            }
            else if (Diag1 == "5")
            {

                D1 = "A09 Diarrea diarreico(a)(de presunto origen infeccioso)";
                D2 = "(Sintomatología gastrointestinal que podría tener relación con COVID19)";
            }

            else if (Diag1 == "6")
            {

                D1 = "U12.9 Vacunas COVID-19 que causan efectos adversos en uso terapéutico, no especificado";
                D2 = "";
            }

            var query = (from A in db.RegistroTrabajador
                             join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                             from B in B_join.DefaultIfEmpty()
                             join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                             from C in C_join.DefaultIfEmpty()
                             join D in db.Sede on A.SedeId equals D.Id
                             where A.Id == trabajadorId
                             select new ReporteAltaBE
                             {
                                 Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                                 Edad = A.Edad,
                                 Dni = A.Dni,
                                 Empresa = A.Empleadora,
                                 PuestoTrabajo = A.PuestoTrabajo,
                                 ModoIngreso = B.v_Value1,
                                 ViaIngreso = C.v_Value1,
                                 //FechaRegistro = A.FechaIngreso,
                                 Sexo = A.Sexo,
                                 //DiasCuarentena = dias,
                                 DatosDoctor = (from A2 in db.Usuario
                                                join B3 in db.Persona on A2.PersonaId equals B3.Id
                                                where A2.Id == A.MedicoVigilaId
                                                select new
                                                {
                                                    Nombres = B3.Nombres + " " + B3.Apellidos
                                                }).FirstOrDefault().Nombres,
                                 DoctorId = (from A2 in db.Usuario
                                             join B3 in db.Persona on A2.PersonaId equals B3.Id
                                             where A2.Id == A.MedicoVigilaId
                                             select new
                                             {
                                                 Id = A2.Id
                                             }).FirstOrDefault().Id,
                                 Colegiatura = (from A2 in db.Usuario
                                                join B3 in db.Persona on A2.PersonaId equals B3.Id
                                                where A2.Id == A.MedicoVigilaId
                                                select new
                                                {
                                                    CMP = B3.CMP
                                                }).FirstOrDefault().CMP,
                                 Examenes = (from A1 in db.Examen
                                             join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                             from B1 in B1_join.DefaultIfEmpty()
                                             join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                             from C1 in C1_join.DefaultIfEmpty()
                                             where A1.TrabajadorId == trabajadorId
                                             select new ExamenBE
                                             {
                                                 Fecha = A1.Fecha,
                                                 TipoPrueba = B1.v_Value1,
                                                 Resultado = C1.v_Value1
                                             }).ToList(),
                                 ComentarioAlta = A.ComentarioAlta,
                                 Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                                 CorreoSede = D.Correos,
                                 CorreosChampios = D.CorreosChampion,
                                 //FechaAltaMedica = fechaFormat,

                                 FechaAislaminetoCuarentena = FAC,
                                 FechaPosibleAlta = FPA,
                                 DiasTotalDescanso = F1,
                                 CorreosTrabajador = A.Email,
                                 Diagnostico = D1,
                                 DetalleDiagnostico = D2,
                                 CorreosBP = D.CorreosBP,
                                 CorreosPeople = D.CorreosPeople,
                                 CorreosSeguridad = D.CorreosSeguridadFisica,
                                 CorreosMedicoZona = D.CorreosMedico,
                                 CorreosMedicoCoord = D.CorreosCoordinador
                             }).FirstOrDefault();


                return query;
            }

       





        public ReporteAltaBE EnviarDocumentoDM2(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;

            var FechaPosibleAlta = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta" select AM).FirstOrDefault().Fecha;
            var FechaPosibleAlta2 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta2" select AM).FirstOrDefault().Fecha;

            var diasDescanso = (FechaPosibleAlta2 - FechaPosibleAlta).Value.Days;

            var F1 = diasDescanso;

            var FAC = FechaPosibleAlta.Value.ToString("dd/MM/yyyy");
            var FPA = FechaPosibleAlta2.Value.ToString("dd/MM/yyyy");


            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }

        public ReporteAltaBE EnviarDocumentoDM3(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;

            var FechaPosibleAlta2 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta2" select AM).FirstOrDefault().Fecha;
            var FechaPosibleAlta3 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta3" select AM).FirstOrDefault().Fecha;

            var diasDescanso = (FechaPosibleAlta3 - FechaPosibleAlta2).Value.Days;

            var F1 = diasDescanso;

            var FAC = FechaPosibleAlta2.Value.ToString("dd/MM/yyyy");
            var FPA = FechaPosibleAlta3.Value.ToString("dd/MM/yyyy");


            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }


        public ReporteAltaBE EnviarDocumentoDM4(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;

            var FechaPosibleAlta3 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta3" select AM).FirstOrDefault().Fecha;
            var FechaPosibleAlta4 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta4" select AM).FirstOrDefault().Fecha;

            var diasDescanso = (FechaPosibleAlta4 - FechaPosibleAlta3).Value.Days;

            var F1 = diasDescanso;

            var FAC = FechaPosibleAlta3.Value.ToString("dd/MM/yyyy");
            var FPA = FechaPosibleAlta4.Value.ToString("dd/MM/yyyy");


            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }

        public ReporteAltaBE EnviarDocumentoDM5(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;

            var FechaPosibleAlta4 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta4" select AM).FirstOrDefault().Fecha;
            var FechaPosibleAlta5 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta5" select AM).FirstOrDefault().Fecha;

            var diasDescanso = (FechaPosibleAlta5 - FechaPosibleAlta4).Value.Days;

            var F1 = diasDescanso;

            var FAC = FechaPosibleAlta4.Value.ToString("dd/MM/yyyy");
            var FPA = FechaPosibleAlta5.Value.ToString("dd/MM/yyyy");


            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }

        public ReporteAltaBE EnviarDocumentoDM6(int trabajadorId, int usuarioId)
        {
            var fechaHoy = DateTime.Now;

            var FechaPosibleAlta5 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta5" select AM).FirstOrDefault().Fecha;
            var FechaPosibleAlta6 = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaPosibleAlta6" select AM).FirstOrDefault().Fecha;

            var diasDescanso = (FechaPosibleAlta6 - FechaPosibleAlta5).Value.Days;

            var F1 = diasDescanso;

            var FAC = FechaPosibleAlta5.Value.ToString("dd/MM/yyyy");
            var FPA = FechaPosibleAlta6.Value.ToString("dd/MM/yyyy");


            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,

                             FechaAislaminetoCuarentena = FAC,
                             FechaPosibleAlta = FPA,
                             DiasTotalDescanso = F1,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosPeople = D.CorreosPeople,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }



        public ReporteAltaBE ObtenerDatosReceta(int trabajadorId, int usuarioId)
        {
            //var fechaHoy = DateTime.Now;
            //var fechaPrimerSeguiento = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.NroSeguimiento == 1 select A4).FirstOrDefault().Fecha;
            //var diasCuarentena = (fechaHoy - fechaPrimerSeguiento).TotalDays.ToString();
            //var rDias = Decimal.Parse(diasCuarentena.ToString());
            //var dias = Decimal.Round(rDias).ToString();
            //var fechaAltaMedica = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaAltaMedica" select AM).FirstOrDefault().Fecha;
            //var fechaFormat = fechaAltaMedica.Value.ToString("dd/MM/yyyy");

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,
                           
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador
                         }).FirstOrDefault();

            return query;
        }

        

             public ReporteAltaBE ObtenerDatosPruebaEmail(int trabajadorId, int usuarioId, int seguimientoId)
        {
            //var fechaHoy = DateTime.Now;
            //var fechaPrimerSeguiento = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.NroSeguimiento == 1 select A4).FirstOrDefault().Fecha;
            //var diasCuarentena = (fechaHoy - fechaPrimerSeguiento).TotalDays.ToString();
            //var rDias = Decimal.Parse(diasCuarentena.ToString());
            //var dias = Decimal.Round(rDias).ToString();
            //var fechaAltaMedica = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaAltaMedica" select AM).FirstOrDefault().Fecha;
            //var fechaFormat = fechaAltaMedica.Value.ToString("dd/MM/yyyy");


            //var FechaProgramada = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.Id == seguimientoId select A4).FirstOrDefault().FechaProgramada;
            //var FechaProgramadaP = FechaProgramada.Value.ToString("dd/MM/yyyy");

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador,
                             Direccion = A.Direccion,
                             Telefono = A.Celular,
                             //FechaProgramada = FechaProgramadaP,
                             CorreosTodaslasSedes = D.CorreosTodaslasSedes,
                             CorreosSedesProvincia = D.CorreosSedesProvincia,
                             CorreosSedesLima = D.CorreosSedesLima,
                             MedicoEncargado = D.MedicoEncargado,
                             NombreSede = D.NombreSede

                         }).FirstOrDefault();

            return query;
        }





        public ReporteAltaBE ObtenerDatosPrueba(int trabajadorId, int usuarioId, int seguimientoId)
        {
            //var fechaHoy = DateTime.Now;
            //var fechaPrimerSeguiento = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.NroSeguimiento == 1 select A4).FirstOrDefault().Fecha;
            //var diasCuarentena = (fechaHoy - fechaPrimerSeguiento).TotalDays.ToString();
            //var rDias = Decimal.Parse(diasCuarentena.ToString());
            //var dias = Decimal.Round(rDias).ToString();
            //var fechaAltaMedica = (from AM in db.FechaImportante where AM.TrabajadorId == trabajadorId && AM.Descripcion == "FechaAltaMedica" select AM).FirstOrDefault().Fecha;
            //var fechaFormat = fechaAltaMedica.Value.ToString("dd/MM/yyyy");


            var FechaProgramada = (from A4 in db.Seguimiento where A4.RegistroTrabajadorId == trabajadorId && A4.Id == seguimientoId select A4).FirstOrDefault().FechaProgramada;
            var FechaProgramadaP = FechaProgramada.Value.ToString("dd/MM/yyyy");

            var query = (from A in db.RegistroTrabajador
                         join B in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = B.i_ParameterId, b = B.i_GroupId } into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Sede on A.SedeId equals D.Id
                         where A.Id == trabajadorId
                         select new ReporteAltaBE
                         {
                             Trabajador = A.NombreCompleto + " " + A.ApePaterno + " " + A.ApeMaterno,
                             Edad = A.Edad,
                             Dni = A.Dni,
                             Empresa = A.Empleadora,
                             PuestoTrabajo = A.PuestoTrabajo,
                             ModoIngreso = B.v_Value1,
                             ViaIngreso = C.v_Value1,
                             //FechaRegistro = A.FechaIngreso,
                             Sexo = A.Sexo,
                             //DiasCuarentena = dias,
                             DatosDoctor = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                Nombres = B3.Nombres + " " + B3.Apellidos
                                            }).FirstOrDefault().Nombres,
                             DoctorId = (from A2 in db.Usuario
                                         join B3 in db.Persona on A2.PersonaId equals B3.Id
                                         where A2.Id == A.MedicoVigilaId
                                         select new
                                         {
                                             Id = A2.Id
                                         }).FirstOrDefault().Id,
                             Colegiatura = (from A2 in db.Usuario
                                            join B3 in db.Persona on A2.PersonaId equals B3.Id
                                            where A2.Id == A.MedicoVigilaId
                                            select new
                                            {
                                                CMP = B3.CMP
                                            }).FirstOrDefault().CMP,
                             Examenes = (from A1 in db.Examen
                                         join B1 in db.Parametro on new { a = A1.TipoPrueba, b = 103 } equals new { a = B1.i_ParameterId, b = B1.i_GroupId } into B1_join
                                         from B1 in B1_join.DefaultIfEmpty()
                                         join C1 in db.Parametro on new { a = A1.Resultado, b = 102 } equals new { a = C1.i_ParameterId, b = C1.i_GroupId } into C1_join
                                         from C1 in C1_join.DefaultIfEmpty()
                                         where A1.TrabajadorId == trabajadorId
                                         select new ExamenBE
                                         {
                                             Fecha = A1.Fecha,
                                             TipoPrueba = B1.v_Value1,
                                             Resultado = C1.v_Value1
                                         }).ToList(),
                             ComentarioAlta = A.ComentarioAlta,
                             Receta = A.Recetamedica, //SE AGREGA RECETA MEDICA PARA TABLA REGISTROTRABAJADOR
                             CorreoSede = D.Correos,
                             CorreosChampios = D.CorreosChampion,
                             //FechaAltaMedica = fechaFormat,
                             CorreosTrabajador = A.Email,
                             CorreosBP = D.CorreosBP,
                             CorreosSeguridad = D.CorreosSeguridadFisica,
                             CorreosMedicoZona = D.CorreosMedico,
                             CorreosMedicoCoord = D.CorreosCoordinador,
                             Direccion = A.Direccion, 
                             Telefono = A.Celular,
                             FechaProgramada = FechaProgramadaP,
                             CorreosTodaslasSedes = D.CorreosTodaslasSedes,
                             CorreosSedesProvincia = D.CorreosSedesProvincia,
                             CorreosSedesLima = D.CorreosSedesLima,
                             MedicoEncargado = D.MedicoEncargado,
                             NombreSede = D.NombreSede

                         }).FirstOrDefault();

            return query;
        }


        //ObtenerDatosPrueba


        public List<Parametro> ParametroCorreo()
        {
            return (from A in db.Parametro
                    where A.i_GroupId == 161
                    orderby A.i_ParameterId
                    select A).ToList();
        }

        public List<ReporteAcumuladoManualBE> ReporteAcumuladoManual(ReporteAcumuladoManualBE oReporteAcumuladoManualBE)
        {
            return null;
        }
    }
}