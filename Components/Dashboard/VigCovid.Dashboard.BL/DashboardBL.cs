using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Common.Resource;
using VigCovid.Worker.BL;
using static VigCovid.Common.Resource.Enums;

namespace VigCovid.Dashboard.BL
{
    public class DashboardBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<GraficoCasosDiariosBE> CasosDiarios(DateTime fi, DateTime ff, List<int> sedesId)
        {
            var query = (from A in db.RegistroTrabajador where A.FechaIngreso >= fi && A.FechaIngreso <= ff && sedesId.Contains(A.SedeId) select A).OrderBy(o => o.FechaIngreso).ToList();

            var result = new List<GraficoCasosDiariosBE>();

            var diasQuery = query.GroupBy(g => g.FechaIngreso).Select(s => s.First());

            foreach (var item in diasQuery)
            {
                var reg = new GraficoCasosDiariosBE();
                reg.dia = item.FechaIngreso.ToString("dd MMMM");
                reg.CasosDiariosAsintomaticos = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.AsintomaticoPositivo && p.FechaIngreso == item.FechaIngreso).Count;
                reg.CasosDiariosConfirmadoSintomatico = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.CovidConfirmadoSintomatico && p.FechaIngreso == item.FechaIngreso).Count;
                reg.CasosDiariosSospechosos = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.Sospechoso && p.FechaIngreso == item.FechaIngreso).Count;
                result.Add(reg);
            }

            return result;
        }

        public List<GraficoAltasBE> Altas(DateTime fi, DateTime ff, List<int> sedesId)
        {
            var result = new List<GraficoAltasBE>();

            var query = (from A in db.FechaImportante
                         join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                         where
                         (A.Fecha >= fi && A.Fecha <= ff)
                         && sedesId.Contains(B.SedeId)
                         && A.Fecha != null
                         && (A.Descripcion.Contains("Alta"))
                         select A)
                        .OrderBy(o => o.Fecha)
                        .ToList();
            //query = query.GroupBy(g => g.TrabajadorId).ToList().Select(s => s.First()).ToList();

            var fechaActual = DateTime.Now.Date;

            var AltasDadas = query.FindAll(p => p.Fecha < fechaActual).ToList().OrderBy(o => o.Fecha).GroupBy(g => g.Fecha).Select(s => s.First()).ToList();
            foreach (var item in AltasDadas)
            {
                var oGraficoAltasBE = new GraficoAltasBE();
                oGraficoAltasBE.Dia = item.Fecha.Value.ToString("dd MMM");
                oGraficoAltasBE.Tipo = "#F9FF33";
                oGraficoAltasBE.Numero = query.FindAll(p => p.Fecha == item.Fecha).Count;
                result.Add(oGraficoAltasBE);
            }

            var AltasProgramadas = query.FindAll(p => p.Fecha >= fechaActual).ToList().OrderBy(o => o.Fecha).GroupBy(g => g.Fecha).Select(s => s.First()).ToList();
            foreach (var item in AltasProgramadas)
            {
                var oGraficoAltasBE = new GraficoAltasBE();
                oGraficoAltasBE.Dia = item.Fecha.Value.ToString("dd MMM");
                oGraficoAltasBE.Tipo = "#3352FF";
                oGraficoAltasBE.Numero = query.FindAll(p => p.Fecha == item.Fecha).GroupBy(g => g.TrabajadorId).Select(s => s.First()).ToList().Count;
                result.Add(oGraficoAltasBE);
            }

            return result;
        }

        public List<GraficoSedesBE> Sedes(DateTime fi, DateTime ff, List<int> sedesId)
        {
            var query = (from A in db.RegistroTrabajador
                         join B in db.Sede on A.SedeId equals B.Id
                         where A.FechaIngreso >= fi && A.FechaIngreso <= ff && sedesId.Contains(A.SedeId)
                         select new
                         {
                             B.NombreSede,
                             A.FechaIngreso,
                             A.ModoIngreso,
                             A.SedeId
                         }).OrderBy(o => o.FechaIngreso).ToList();

            var result = new List<GraficoSedesBE>();

            var SedesQuery = query.GroupBy(g => g.SedeId).Select(s => s.First());

            foreach (var item in SedesQuery)
            {
                var reg = new GraficoSedesBE();
                reg.Sede = item.NombreSede;
                reg.CasosAsintomaticos = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.AsintomaticoPositivo && p.SedeId == item.SedeId).Count;
                reg.CasosConfirmadoSintomatico = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.CovidConfirmadoSintomatico && p.SedeId == item.SedeId).Count;
                reg.CasosSospechosos = query.FindAll(p => p.ModoIngreso == (int)Common.Resource.Enums.ModoIngreso.Sospechoso && p.SedeId == item.SedeId).Count;
                result.Add(reg);
            }

            return result;
        }

        public IndicadoresDashboardBE IndicadoresDashboard(List<int> sedesId, int usuarioId, int tipoUsuarioId)
        {
            var oIndicadoresDashboardBE = new IndicadoresDashboardBE();

            try
            {
                List<IndicadoresBE> serviceDatas = new List<IndicadoresBE>();

                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerIndicadoresDashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuarioId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            IndicadoresBE data = new IndicadoresBE();
                            data.TrabajadorEnSeguimiento = dr.Field<int>("TrabajadorEnSeguimiento");
                            data.TotalSeguimientoHoy = dr.Field<int>("TotalSeguimientoHoy");
                            data.SeguimientoPorCompletarHoy = dr.Field<int>("SeguimientoPorCompletarHoy");
                            data.TotalAltasHoy = dr.Field<int>("TotalAltasHoy");
                            data.AltasPorCompletarHoy = dr.Field<int>("AltasPorCompletarHoy");
                            data.HospitalizadoHoy = dr.Field<int>("HospitalizadoHoy");
                            data.TotalAltas = dr.Field<int>("TotalAltas");
                            data.ModeradoCriticosHoy = dr.Field<int>("ModeradoCriticosHoy");
                            data.Cuarentena = dr.Field<int>("Cuarentena");

                            data.Fallecidos = dr.Field<int>("TotalFallecidos");

                            data.CovidPositivoAcumulado = dr.Field<int>("CovidPositivoAcumulado");
                            data.TotalIgG = dr.Field<int>("TotalIgG");
                            data.TotalIgM = dr.Field<int>("TotalIgM");
                            data.TotalIgG_IgM = dr.Field<int>("TotalIgG_IgM");

                            serviceDatas.Add(data);
                        }
                    }
                }

                var oAltas = AltasPendientes_Dadas(sedesId, usuarioId, tipoUsuarioId);
                oIndicadoresDashboardBE.SeguimientoTotales = serviceDatas[0].TrabajadorEnSeguimiento.ToString();
                oIndicadoresDashboardBE.AltasDadas = serviceDatas[0].TotalAltas.ToString();
                oIndicadoresDashboardBE.AltasHoy = oAltas.Dadas + "/" + oAltas.Hoy;// serviceDatas[0].AltasPorCompletarHoy.ToString() + "/" + serviceDatas[0].TotalAltasHoy.ToString();
                
                oIndicadoresDashboardBE.ProgramadosHoy = serviceDatas[0].SeguimientoPorCompletarHoy.ToString() + "/" + serviceDatas[0].TotalSeguimientoHoy.ToString();

                oIndicadoresDashboardBE.Hospitalizados = serviceDatas[0].HospitalizadoHoy.ToString();
                oIndicadoresDashboardBE.ModeradosCriticos = serviceDatas[0].ModeradoCriticosHoy.ToString();
                oIndicadoresDashboardBE.Cuarentena = serviceDatas[0].Cuarentena.ToString();
                oIndicadoresDashboardBE.CovidPositivoAcumulado = serviceDatas[0].CovidPositivoAcumulado.ToString();
                oIndicadoresDashboardBE.TotalIgG = serviceDatas[0].TotalIgG.ToString();
                oIndicadoresDashboardBE.TotalIgM = serviceDatas[0].TotalIgM.ToString();
                oIndicadoresDashboardBE.TotalIgG_IgM = serviceDatas[0].TotalIgG_IgM.ToString();

                oIndicadoresDashboardBE.Fallecidos = serviceDatas[0].Fallecidos.ToString();

                return oIndicadoresDashboardBE;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int VerificarAltasCumplidas(List<FechaImportante> altasHoy)
        {
            var trabajadores = (from A in db.RegistroTrabajador where A.Eliminado == 0 select A).ToList();
            trabajadores = trabajadores.FindAll(p => p.EstadoClinicoId == null);
            var seguimientosAltas = db.Seguimiento.Where(w => w.TipoEstadoId == 5).Select(x => x.RegistroTrabajadorId).ToList();
            trabajadores = trabajadores.FindAll(p => !seguimientosAltas.Contains(p.Id)).ToList();

            var contador = 0;
            foreach (var item in altasHoy)
            {
                var obj = trabajadores.Find(p => p.Id == item.TrabajadorId);
                if (obj != null)
                {
                    if (obj.EstadoClinicoId == null)
                    {
                        contador++;
                    }
                }
            }

            return contador;
        }

        private int VerificarSeguimientosCumplidos(List<Seguimiento> seguimientosHoy)
        {
            var fechaActual = DateTime.Now.Date;
            var contador = 0;
            var seguimientosRealizados = (from A in db.Seguimiento where A.FechaIngresa >= fechaActual select A).ToList();
            foreach (var item in seguimientosHoy)
            {
                var obj = seguimientosRealizados.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId);
                if (obj != null)
                {
                    contador++;
                }
            }

            return contador;
        }

        public List<int> ListarSeguimientosHoy(List<int> sedesId)
        {
            var fechaHoy = DateTime.Now.Date;
            var result = new List<int>();
            var seguimientosHoy = (from A in db.Seguimiento
                                   join B in db.RegistroTrabajador on A.RegistroTrabajadorId equals B.Id
                                   where sedesId.Contains(B.SedeId)
                                   && A.ProximoSeguimiento == fechaHoy
                                   select new
                                   {
                                       IdTrabajador = B.Id
                                   }
                                ).ToList();

            var seguimientosRealizados = (from A in db.Seguimiento where A.FechaIngresa >= fechaHoy select A).ToList();
            foreach (var item in seguimientosHoy)
            {
                var obj = seguimientosRealizados.Find(p => p.RegistroTrabajadorId == item.IdTrabajador);
                if (obj == null)
                {
                    result.Add(item.IdTrabajador);
                }
            }

            //foreach (var item in seguimientosHoy)
            //{
            //    result.Add(item.IdTrabajador);
            //}

            return result;
        }

        public List<int> ListarAltasHoy(List<int> sedesId)
        {
            var fechaHoy = DateTime.Now.Date;
            var result = new List<int>();
            var altasHoy = (from A in db.FechaImportante
                            join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                            where sedesId.Contains(B.SedeId)
                            && (A.Descripcion == "FechaPosibleAlta" || A.Descripcion == "FechaPosibleAlta2" || A.Descripcion == "FechaPosibleAlta3")
                            && A.Fecha == fechaHoy

                            select new
                            {
                                IdTrabajador = B.Id,
                                Descipcion = A.Descripcion,
                            }
                              ).ToList();

            foreach (var item in altasHoy)
            {
                result.Add(item.IdTrabajador);
            }

            return result;
        }

        public List<int> ListarHospitalizadosHoy(List<int> sedesId)
        {
            var result = new List<int>();

            var seguimientos = (from A in db.Seguimiento
                                join B in db.RegistroTrabajador on A.RegistroTrabajadorId equals B.Id
                                where A.Eliminado == 0 && sedesId.Contains(B.SedeId)
                                select new
                                {
                                    RegistroTrabajadorId = A.RegistroTrabajadorId,
                                    NroSeguimiento = A.NroSeguimiento,
                                    TipoEstadoId = A.TipoEstadoId
                                }).ToList();

            var trabajadoresHospitalizados = seguimientos.GroupBy(g => g.RegistroTrabajadorId).Select(s => s.First()).ToList();

            foreach (var item in trabajadoresHospitalizados)
            {
                var ultimoSeguimiento = seguimientos
                                        .FindAll(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId)
                                        .OrderByDescending(p => p.NroSeguimiento)
                                        .ToList();

                if (ultimoSeguimiento.Count > 0)
                {
                    if (ultimoSeguimiento[0].TipoEstadoId == (int)TipoEstado.Hospitalizado)
                    {
                        result.Add(ultimoSeguimiento[0].RegistroTrabajadorId);
                    }
                }
            }

            return result;
        }

        public List<int> ListarModeradosCriticos(List<int> sedesId)
        {
            var result = new List<int>();

            var seguimientos = (from A in db.Seguimiento
                                join B in db.RegistroTrabajador on A.RegistroTrabajadorId equals B.Id
                                where A.Eliminado == 0 && sedesId.Contains(B.SedeId)
                                select new
                                {
                                    RegistroTrabajadorId = A.RegistroTrabajadorId,
                                    NroSeguimiento = A.NroSeguimiento,
                                    TipoEstadoId = A.TipoEstadoId
                                }).ToList();

            var trabajadoresHospitalizados = seguimientos.GroupBy(g => g.RegistroTrabajadorId).Select(s => s.First()).ToList();

            foreach (var item in trabajadoresHospitalizados)
            {
                var ultimoSeguimiento = seguimientos
                                        .FindAll(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId)
                                        .OrderByDescending(p => p.NroSeguimiento)
                                        .ToList();

                if (ultimoSeguimiento.Count > 0)
                {
                    if (ultimoSeguimiento[0].TipoEstadoId == (int)TipoEstado.AislamientoModerado || ultimoSeguimiento[0].TipoEstadoId == (int)TipoEstado.AislamientoSevero)
                    {
                        result.Add(ultimoSeguimiento[0].RegistroTrabajadorId);
                    }
                }
            }

            return result;
        }

        public List<int> ListarCuarentena(List<int> sedesId)
        {
            var result = new List<int>();

            var seguimientos = (from A in db.Seguimiento
                                join B in db.RegistroTrabajador on A.RegistroTrabajadorId equals B.Id
                                where A.Eliminado == 0 && sedesId.Contains(B.SedeId)
                                select new
                                {
                                    RegistroTrabajadorId = A.RegistroTrabajadorId,
                                    NroSeguimiento = A.NroSeguimiento,
                                    TipoEstadoId = A.TipoEstadoId
                                }).ToList();

            var trabajadoresHospitalizados = seguimientos.GroupBy(g => g.RegistroTrabajadorId).Select(s => s.First()).ToList();

            foreach (var item in trabajadoresHospitalizados)
            {
                var ultimoSeguimiento = seguimientos
                                        .FindAll(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId)
                                        .OrderByDescending(p => p.NroSeguimiento)
                                        .ToList();

                if (ultimoSeguimiento.Count > 0)
                {
                    if (ultimoSeguimiento[0].TipoEstadoId == (int)TipoEstado.Cuarentena)
                    {
                        result.Add(ultimoSeguimiento[0].RegistroTrabajadorId);
                    }
                }
            }

            return result;
        }

        public List<int> ListarAltasDadas(List<int> sedesId)
        {
            var result = new List<int>();

            var altasTotal = (from A in db.RegistroTrabajador
                              join B in db.Empresa on A.EmpresaId equals B.Id
                              join C in db.Sede on A.SedeId equals C.Id
                              join HC in db.HeadCount on A.Dni equals HC.Dni
                              join D in db.Seguimiento on A.Id equals D.RegistroTrabajadorId
                              join E in db.Usuario on A.MedicoVigilaId equals E.Id
                              join F in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = F.i_ParameterId, b = F.i_GroupId }
                              join G in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = G.i_ParameterId, b = G.i_GroupId }
                              join H in db.Parametro on new { a = A.TipoContacto.Value, b = 105 } equals new { a = H.i_ParameterId, b = H.i_GroupId } into G_join
                              from H in G_join.DefaultIfEmpty()
                              where sedesId.Contains(A.SedeId)
                             //where
                             //A.Eliminado == 0 && sedesId.Contains(A.SedeId)
                             && D.TipoEstadoId == 5
                              //&& A.EstadoClinicoId == 1
                              select new
                              {
                                  RegistroTrabajadorId = A.Id
                              }).ToList();

            foreach (var item in altasTotal)
            {
                result.Add(item.RegistroTrabajadorId);
            }

            return result;
        }

        public AltasBE AltasPendientes_Dadas(List<int> sedesId, int medicoVigilaId, int TipoUsuarioId)
        {
            var fechaActual = DateTime.Now.Date;
            var contadorAltasTotalHoy = 0;
            var contadorAltasPendientes = 0;

            if (TipoUsuarioId == (int)TipoUsuario.AdministradorGeneral)
            {
                var altasTotalHoy = (from A in db.FechaImportante
                                     join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                     where A.Fecha == fechaActual
                                     && A.Descripcion.Contains("alta")
                                     && sedesId.Contains(B.SedeId)

                                     select A).ToList();

                altasTotalHoy = altasTotalHoy.GroupBy(p => p.TrabajadorId).Select(s => s.First()).ToList();
                contadorAltasTotalHoy = altasTotalHoy.Count();

                contadorAltasPendientes = 0;
                foreach (var item in altasTotalHoy)
                {
                    var condicion = (from A in db.FechaImportante
                                     join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                     where A.TrabajadorId == item.TrabajadorId
                                     && A.Fecha == fechaActual
                                     && sedesId.Contains(B.SedeId)
                                     && A.Descripcion.Contains("alta")
                                     && B.EstadoClinicoId == null

                                     select A).ToList();
                    if (condicion.Count == 1)
                    {
                        var fechaPosterior = (from A in db.FechaImportante where A.TrabajadorId == item.TrabajadorId && A.Fecha > fechaActual select A).ToList();
                        if (fechaPosterior.Count > 0)
                        {

                        }
                        else
                        {
                            contadorAltasPendientes++;
                        }
                    }

                }
            }
            else if (TipoUsuarioId == (int)TipoUsuario.MedicoVigilancia || TipoUsuarioId == (int)TipoUsuario.MedicoAdministrador)
            {
                var altasTotalHoy = (from A in db.FechaImportante
                                join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                where A.Fecha == fechaActual
                                && A.Descripcion.Contains("alta")
                                && sedesId.Contains(B.SedeId)
                                && B.MedicoVigilaId == medicoVigilaId
                                //&& B.EstadoClinicoId == null
                                     select A).ToList();


                var AltasTotalHoy = altasTotalHoy.GroupBy(g => g.TrabajadorId).Select(s => s.First()).ToList();
                contadorAltasTotalHoy = AltasTotalHoy.Count();

                contadorAltasPendientes = 0;
                foreach (var item in AltasTotalHoy)
                {
                    var condicion = (from A in db.FechaImportante
                                     join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                     where A.TrabajadorId == item.TrabajadorId
                                     && A.Fecha == fechaActual
                                     && sedesId.Contains(B.SedeId)
                                     && B.MedicoVigilaId == medicoVigilaId
                                     && A.Descripcion.Contains("alta")
                                     && B.EstadoClinicoId == null
                                     select A).ToList();


                    if (condicion.Count == 1)
                    {
                        var fechaPosterior = (from A in db.FechaImportante where A.TrabajadorId == item.TrabajadorId && A.Fecha > fechaActual select A).ToList();
                        if (fechaPosterior.Count > 0)
                        {
                            
                        }
                        else
                        {
                            contadorAltasPendientes++;
                        }
                        
                    }

                }
            }



            var oAltasBE = new AltasBE();
            oAltasBE.Hoy = contadorAltasTotalHoy;
            oAltasBE.Dadas = contadorAltasPendientes;

            return oAltasBE;
        }

        public List<ListaTrabajadoresBE> Lista_AltasPendientes(List<int> sedesId, int medicoVigilaId, int TipoUsuarioId)
        {
            var fechaActual = DateTime.Now.Date;
            var contadorAltasTotalHoy = 0;
            var contadorAltasPendientes = 0;
            var listaIds = new List<int>();

            if (TipoUsuarioId == (int)TipoUsuario.AdministradorGeneral)
            {
                var altasTotalHoy = (from A in db.FechaImportante
                                join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                where A.Fecha == fechaActual                                
                                && A.Descripcion.Contains("alta")
                                && sedesId.Contains(B.SedeId)

                                select A).ToList();

                altasTotalHoy = altasTotalHoy.GroupBy(p => p.TrabajadorId).Select(s => s.First()).ToList();
                contadorAltasTotalHoy = altasTotalHoy.Count();

                
                contadorAltasPendientes = 0;
                foreach (var item in altasTotalHoy)
                {
                    var condicion = (from A in db.FechaImportante
                                     join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                     where A.TrabajadorId == item.TrabajadorId
                                     && A.Fecha == fechaActual
                                     && sedesId.Contains(B.SedeId)
                                     && A.Descripcion.Contains("alta")
                                     && B.EstadoClinicoId == null 

                                     select A).ToList();
                    if (condicion.Count == 1)
                    {
                        var fechaPosterior = (from A in db.FechaImportante where A.TrabajadorId == item.TrabajadorId && A.Fecha > fechaActual select A).ToList();
                        if (fechaPosterior.Count > 0)
                        {

                        }
                        else
                        {
                            listaIds.Add(condicion[0].TrabajadorId);
                            contadorAltasPendientes++;
                        }

                    }

                }
            }
            else if (TipoUsuarioId == (int)TipoUsuario.MedicoVigilancia || TipoUsuarioId == (int)TipoUsuario.MedicoAdministrador)
            {
                var altasHoy = (from A in db.FechaImportante
                                join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                where A.Fecha == fechaActual
                                && A.Descripcion.Contains("alta")
                                && sedesId.Contains(B.SedeId)
                                && B.MedicoVigilaId == medicoVigilaId
                                select A).ToList();


                contadorAltasTotalHoy = altasHoy.Count();

                contadorAltasPendientes = 0;
                foreach (var item in altasHoy)
                {
                    var condicion = (from A in db.FechaImportante
                                     join B in db.RegistroTrabajador on A.TrabajadorId equals B.Id
                                     where A.TrabajadorId == item.TrabajadorId
                                     && A.Fecha == fechaActual
                                     && sedesId.Contains(B.SedeId)
                                     && B.MedicoVigilaId == medicoVigilaId
                                     && A.Descripcion.Contains("alta")
                                     && B.EstadoClinicoId == null
                                     select A).ToList();
                    
                    if (condicion.Count == 1)
                    {
                        var fechaPosterior = (from A in db.FechaImportante where A.TrabajadorId == item.TrabajadorId && A.Fecha > fechaActual select A).ToList();
                        if (fechaPosterior.Count > 0)
                        {

                        }
                        else
                        {
                            listaIds.Add(condicion[0].TrabajadorId);
                            contadorAltasPendientes++;
                        }

                    }

                }
            }


            var lista = new WorkerRegisterBL();

            return lista.ListarTrabajadoresPorIds(listaIds);            
        }
    }
}