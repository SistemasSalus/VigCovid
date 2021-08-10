using System.Collections.Generic;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;

namespace VigCovid.MedicalMonitoring.BL
{
    public class LineaTiempoBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<LineaTiempo> GetAllFechasLineaTiempo(int trabajadorId)
        {
            var fechas = new List<LineaTiempo>();
            var InicioSeguimiento = (from A in db.Seguimiento where A.NroSeguimiento == 1 && A.RegistroTrabajadorId == trabajadorId select A).FirstOrDefault();
            if (InicioSeguimiento == null) return new List<LineaTiempo>();

            #region Fecha incio seguimiento

            var oInicioSeguimiento = new LineaTiempo();
            oInicioSeguimiento.Fecha = InicioSeguimiento.Fecha;
            oInicioSeguimiento.DateLine = InicioSeguimiento.Fecha.ToString("dd/MMM") + "(PS)";
            oInicioSeguimiento.DateDate = InicioSeguimiento.Fecha.ToString("dd/MM/yyyy");

            fechas.Add(oInicioSeguimiento);

            #endregion Fecha incio seguimiento

            #region Fecha Exámenes

            var pruebaRapida = (from A in db.Examen where A.TrabajadorId == trabajadorId && A.TipoPrueba == 1 select A).FirstOrDefault();

            if (pruebaRapida != null)
            {
                var oPruebaRapida = new LineaTiempo();
                oPruebaRapida.Fecha = pruebaRapida.Fecha;
                oPruebaRapida.DateLine = pruebaRapida.Fecha.ToString("dd/MMM") + ObtenerResultadoPR(pruebaRapida.Resultado);
                oPruebaRapida.DateDate = pruebaRapida.Fecha.ToString("dd/MM/yyyy");
                fechas.Add(oPruebaRapida);
            }

            var pruebaMolecular = (from A in db.Examen where A.TrabajadorId == trabajadorId && A.TipoPrueba == 2 select A).FirstOrDefault();

            if (pruebaMolecular != null)
            {
                var oPruebaMolecular = new LineaTiempo();
                oPruebaMolecular.Fecha = pruebaMolecular.Fecha;
                oPruebaMolecular.DateLine = pruebaMolecular.Fecha.ToString("dd/MMM") + ObtenerResultadoMolecular(pruebaMolecular.Resultado);
                oPruebaMolecular.DateDate = pruebaMolecular.Fecha.ToString("dd/MM/yyyy");
                fechas.Add(oPruebaMolecular);
            }

            var fechasImportantes = (from A in db.FechaImportante where A.TrabajadorId == trabajadorId select A).ToList();

            var fechaInicioSintomas = fechasImportantes.Find(p => p.Descripcion == "FechaFinSintomas");
            if (fechaInicioSintomas != null)
            {
                var oFechaInicioSintomas = new LineaTiempo();
                oFechaInicioSintomas.Fecha = fechaInicioSintomas.Fecha.Value;
                oFechaInicioSintomas.DateLine = fechaInicioSintomas.Fecha.Value.ToString("dd/MMM") + "(Sx)";
                oFechaInicioSintomas.DateDate = fechaInicioSintomas.Fecha.Value.ToString("dd/MM/yyyy");
                fechas.Add(oFechaInicioSintomas);
            }

            var fechaAlta = fechasImportantes.Find(p => p.Descripcion == "FechaPosibleAlta");
            if (fechaAlta != null)
            {
                var oFechaAlta = new LineaTiempo();
                oFechaAlta.Fecha = fechaAlta.Fecha.Value;
                oFechaAlta.DateLine = fechaAlta.Fecha.Value.ToString("dd/MMM") + "(AP)";
                oFechaAlta.DateDate = fechaAlta.Fecha.Value.ToString("dd/MM/yyyy");
                fechas.Add(oFechaAlta);
            }

            var fechaCuarentena = fechasImportantes.Find(p => p.Descripcion == "FechaAislaminetoCuarentena");
            if (fechaCuarentena != null)
            {
                var oFechaCuarentena = new LineaTiempo();
                oFechaCuarentena.Fecha = fechaCuarentena.Fecha.Value;
                oFechaCuarentena.DateLine = fechaCuarentena.Fecha.Value.ToString("dd/MMM") + "(IAC)";
                oFechaCuarentena.DateDate = fechaCuarentena.Fecha.Value.ToString("dd/MM/yyyy");
                fechas.Add(oFechaCuarentena);
            }

            #endregion Fecha Exámenes

            fechas.Sort((x, y) => x.Fecha.CompareTo(y.Fecha));

            var result = fechas.GroupBy(g => g.Fecha)
                .Select(s => new LineaTiempo
                {
                    Fecha = s.Key,
                    DateLine = s.Select(ss => ss.DateLine).FirstOrDefault(),
                    DateDate = s.Select(ss => ss.DateDate).FirstOrDefault()
                }).ToList();
            return result;
        }


        public List<LineaTiempo> GetAllTipoRango(int trabajadorId)
        {


            var fechasImportantes = (from A in db.FechaImportante where A.TrabajadorId == trabajadorId && A.TipoRango >= 1 select A).ToList();
            var fechas = new List<LineaTiempo>();
            //var fechaInicioSintomas = fechasImportantes.Find(p => p.TipoRango >= 1); 

            foreach (var item in fechasImportantes)
            {
                var oFechaInicioSintomas = new LineaTiempo();



                oFechaInicioSintomas.FechaInicio = item.FechaInicio.Value;
                oFechaInicioSintomas.FechaFin = item.FechaFin;



                //== DbNull.Value ? (DateTime?)null : item.FechaFin.Value;



                //oFechaInicioSintomas.FechaFin = Nullable<item.FechaFin>;
                oFechaInicioSintomas.Descripcion = item.Descripcion;
                oFechaInicioSintomas.TipoRango = item.TipoRango;
                oFechaInicioSintomas.NroDias = item.NroDias;

                fechas.Add(oFechaInicioSintomas);

                  
                //fechas.Sort((x, y) => x.Fecha.CompareTo(y.Fecha));
                //var result = fechas.GroupBy(g => g.Fecha)
                //    .Select(s => new LineaTiempo
                //    {
                //       Fecha = s.Key,
                //        DateLine = s.Select(ss => ss.DateLine).FirstOrDefault(),
                //        DateDate = s.Select(ss => ss.DateDate).FirstOrDefault()
                //  }).ToList();
            }
            return fechas;
            
        }





















    private string ObtenerResultadoPR(int resultado)
        {
            if (resultado == 0)
            {
                return "(PR-)";
            }
            else if (resultado == 1)
            {
                return "(NV)";
            }
            else if (resultado == 2)
            {
                return "(PR+)";
            }
            else if (resultado == 3)
            {
                return "(PR+)";
            }
            else if (resultado == 4)
            {
                return "(PR+)";
            }
            else
            {
                return "";
            }
        }

        private string ObtenerResultadoMolecular(int resultado)
        {
            if (resultado == 1)
            {
                return "(PM+)";
            }
            else if (resultado == 2)
            {
                return "(PM-)";
            }
            else
            {
                return "(PM)";
            }
        }
    }
}