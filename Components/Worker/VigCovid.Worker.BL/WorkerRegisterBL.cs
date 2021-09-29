using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Common.Resource;
using static VigCovid.Common.Resource.Enums;

namespace VigCovid.Worker.BL
{
    public class WorkerRegisterBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public bool VerificarDuplicidadRegistro(RegistroTrabajador registrarTrabajador)
        //{
        //    var trabajador = (from A in db.RegistroTrabajador
        //                      where A.Dni == registrarTrabajador.Dni
        //                      select A).ToList();

        //    if (trabajador.Count > 0)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //Modificado por Saul Ramos - Verificacion de Datos
        public string VerificarDuplicidadRegistro(RegistroTrabajador registrarTrabajador)
        {
            var trabajador = (from A in db.RegistroTrabajador
                              where A.Dni == registrarTrabajador.Dni
                              select A).ToList();

            if (trabajador.Count > 0)
            {
                return "DNI";
            }

            if (registrarTrabajador.ConfirmarDuplicidadNombre ?? false)
            {
                return "OK";
            }

            var trabajadorNom = (from A in db.RegistroTrabajador
                                 where A.NombreCompleto + A.ApePaterno + A.ApeMaterno == registrarTrabajador.NombreCompleto + registrarTrabajador.ApePaterno + registrarTrabajador.ApeMaterno
                                 select A).ToList();

            if (trabajadorNom.Count > 0)
            {
                return "NOM";
            }


            return "OK";
        }



        public bool WorkerRegister(RegistroTrabajador registrarTrabajador)
        {
            
            try
            {
                registrarTrabajador.FechaIngreso = DateTime.Now.Date;

                registrarTrabajador.Eliminado = 0;
                registrarTrabajador.FechaIngresa = DateTime.Now;
                registrarTrabajador.UsuarioIngresa = registrarTrabajador.UsuarioIngresa;
                registrarTrabajador.TipoIngreso = 1;
                registrarTrabajador.NotificacionIngreso = 1;

                db.RegistroTrabajador.Add(registrarTrabajador);
                db.SaveChanges();

                var trabajadorId = registrarTrabajador.Id;

                if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.AsintomaticoPositivo)
                {
                    var oExamen = new Examen();

                    oExamen.Fecha = registrarTrabajador.FechaExamen.Value;
                    oExamen.TrabajadorId = trabajadorId;
                    oExamen.TipoPrueba = registrarTrabajador.TipoExamen.Value;
                    oExamen.Resultado = registrarTrabajador.ResultadoExamen.Value;

                    db.Examen.Add(oExamen);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool WorkerUpdate(RegistroTrabajador registrarTrabajador)
        {
            try
            {
                var entidadTrabjador = (from A in db.RegistroTrabajador where A.Id == registrarTrabajador.Id select A).FirstOrDefault();

                entidadTrabjador.ModoIngreso = registrarTrabajador.ModoIngreso;
                entidadTrabjador.ViaIngreso = registrarTrabajador.ViaIngreso;
                entidadTrabjador.NombreCompleto = registrarTrabajador.NombreCompleto;
                entidadTrabjador.ApePaterno = registrarTrabajador.ApePaterno;
                entidadTrabjador.ApeMaterno = registrarTrabajador.ApeMaterno;
                entidadTrabjador.Dni = registrarTrabajador.Dni;
                entidadTrabjador.Edad = registrarTrabajador.Edad;
                entidadTrabjador.PuestoTrabajo = registrarTrabajador.PuestoTrabajo;
                entidadTrabjador.Celular = registrarTrabajador.Celular;
                entidadTrabjador.TelfReferencia = registrarTrabajador.TelfReferencia;
                entidadTrabjador.Email = registrarTrabajador.Email;
                entidadTrabjador.Direccion = registrarTrabajador.Direccion;
                entidadTrabjador.Sexo = registrarTrabajador.Sexo;
                entidadTrabjador.EmpresaId = registrarTrabajador.EmpresaId;
                entidadTrabjador.Empleadora = registrarTrabajador.Empleadora;
                entidadTrabjador.SedeId = registrarTrabajador.SedeId;
                entidadTrabjador.MedicoVigilaId = registrarTrabajador.MedicoVigilaId;
                entidadTrabjador.TipoEmpresaId = registrarTrabajador.TipoEmpresaId;
                entidadTrabjador.FechaActualiza = DateTime.Now.Date;
                entidadTrabjador.UsuarioActualiza = registrarTrabajador.UsuarioActualiza;
                registrarTrabajador.TipoIngreso = registrarTrabajador.TipoIngreso;
                registrarTrabajador.NotificacionIngreso = registrarTrabajador.NotificacionIngreso;


                //db.RegistroTrabajador.Add(registrarTrabajador);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TrabajadorBE> BuscarTrabajador(string filter)
        {
            try
            {
                var result = new List<TrabajadorBE>();

                var porNombres = (from A in db.HeadCount where A.Nombres.Contains(filter) select new TrabajadorBE { Id = A.Id, NombresCompletosDoc = A.ApellidoPaterno + " " + A.ApellidoMaterno + ", " + A.Nombres + "-" + A.Dni }).ToList();
                if (porNombres.Count > 0)
                    result.AddRange(porNombres);

                var porApePaterno = (from A in db.HeadCount where A.ApellidoPaterno.Contains(filter) select new TrabajadorBE { Id = A.Id, NombresCompletosDoc = A.ApellidoPaterno + " " + A.ApellidoMaterno + ", " + A.Nombres + "-" + A.Dni }).ToList();
                if (porApePaterno.Count > 0)
                    result.AddRange(porApePaterno);

                var porApeMaterno = (from A in db.HeadCount where A.ApellidoMaterno.Contains(filter) select new TrabajadorBE { Id = A.Id, NombresCompletosDoc = A.ApellidoPaterno + " " + A.ApellidoMaterno + ", " + A.Nombres + "-" + A.Dni }).ToList();
                if (porApeMaterno.Count > 0)
                    result.AddRange(porApeMaterno);

                var porDocumento = (from A in db.HeadCount where A.Dni.Contains(filter) select new TrabajadorBE { Id = A.Id, NombresCompletosDoc = A.ApellidoPaterno + " " + A.ApellidoMaterno + ", " + A.Nombres + "-" + A.Dni }).ToList();
                if (porDocumento.Count > 0)
                    result.AddRange(porDocumento);

                result = result.GroupBy(g => g.Id).Select(s => s.First()).ToList();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Modificado por Saul Ramos 25052021  - HC  Empresa Principal usando OrganizationId
        public TrabajadorHcBE BuscarTrabajadorPorDni(string dni)
        {
            try
            {
                var trabajador = (from A in db.HeadCount join E in db.Empresa on A.OrganizationId equals E.OrganizationId
                                  where A.Dni == dni
                                  select new TrabajadorHcBE
                                  {
                                      HC = A.HC,
                                      EmpresaEmpleadora = A.EmpresaEmpleadora,
                                      ApellidoPaterno = A.ApellidoPaterno,
                                      ApellidoMaterno = A.ApellidoMaterno,
                                      Nombres = A.Nombres,
                                      Dni = A.Dni,
                                      Sexo = A.Sexo,
                                      CorreoTrabajador = A.CorreoTrabajador,
                                      Sede = A.Sede,
                                      Puesto = A.Puesto,
                                      FechaNacimiento = A.FechaNacimiento.Value,
                                      CorreoSupervisor = A.CorreoSupervisor,
                                      OrganizationId = A.OrganizationId,
                                      NombreEmpresa = E.NombreEmpresa
                                  }
                                  ).FirstOrDefault();

                return trabajador;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<ReporteAcumuladoManualBE> ListarTrabajadoresPorSedesReporteAcumulado(List<int> sedesId, int EP, DateTime FechaInicio, DateTime FechaFin)
        {
            List<ReporteAcumuladoManualBE> serviceDatas = new List<ReporteAcumuladoManualBE>();
            List<ReporteAcumuladoManualBE> serviceDatasModificado = new List<ReporteAcumuladoManualBE>();

            //var FECHA1 = FechaInicio.ToString("yyyy-dd-MM");
            //var FECHA2 = FechaFin.ToString("yyyy-dd-MM");

            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("ObtenerReporteTrabajadoresAcumulado", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmpresaPrincipal", SqlDbType.Int).Value = EP;
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = FechaFin;
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        ReporteAcumuladoManualBE data = new ReporteAcumuladoManualBE();
                        data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                        data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                        data.Idhmc = dr.Field<string>("Idhmc");
                        data.Dni = dr.Field<string>("Dni");
                        data.Edad = dr.Field<int>("Edad");
                        data.FechaRegistro = dr.Field<string>("FechaRegistro");
                        data.SedeId = dr.Field<int>("SedeId");
                        data.Sede = dr.Field<string>("Sede");
                        data.ModoIngreso = dr.Field<string>("ModoIngreso");
                        data.EstadoActual = dr.Field<string>("EstadoActual");
                        data.Telefono = dr.Field<string>("Telefono");
                        data.Puesto = dr.Field<string>("Puesto");
                        data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                        data.CentroCosteArea = dr.Field<string>("CentroCosteArea");
                        data.ViaIngreso = dr.Field<string>("ViaIngreso");
                        data.FechaUltimoDiaTrabajo = dr.Field<string>("FechaUltimoDiaTrabajo");
                       //Deshabilitado por Saul Ramos Vega -- 04082021
                        // data.AntecedentePatologico = dr.Field<string>("AntecedentePatologico");
                        data.TipoContacto = dr.Field<string>("TipoContacto");
                        data.NombreContacto = dr.Field<string>("NombreContacto");
                        //data.PrimerResultadoPr = dr.Field<string>("PrimerResultadoPr");
                        //data.FechaResultadoPr = dr.Field<string>("FechaResultadoPr");
                        //data.PrimerResultadoPositivoPr = dr.Field<string>("PrimerResultadoPositivoPr");
                        //data.FechaResultadoPositivoPr = dr.Field<string>("FechaResultadoPositivoPr");
                        //data.ResultadoPcr = dr.Field<string>("ResultadoPcr");
                        //data.FechaResultadoPcr = dr.Field<string>("FechaResultadoPcr");


                        //data.FechaInicioSintomas = dr.Field<string>("FechaInicioSintomas");
                        //data.FechanFinSintomas = dr.Field<string>("FechanFinSintomas");
                        //data.NroDiasSinSintomas = dr.Field<string>("NroDiasSinSintomas");
                        //data.FechaAislaminetoCuarentena = dr.Field<string>("FechaAislaminetoCuarentena");
                       
                        data.DescTipoRango = dr.Field<string>("DescTipoRango");
                        data.FechaInicio = dr.Field<DateTime>("FechaInicio");
                        data.FechanFin = dr.Field<DateTime>("FechaFin");
                        data.NroDiasDescansoMedico = dr.Field<string>("NroDiasDescansoMedico");
                        //data.NroDiasSegundoDescansoMedico = dr.Field<string>("NroDiasSegundoDescansoMedico");
                        //data.FechaPosibleAltaA = dr.Field<string>("FechaPosibleAltaA");
                        //data.NroDiasPosibleAltaA = dr.Field<string>("NroDiasPosibleAltaA");
                        //data.FechaPosibleAltaB = dr.Field<string>("FechaPosibleAltaB");
                        //data.NroDiasPosibleAltaB = dr.Field<string>("NroDiasPosibleAltaB");
                        //data.FechaPosibleAltaC = dr.Field<string>("FechaPosibleAltaC");
                        //data.NroDiasPosibleAltaC = dr.Field<string>("NroDiasPosibleAltaC");

                        
                        data.FechaAltaMedica = dr.Field<string>("FechaAltaMedica");
                        //data.NroDiasAltaMedica = dr.Field<string>("NroDiasAltaMedica");
                        data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                        data.MedicoVigila = dr.Field<string>("MedicoVigila");




                        serviceDatas.Add(data);
                    }
                }

                serviceDatasModificado = CompletarDataExcel(serviceDatas);

                serviceDatasModificado = (from A in serviceDatasModificado
                                          where sedesId.Contains(A.SedeId)
                                          select A).ToList();
            }

            return serviceDatasModificado;
        }


        //Se deshabilita para implementar nueva funcionalidad -- Saul RV --20210720 



        


            public List<ReporteAcumuladoManualBE> ListarTrabajadoresAltaHoy(List<int> sedesId, int EP, int IdUser)
        {
            List<ReporteAcumuladoManualBE> serviceDatas = new List<ReporteAcumuladoManualBE>();
            List<ReporteAcumuladoManualBE> serviceDatasModificado = new List<ReporteAcumuladoManualBE>();

            //var FECHA1 = FechaInicio.ToString("yyyy-dd-MM");
            //var FECHA2 = FechaFin.ToString("yyyy-dd-MM");

            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand("ObtenerReporteTrabajadoresAcumuladoAlta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EmpresaPrincipal", SqlDbType.Int).Value = EP;
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        ReporteAcumuladoManualBE data = new ReporteAcumuladoManualBE();
                        data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                        data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                        data.Idhmc = dr.Field<string>("Idhmc");
                        data.Dni = dr.Field<string>("Dni");
                        data.Edad = dr.Field<int>("Edad");
                        data.FechaRegistro = dr.Field<string>("FechaRegistro");
                        data.SedeId = dr.Field<int>("SedeId");
                        data.Sede = dr.Field<string>("Sede");
                        data.ModoIngreso = dr.Field<string>("ModoIngreso");
                        data.EstadoActual = dr.Field<string>("EstadoActual");
                        data.Telefono = dr.Field<string>("Telefono");
                        data.Puesto = dr.Field<string>("Puesto");
                        data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                        data.CentroCosteArea = dr.Field<string>("CentroCosteArea");
                        data.ViaIngreso = dr.Field<string>("ViaIngreso");
                        data.FechaUltimoDiaTrabajo = dr.Field<string>("FechaUltimoDiaTrabajo");
                        //Deshabilitado por Saul Ramos Vega -- 04082021
                        // data.AntecedentePatologico = dr.Field<string>("AntecedentePatologico");
                        data.TipoContacto = dr.Field<string>("TipoContacto");
                        data.NombreContacto = dr.Field<string>("NombreContacto");
                        //data.PrimerResultadoPr = dr.Field<string>("PrimerResultadoPr");
                        //data.FechaResultadoPr = dr.Field<string>("FechaResultadoPr");
                        //data.PrimerResultadoPositivoPr = dr.Field<string>("PrimerResultadoPositivoPr");
                        //data.FechaResultadoPositivoPr = dr.Field<string>("FechaResultadoPositivoPr");
                        //data.ResultadoPcr = dr.Field<string>("ResultadoPcr");
                        //data.FechaResultadoPcr = dr.Field<string>("FechaResultadoPcr");
                        data.FechaAltaMedica = dr.Field<string>("FechaAltaMedica");

                        //data.FechaInicioSintomas = dr.Field<string>("FechaInicioSintomas");
                        //data.FechanFinSintomas = dr.Field<string>("FechanFinSintomas");
                        //data.NroDiasSinSintomas = dr.Field<string>("NroDiasSinSintomas");
                        //data.FechaAislaminetoCuarentena = dr.Field<string>("FechaAislaminetoCuarentena");

                        //data.DescTipoRango = dr.Field<string>("DescTipoRango");
                        //data.FechaInicio = dr.Field<DateTime>("FechaInicio");
                        //data.FechanFin = dr.Field<DateTime>("FechaFin");
                        //data.NroDiasDescansoMedico = dr.Field<string>("NroDiasDescansoMedico");
                        //data.NroDiasSegundoDescansoMedico = dr.Field<string>("NroDiasSegundoDescansoMedico");
                        //data.FechaPosibleAltaA = dr.Field<string>("FechaPosibleAltaA");
                        //data.NroDiasPosibleAltaA = dr.Field<string>("NroDiasPosibleAltaA");
                        //data.FechaPosibleAltaB = dr.Field<string>("FechaPosibleAltaB");
                        //data.NroDiasPosibleAltaB = dr.Field<string>("NroDiasPosibleAltaB");
                        //data.FechaPosibleAltaC = dr.Field<string>("FechaPosibleAltaC");
                        //data.NroDiasPosibleAltaC = dr.Field<string>("NroDiasPosibleAltaC");


                        //data.FechaAltaMedica = dr.Field<string>("FechaAltaMedica");
                        //data.NroDiasAltaMedica = dr.Field<string>("NroDiasAltaMedica");
                        data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                        data.MedicoVigila = dr.Field<string>("MedicoVigila");




                        serviceDatas.Add(data);
                    }
                }

                serviceDatasModificado = CompletarDataExcel(serviceDatas);

                serviceDatasModificado = (from A in serviceDatasModificado
                                          where sedesId.Contains(A.SedeId)
                                          select A).ToList();
            }

            return serviceDatasModificado;
        }





        private List<ReporteAcumuladoManualBE> CompletarDataExcel(List<ReporteAcumuladoManualBE> serviceDatas)
        {
            var newServiceDatas = new List<ReporteAcumuladoManualBE>();
            
            var servicesIds = serviceDatas.Select(p => p.RegistroTrabajadorId).ToList();

            var fechasImportantes = (from A in db.FechaImportante where servicesIds.Contains(A.TrabajadorId) select A).ToList();
            var examenes = (from a in db.Examen
                            join E in db.Parametro on new { a = a.Resultado, b = 102 } equals new { a = E.i_ParameterId, b = E.i_GroupId }
                            join F in db.Parametro on new { a = a.TipoPrueba, b = 103 } equals new { a = F.i_ParameterId, b = F.i_GroupId }
                            where servicesIds.Contains(a.TrabajadorId) 
                            select new
                            {
                                TrabajadorId = a.TrabajadorId,
                                TipoExamen = F.v_Value1,
                                FechaExmen = a.Fecha,
                                ResultadoId =a.Resultado,
                                ResultadoExamen = E.v_Value1
                            }
                            ).ToList();

            foreach (var item in serviceDatas)
            {

                #region FECHA ÚLTIMO DÍADE TRABAJO
                var fechaUltimoDiaTrabajo = "--";
                var fechaUltimoDiaTrabajoData = fechasImportantes.Find(p => p.TrabajadorId == item.RegistroTrabajadorId && p.Descripcion == "FechaUltimodiatrabajo");
                if (fechaUltimoDiaTrabajoData != null)
                {
                    fechaUltimoDiaTrabajo = fechaUltimoDiaTrabajoData.Fecha.Value.ToString("dd/MM/yyy");
                }
                #endregion

                #region EXÁMENES
                var TipoExamen = "*****SIN PRUEBAS*****";
                var FechaExmen = "*****SIN PRUEBAS*****";
                var ResultadoExamen = "*****SIN PRUEBAS*****";

                var ultimoExamenPositivo = examenes.FindAll(p => p.TrabajadorId == item.RegistroTrabajadorId
                                            && (p.ResultadoId == (int)ResultadoCovid19.IgGPositivo
                                                || p.ResultadoId == (int)ResultadoCovid19.IgMPositivo
                                                || p.ResultadoId == (int)ResultadoCovid19.IgMeIgGpositivo
                                                || p.ResultadoId == (int)ResultadoCovid19.Positivo)).OrderByDescending(p => p.FechaExmen).ToList();

                if (ultimoExamenPositivo.Count > 0)
                {
                    TipoExamen = ultimoExamenPositivo[0].TipoExamen.ToString();
                    FechaExmen = ultimoExamenPositivo[0].FechaExmen.ToString();
                    ResultadoExamen = ultimoExamenPositivo[0].ResultadoExamen.ToString();
                }
                else
                {
                    TipoExamen = "*****EXAMEN NEGATIVO*****";
                    FechaExmen = "*****EXAMEN NEGATIVO*****";
                    ResultadoExamen = "*****EXAMEN NEGATIVO*****";
                }
                #endregion


                var objItem = new ReporteAcumuladoManualBE();
                objItem.RegistroTrabajadorId = item.RegistroTrabajadorId;
                objItem.ApellidosNombres = item.ApellidosNombres;
                objItem.Idhmc = item.Idhmc;
                objItem.Dni = item.Dni;
                objItem.Edad = item.Edad;
                objItem.FechaRegistro = item.FechaRegistro;
                objItem.SedeId = item.SedeId;
                objItem.Sede = item.Sede;
                objItem.ModoIngreso = item.ModoIngreso;
                objItem.EstadoActual = item.EstadoActual;
                objItem.Telefono = item.Telefono;
                objItem.Puesto = item.Puesto;
                objItem.DivisionPersonal = item.DivisionPersonal;
                objItem.CentroCosteArea = item.CentroCosteArea;
                objItem.ViaIngreso = item.ViaIngreso;
                objItem.FechaUltimoDiaTrabajo = item.FechaUltimoDiaTrabajo;
                //Modificado por Saul Ramos Vega -- 04082021 Reporte DM
                //objItem.AntecedentePatologico = item.AntecedentePatologico;
                objItem.TipoContacto = item.TipoContacto;
                objItem.NombreContacto = item.NombreContacto;
                objItem.TipoExamen = TipoExamen;
                objItem.FechaExamen = FechaExmen;
                objItem.ResultadoExamen = ResultadoExamen;
                //objItem.PrimerResultadoPr = item.PrimerResultadoPr;
                //objItem.FechaResultadoPr = item.FechaResultadoPr;
                //objItem.PrimerResultadoPositivoPr = item.PrimerResultadoPositivoPr;
                //objItem.FechaResultadoPositivoPr = item.FechaResultadoPositivoPr;
                //objItem.ResultadoPcr = item.ResultadoPcr;
                //objItem.FechaResultadoPcr = item.FechaResultadoPcr;


                //Datos comentados - 20072021 Saul RV
                //objItem.FechaInicioSintomas = item.FechaInicioSintomas;
                //objItem.FechanFinSintomas = item.FechanFinSintomas;
                //objItem.NroDiasSinSintomas = item.NroDiasSinSintomas;
                //objItem.FechaUltimoDiaTrabajo = fechaUltimoDiaTrabajo;

                //objItem.FechaAislaminetoCuarentena = item.FechaAislaminetoCuarentena;

                objItem.DescTipoRango = item.DescTipoRango;
                objItem.FechaInicio = item.FechaInicio;
                objItem.FechanFin = item.FechanFin;

                objItem.NroDiasDescansoMedico = item.NroDiasDescansoMedico;
               
                
                
                
                
                //objItem.NroDiasSegundoDescansoMedico = item.NroDiasSegundoDescansoMedico;
                //objItem.FechaPosibleAltaA = item.FechaPosibleAltaA;
                //objItem.NroDiasPosibleAltaA = item.NroDiasPosibleAltaA;
                //objItem.FechaPosibleAltaB = item.FechaPosibleAltaB;
                //objItem.NroDiasPosibleAltaB = item.NroDiasPosibleAltaB;
                //objItem.FechaPosibleAltaC = item.FechaPosibleAltaC;
                //objItem.NroDiasPosibleAltaC = item.NroDiasPosibleAltaC;

                //objItem.FechaPosibleAltaDD = item.FechaPosibleAltaDD;
                //objItem.NroDiasPosibleAltaD = item.NroDiasPosibleAltaD;

                //objItem.FechaPosibleAltaEE = item.FechaPosibleAltaEE;
                //objItem.NroDiasPosibleAltaE = item.NroDiasPosibleAltaE;

                //objItem.FechaPosibleAltaEE = item.FechaPosibleAltaFF;
                //objItem.NroDiasPosibleAltaE = item.NroDiasPosibleAltaF;

                //Revisar Implementación segun nueva logica de envio de DM - 31052021
                //objItem.FechaPosibleAltaDD = item.FechaPosibleAltaDD;
                //objItem.NroDiasPosibleAltaD = item.NroDiasPosibleAltaD;
                //objItem.FechaPosibleAltaEE = item.FechaPosibleAltaEE;
                //objItem.NroDiasPosibleAltaE = item.NroDiasPosibleAltaE;
                //objItem.FechaPosibleAltaFF = item.FechaPosibleAltaFF;
                
                //objItem.NroDiasPosibleAltaF = item.NroDiasPosibleAltaF;
                objItem.FechaAltaMedica = item.FechaAltaMedica;
                //objItem.NroDiasAltaMedica = item.NroDiasAltaMedica;
                
                
                objItem.MedicoVigilaId = item.MedicoVigilaId;
                objItem.MedicoVigila = item.MedicoVigila;

                newServiceDatas.Add(objItem);
            }

            return newServiceDatas;
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnSeguimientoHoy(int usuarioId, int tipoUsuario, int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnSeguimientoHoy", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnSeguimiento(int usuarioId, int tipoUsuario, int EmpresaId)

        //public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnSeguimiento(int usuarioId, int tipoUsuario)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnSeguimiento", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int?>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            //data.TipoEmpresaId  = dr.Field<int>("TipoEmpresaId");
                            serviceDatas.Add(data);
                        }
                    }
                }

                var resultserviceDatas = new List<ListaTrabajadoresBE>();

                foreach (var item in serviceDatas)
                {
                    if(item.EstadoDiario == "ALTA EPIDEMIOLÓGICA")
                    {
                        if (item.EstadoDiario == "ALTA EPIDEMIOLÓGICA")
                        {
                            //OBTENER FECHA DE ALTA
                            var fechaAlta = GetLastFechaAlta(item.RegistroTrabajadorId);
                            if (!string.IsNullOrEmpty(fechaAlta))
                            {
                                item.FechaAlta = fechaAlta;
                            }

                        }
                    }
                    else
                    {
                        item.FechaAlta = "----";
                    }

                    resultserviceDatas.Add(item);
                }


                return resultserviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetLastFechaAlta(int trabajadorId)
        {
            var fechas = (from a in db.FechaImportante where a.TrabajadorId == trabajadorId && a.Descripcion.Contains("alta") select a).OrderByDescending(p => p.Fecha).ToList();
          
            if (fechas.Count > 0)
            {
                return fechas[0].Fecha.Value.ToString("dd/MM/yyyy");
            }


            return "****";

        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnAlta(int usuarioId, int tipoUsuario, int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnAlta", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }

                        var resultserviceDatas = new List<ListaTrabajadoresBE>();

                        foreach (var item in serviceDatas)
                        {
                          
                            if (item.EstadoDiario == "ALTA EPIDEMIOLÓGICA")
                            {
                                //OBTENER FECHA DE ALTA
                                var fechaAlta = GetLastFechaAlta(item.RegistroTrabajadorId);
                           
                                if (!string.IsNullOrEmpty(fechaAlta))
                                {
                                    item.FechaAlta = fechaAlta;
                                }
                                
                            }
                            else
                            {
                                item.FechaAlta = "----";
                            }

                            resultserviceDatas.Add(item);
                        }


                        return resultserviceDatas;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnCuarentena(int usuarioId, int tipoUsuario, int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnCuarentena", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnModeradoCritico(int usuarioId, int tipoUsuario,int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnModeradoCritico", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresHospitalizadoHoy(int usuarioId, int tipoUsuario,int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresHospitalizadoHoy", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ListaTrabajadoresBE> ObtenerTrabajadoresEnAltaPendiente(int usuarioId, int tipoUsuario)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresEnAltaPendiente", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public List<ListaTrabajadoresBE> ObtenerTrabajadoresFallecidos(int usuarioId, int tipoUsuario, int EmpresaId)
        {
            List<ListaTrabajadoresBE> serviceDatas = new List<ListaTrabajadoresBE>();

            try
            {
                using (SqlConnection con = new SqlConnection(Constants.CONEXION))
                {
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("ObtenerTrabajadoresFallecidos", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@TipoUsuario", SqlDbType.Int).Value = tipoUsuario;
                        cmd.Parameters.Add("@EmpresaId", SqlDbType.Int).Value = EmpresaId;



                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            ListaTrabajadoresBE data = new ListaTrabajadoresBE();
                            data.RegistroTrabajadorId = dr.Field<int>("RegistroTrabajadorId");
                            data.ModoIngresoId = dr.Field<int>("ModoIngresoId");
                            data.ModoIngreso = dr.Field<string>("ModoIngreso");
                            data.ViaIngresoId = dr.Field<int>("ViaIngresoId");
                            data.ViaIngreso = dr.Field<string>("ViaIngreso");
                            data.FechaIngreso = dr.Field<string>("FechaIngreso");
                            data.NombreCompleto = dr.Field<string>("NombreCompleto");
                            data.ApePaterno = dr.Field<string>("ApePaterno");
                            data.ApeMaterno = dr.Field<string>("ApeMaterno");
                            data.Dni = dr.Field<string>("Dni");
                            data.Edad = dr.Field<int>("Edad");
                            data.PuestoTrabajo = dr.Field<string>("PuestoTrabajo");
                            data.Celular = dr.Field<string>("Celular");
                            data.TelfReferencia = dr.Field<string>("TelfReferencia");
                            data.Email = dr.Field<string>("Email");
                            data.Direccion = dr.Field<string>("Direccion");
                            data.Sexo = dr.Field<string>("Sexo");
                            data.EmpresaId = dr.Field<int>("EmpresaId");
                            data.NombreEmpresa = dr.Field<string>("NombreEmpresa");
                            data.Empleadora = dr.Field<string>("Empleadora");
                            data.SedeId = dr.Field<int>("SedeId");
                            data.NombreSede = dr.Field<string>("NombreSede");
                            data.MedicoVigilaId = dr.Field<int>("MedicoVigilaId");
                            data.MedicoVigila = dr.Field<string>("MedicoVigila");
                            data.EstadoClinicoId = dr.Field<Nullable<int>>("EstadoClinicoId");
                            data.ComentarioAlta = dr.Field<string>("ComentarioAlta");
                            data.NombreContacto = dr.Field<string>("NombreContacto");
                            data.TipoContactoId = dr.Field<Nullable<int>>("TipoContactoId");
                            data.TipoContacto = dr.Field<string>("TipoContacto");
                            data.Eliminado = dr.Field<int>("Eliminado");
                            data.UsuarioIngresa = dr.Field<int>("UsuarioIngresa");
                            data.FechaIngresa = dr.Field<string>("FechaIngresa");
                            data.UsuarioActualiza = dr.Field<int>("UsuarioActualiza");
                            data.FechaActualiza = dr.Field<string>("FechaActualiza");
                            data.HCM = dr.Field<string>("HCM");
                            data.DivisionPersonal = dr.Field<string>("DivisionPersonal");
                            data.CentroCoste = dr.Field<string>("CentroCoste");
                            data.ApellidosNombres = dr.Field<string>("ApellidosNombres");
                            data.EstadoDiario = dr.Field<string>("EstadoActual");
                            serviceDatas.Add(data);
                        }
                    }
                }

                return serviceDatas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }







        public List<ListaTrabajadoresBE> ListarTrabajadoresPorSedes(List<int> sedesId, int UsuarioId)
        {
            var query = (from A in db.RegistroTrabajador
                         join B in db.Empresa on A.EmpresaId equals B.Id
                         join C in db.Sede on A.SedeId equals C.Id
                         join D in db.Usuario on A.MedicoVigilaId equals D.Id
                         join E in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = E.i_ParameterId, b = E.i_GroupId }

                         where sedesId.Contains(A.SedeId)
                         //&& A.EstadoClinicoId == null
                         //&& A.UsuarioIngresa == UsuarioId
                         select new
                         {
                             RegistroTrabajadorId = A.Id,
                             NombreCompleto = A.ApePaterno + " " + A.ApeMaterno + "," + A.NombreCompleto,
                             Dni = A.Dni,
                             Empresa = B.NombreEmpresa,
                             Sede = C.NombreSede,
                             Edad = A.Edad,
                             FechaIngreso = A.FechaIngreso,
                             ModoIngreso = E.v_Value1,
                             //ModoIngreso ="",
                             EstadoDiario = "",
                             MedicoVigila = D.NombreUsuario,
                             UsuarioIngresa = A.UsuarioIngresa,
                             MedicoVigilaId = A.MedicoVigilaId,
                             EstadoClinicoId = A.EstadoClinicoId
                         }).ToList();

            var fechaHoy = DateTime.Now.Date;
            //var listaSeguimientosHoy = db.Seguimiento.Where(p => p.Fecha == fechaHoy).ToList();
            var listaSeguimientos = db.Seguimiento.Where(p => p.Eliminado == 0).ToList();
            var result = new List<ListaTrabajadoresBE>();

            foreach (var item in query)
            {
                var oListaTrabajadoresBE = new ListaTrabajadoresBE();
                oListaTrabajadoresBE.RegistroTrabajadorId = item.RegistroTrabajadorId;
                oListaTrabajadoresBE.NombreCompleto = item.NombreCompleto;
                oListaTrabajadoresBE.Dni = item.Dni;
                oListaTrabajadoresBE.Empresa = item.Empresa;
                oListaTrabajadoresBE.Sede = item.Sede;
                oListaTrabajadoresBE.Edad = item.Edad;
                oListaTrabajadoresBE.FechaIngreso = item.FechaIngreso.ToString("dd/MM/yyyy");
                oListaTrabajadoresBE.ModoIngreso = item.ModoIngreso;
                oListaTrabajadoresBE.MedicoVigila = item.MedicoVigila;
                var seguimientosTrabajador = listaSeguimientos.FindAll(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId);
                oListaTrabajadoresBE.EstadoDiario = ObtenerUltimoEstado(seguimientosTrabajador);
                //oListaTrabajadoresBE.Clasificacion = listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId) == null ? "" : ObtenerNombreCalificacion(listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId).ClasificacionId);
                //oListaTrabajadoresBE.UltimoComentario = listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId) == null ? "" : listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId).Comentario;
                oListaTrabajadoresBE.UsuarioIngresa = item.UsuarioIngresa;
                oListaTrabajadoresBE.MedicoVigilaId = item.MedicoVigilaId;
                //oListaTrabajadoresBE.ContadorSeguimiento = "0";
                //oListaTrabajadoresBE.DiaSinSintomas = "1";
                oListaTrabajadoresBE.EstadoClinicoId = item.EstadoClinicoId;
                result.Add(oListaTrabajadoresBE);
            }

            return result;
        }

        public List<ListaTrabajadoresBE> ListarTrabajadoresPorIds(List<int> ids)
        {
            var query = (from A in db.RegistroTrabajador
                         join B in db.Empresa on A.EmpresaId equals B.Id into B_join
                         from B in B_join.DefaultIfEmpty()
                         join C in db.Sede on A.SedeId equals C.Id into C_join
                         from C in C_join.DefaultIfEmpty()
                         join D in db.Usuario on A.MedicoVigilaId equals D.Id into D_join
                         from D in D_join.DefaultIfEmpty()
                         join E in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = E.i_ParameterId, b = E.i_GroupId } into E_join
                         from E in E_join.DefaultIfEmpty()

                         where ids.Contains(A.Id)
                         //&& A.EstadoClinicoId == null
                         //&& A.UsuarioIngresa == UsuarioId
                         select new
                         {
                             RegistroTrabajadorId = A.Id,
                             NombreCompleto = A.ApePaterno + " " + A.ApeMaterno + "," + A.NombreCompleto,
                             Dni = A.Dni,
                             Empresa = B.NombreEmpresa,
                             Sede = C.NombreSede,
                             Edad = A.Edad,
                             FechaIngreso = A.FechaIngreso,
                             ModoIngreso = E.v_Value1,
                             //ModoIngreso ="",
                             EstadoDiario = "",
                             MedicoVigila = D.NombreUsuario,
                             UsuarioIngresa = A.UsuarioIngresa,
                             MedicoVigilaId = A.MedicoVigilaId,
                             EstadoClinicoId = A.EstadoClinicoId
                         }).ToList();
            if (query.Count == 0) return new List<ListaTrabajadoresBE>();

            var fechaHoy = DateTime.Now.Date;
            //var listaSeguimientosHoy = db.Seguimiento.Where(p => p.Fecha == fechaHoy).ToList();
            var listaSeguimientos = db.Seguimiento.Where(p => p.Eliminado == 0).ToList();
            var result = new List<ListaTrabajadoresBE>();

            foreach (var item in query)
            {
                var oListaTrabajadoresBE = new ListaTrabajadoresBE();
                oListaTrabajadoresBE.RegistroTrabajadorId = item.RegistroTrabajadorId;
                oListaTrabajadoresBE.NombreCompleto = item.NombreCompleto;
                oListaTrabajadoresBE.ApellidosNombres = item.NombreCompleto;
                oListaTrabajadoresBE.Dni = item.Dni;
                oListaTrabajadoresBE.Empresa = item.Empresa;
                oListaTrabajadoresBE.NombreEmpresa = item.Empresa;                
                oListaTrabajadoresBE.Sede = item.Sede;
                oListaTrabajadoresBE.NombreSede = item.Sede;
                oListaTrabajadoresBE.Edad = item.Edad;
                oListaTrabajadoresBE.FechaIngreso = item.FechaIngreso.ToString("dd/MM/yyyy");
                oListaTrabajadoresBE.ModoIngreso = item.ModoIngreso;
                oListaTrabajadoresBE.MedicoVigila = item.MedicoVigila;
                var seguimientosTrabajador = listaSeguimientos.FindAll(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId);
                oListaTrabajadoresBE.EstadoDiario = ObtenerUltimoEstado(seguimientosTrabajador);
                //oListaTrabajadoresBE.Clasificacion = listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId) == null ? "" : ObtenerNombreCalificacion(listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId).ClasificacionId);
                //oListaTrabajadoresBE.UltimoComentario = listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId) == null ? "" : listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId).Comentario;
                oListaTrabajadoresBE.UsuarioIngresa = item.UsuarioIngresa;
                oListaTrabajadoresBE.MedicoVigilaId = item.MedicoVigilaId;
                //oListaTrabajadoresBE.ContadorSeguimiento = "0";
                //oListaTrabajadoresBE.DiaSinSintomas = "1";
                oListaTrabajadoresBE.EstadoClinicoId = item.EstadoClinicoId;
                result.Add(oListaTrabajadoresBE);
            }

            return result;
        }

        private string ObtenerUltimoEstado(List<Seguimiento> seguimientosTrabajador)
        {
            if (seguimientosTrabajador.Count > 0)
            {
                var ultimoEstadoId = seguimientosTrabajador.OrderByDescending(p => p.NroSeguimiento).ToList()[0].TipoEstadoId;

                return ObtenerNombreEstado(ultimoEstadoId.Value);
            }
            else
            {
                return "";
            }

            //listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId) == null ? "" : ObtenerNombreEstado(listaSeguimientosHoy.Find(p => p.RegistroTrabajadorId == item.RegistroTrabajadorId).TipoEstadoId.Value);
        }

        private string ObtenerNombreCalificacion(int clasificacionId)
        {
            if (clasificacionId == 1)
            {
                return "Asintomático";
            }
            else if (clasificacionId == 2)
            {
                return "Sintomático Leve";
            }
            else if (clasificacionId == 3)
            {
                return "Sintomático Moderado";
            }
            else
            {
                return "";
            }
        }

        private string ObtenerNombreEstado(int estadoId)
        {
            if (estadoId == 1)
            {
                return "CUARENTENA CASO DIRECTO";
            }
            else if (estadoId == 2)
            {
                return "HOSPITALIZADO";
            }
            else if (estadoId == 3)
            {
                return "FALLECIDO";
            }
            else if (estadoId == 4)
            {
                return "AISLAMIENTO-CASO LEVE";
            }
            else if (estadoId == 5)
            {
                return "ALTA EPIDEMIOLÓGICA";
            }
            else if (estadoId == 6)
            {
                return "AISLAMIENTO-CASO MODERADO";
            }
            else if (estadoId == 7)
            {
                return "AISLAMIENTO-CASO SEVERO";
            }
            else if (estadoId == 8)
            {
                return "AISLAMIENTO-CASO ASINTOMÁTICO";
            }
            else if (estadoId == 9)
            {
                return "AISLAMIENTO-CASO POST HOSPITALITARIO";
            }
            else
            {
                return "";
            }
        }

        public RegistroTrabajador GetTrabajadorById(int id)
        {
            return db.RegistroTrabajador.Where(w => w.Id == id).Select(p => p).FirstOrDefault();
        }

        public bool RemoveRegister (int idRegistro)
        {
            try
            {
                var examenes = (from A in db.Examen where A.TrabajadorId == idRegistro select A).ToList();

                foreach (var examen in examenes)
                {
                    db.Examen.Remove(examen);
                }

                var seguimientos = (from A in db.Seguimiento where A.RegistroTrabajadorId == idRegistro select A).ToList();

                foreach (var seguimiento in seguimientos)
                {
                    seguimiento.Eliminado = 1;
                }

                var registroTrabajador = (from A in db.RegistroTrabajador where A.Id == idRegistro select A).FirstOrDefault();
                registroTrabajador.Eliminado = 1;

                

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}