using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Common.Resource;
using static VigCovid.Common.Resource.Enums;

namespace VigCovid.MedicalMonitoring.BL
{
    public class DatosCabeceraBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public CabeceraSeguimientoBE GetCabecera(int idTrabajador)
        {
            var datosRegistro = (from A in db.RegistroTrabajador
                                 join B in db.Sede on A.SedeId equals B.Id
                                 join C in db.Parametro on new { a = A.ModoIngreso, b = 100 } equals new { a = C.i_ParameterId, b = C.i_GroupId } into C_join
                                 from C in C_join.DefaultIfEmpty()
                                 join D in db.Parametro on new { a = A.ViaIngreso, b = 101 } equals new { a = D.i_ParameterId, b = D.i_GroupId } into D_join
                                 from D in D_join.DefaultIfEmpty()
                                 where A.Id == idTrabajador
                                 select new CabeceraSeguimientoBE
                                 {
                                     Trabajador = A.ApePaterno + " " + A.ApeMaterno + ", " + A.NombreCompleto,
                                     Empresa = A.Empleadora,
                                     Celular = A.Celular,
                                     Email = A.Email,
                                     Sede = B.NombreSede,
                                     Puesto = A.PuestoTrabajo,
                                     ModoIngreso = C.v_Value1,
                                     ViaIngreso = D.v_Value1,
                                     EstadoClinico = A.EstadoClinicoId.Value
                                 }).FirstOrDefault();
            return datosRegistro;
        }

        public bool DarAlta(RequestDarAltaBE oRequestDarAltaBE)
        {
            try
            {
                //var seguimiento = db.Seguimiento.SingleOrDefault(b => b.Id == oRequestDarAltaBE.SeguimientoId);
                //seguimiento.TipoEstadoId = (int)TipoEstado.AltaEpidemiologica;

                //var registroTrabajador = db.RegistroTrabajador.SingleOrDefault(b => b.Id == oRequestDarAltaBE.TrabajadorId);
                //registroTrabajador.ComentarioAlta = oRequestDarAltaBE.ComentarioAlta;
                //registroTrabajador.EstadoClinicoId = 1;

                UpdateSeguimientoParaAltaMedica(oRequestDarAltaBE.SeguimientoId, oRequestDarAltaBE.ComentarioAlta, oRequestDarAltaBE.UsuarioId);
                InsertFechaImportanteAltaMedica(oRequestDarAltaBE.TrabajadorId, Convert.ToDateTime(oRequestDarAltaBE.FechaAlta));
                UpdateRegistroTrabajadorParaAltaMedica(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.ComentarioAlta, oRequestDarAltaBE.UsuarioId);

                //db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        public bool RecetasMedicasActualizar(RequestDarAltaBE oRequestDarAltaBE)
        {
            try
            {
                //var seguimiento = db.Seguimiento.SingleOrDefault(b => b.Id == oRequestDarAltaBE.SeguimientoId);
                //seguimiento.TipoEstadoId = (int)TipoEstado.AltaEpidemiologica;

                //var registroTrabajador = db.RegistroTrabajador.SingleOrDefault(b => b.Id == oRequestDarAltaBE.TrabajadorId);
                //registroTrabajador.ComentarioAlta = oRequestDarAltaBE.ComentarioAlta;
                //registroTrabajador.EstadoClinicoId = 1;

                UpdateSeguimientoReceta(oRequestDarAltaBE.SeguimientoId, oRequestDarAltaBE.UsuarioId, oRequestDarAltaBE.Receta);
                // InsertFechaImportanteAltaMedica(oRequestDarAltaBE.TrabajadorId, Convert.ToDateTime(oRequestDarAltaBE.FechaAlta));
                UpdateRegistroTrabajadorParaReceta(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.UsuarioId, oRequestDarAltaBE.Receta);

                //db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


        public bool DescansosMedicosActualizar(RequestDarAltaBE oRequestDarAltaBE)
        {
            try
            {
                //var seguimiento = db.Seguimiento.SingleOrDefault(b => b.Id == oRequestDarAltaBE.SeguimientoId);
                //seguimiento.TipoEstadoId = (int)TipoEstado.AltaEpidemiologica;

                //var registroTrabajador = db.RegistroTrabajador.SingleOrDefault(b => b.Id == oRequestDarAltaBE.TrabajadorId);
                //registroTrabajador.ComentarioAlta = oRequestDarAltaBE.ComentarioAlta;
                //registroTrabajador.EstadoClinicoId = 1;

                UpdateFechasImportantesDM(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.FechaInicio, oRequestDarAltaBE.FechaFin,oRequestDarAltaBE.TipoRango,oRequestDarAltaBE.Diagnostico,oRequestDarAltaBE.NroDias);
                // InsertFechaImportanteAltaMedica(oRequestDarAltaBE.TrabajadorId, Convert.ToDateTime(oRequestDarAltaBE.FechaAlta));
                // UpdateRegistroTrabajadorParaReceta(oRequestDarAltaBE.TrabajadorId, oRequestDarAltaBE.UsuarioId, oRequestDarAltaBE.Receta);

                //db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }



        private void UpdateSeguimientoParaAltaMedica(int id, string comentario, int usuario)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSeguimientoParaAltaMedica", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@Comentario", SqlDbType.VarChar).Value = comentario;
                    cmd.Parameters.Add("@UsuarioActualiza", SqlDbType.Int).Value = usuario;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

        //ACTUALIZAR LA TABLA DE SEGUIMIENTO CON LA RECETA
        private void UpdateSeguimientoReceta(int id, int usuario, string comentario)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSeguimientoReceta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@Comentario", SqlDbType.VarChar).Value = comentario;
                    cmd.Parameters.Add("@UsuarioActualiza", SqlDbType.Int).Value = usuario;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }





        //ACTUALIZAR INFORMACION DEL DESCANSO MEDICO


        private void UpdateFechasImportantesDM(int id, string FechaInicio, string FechaFin, int TipoRango, string Diagnostico, int NroDias)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateFechasImportantesDM", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = FechaInicio;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = FechaFin;
                    cmd.Parameters.Add("@TipoRango", SqlDbType.Int).Value = TipoRango;
                    cmd.Parameters.Add("@Diagnostico", SqlDbType.Int).Value = Diagnostico;
                    cmd.Parameters.Add("@NroDias", SqlDbType.Int).Value = NroDias;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }



        //ACTUALIZAR LA TABLA DE REGISTROTRABAJADOR CON LA RECETA


        private void UpdateRegistroTrabajadorParaReceta(int id, int usuario, string comentario)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSeguimientoRecetaRegistroTrabajador", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@ComentarioAlta", SqlDbType.VarChar).Value = comentario;
                    cmd.Parameters.Add("@UsuarioActualiza", SqlDbType.Int).Value = usuario;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }




        private void UpdateRegistroTrabajadorParaAltaMedica(int id, string comentario, int usuario)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateRegistroTrabajadorParaAltaMedica", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@ComentarioAlta", SqlDbType.VarChar).Value = comentario;
                    cmd.Parameters.Add("@UsuarioActualiza", SqlDbType.Int).Value = usuario;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

        private void InsertFechaImportanteAltaMedica(int id, DateTime fecha)
        {
            using (SqlConnection con = new SqlConnection(Constants.CONEXION))
            {
                using (SqlCommand cmd = new SqlCommand("InsertFechaImportanteAltaMedica", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TrabajadorId", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = fecha;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

    }
}