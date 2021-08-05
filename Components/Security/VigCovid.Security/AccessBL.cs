using System.Collections.Generic;
using System.Linq;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using static VigCovid.Common.Resource.Enums;

namespace VigCovid.Security
{
    public class AccessBL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Usuario ValidarUsuario(string usuario, string password)
        {
            var Usuario = (from A in db.Usuario where A.NombreUsuario == usuario && A.PasswordUsuario == password select A).FirstOrDefault();
            if (Usuario != null)
            {
                return Usuario;
            }
            return null;
        }

        public List<EmpresaBE> GetEmpresasRegistro(int usuarioId)
        {
            var empresas = (
                            from A in db.AccesoInformacion
                            join B in db.Empresa on A.EmpresaId equals B.Id
                            where A.UsuarioId == usuarioId
                            group B by new
                            {
                                B.Id,
                                B.CodigoEmpresa,
                                B.NombreEmpresa,
                                B.OrganizationId
                            } into C
                            select new EmpresaBE
                            {
                                EmpresaId = C.Key.Id,
                                EmpresaNombre = C.Key.NombreEmpresa,
                                OrganizationId = C.Key.OrganizationId
                            }
             ).ToList();

            return empresas;
        }

        public PermisosBE PermisoPorTipoUsuario(int tipoUsuario)
        {
            var oPermisosBE = new PermisosBE();

            if (tipoUsuario == (int)TipoUsuario.MedicoVigilancia)
            {
                oPermisosBE.AsignarPacientes = true;
                oPermisosBE.PacientesAsignados = true;
                oPermisosBE.AccesoOtrosPacientesLectura = false;
                oPermisosBE.AccesoOtrosPacientesModificacion = false;
            }
            else if (tipoUsuario == (int)TipoUsuario.MedicoAdministrador)
            {
                oPermisosBE.AsignarPacientes = true;
                oPermisosBE.PacientesAsignados = true;
                oPermisosBE.AccesoOtrosPacientesLectura = true;
                oPermisosBE.AccesoOtrosPacientesModificacion = true;
            }
            else if (tipoUsuario == (int)TipoUsuario.RecursosHumanos)
            {
                oPermisosBE.AsignarPacientes = false;
                oPermisosBE.PacientesAsignados = false;
                oPermisosBE.AccesoOtrosPacientesLectura = true;
                oPermisosBE.AccesoOtrosPacientesModificacion = false;
            }
            else if (tipoUsuario == (int)TipoUsuario.AdministradorGeneral)
            {
                oPermisosBE.AsignarPacientes = true;
                oPermisosBE.PacientesAsignados = false;
                oPermisosBE.AccesoOtrosPacientesLectura = true;
                oPermisosBE.AccesoOtrosPacientesModificacion = true;
            }
            else if (tipoUsuario == (int)TipoUsuario.Enfermera)
            {
                oPermisosBE.AsignarPacientes = true;
                oPermisosBE.PacientesAsignados = false;
                oPermisosBE.AccesoOtrosPacientesLectura = false;
                oPermisosBE.AccesoOtrosPacientesModificacion = false;
            }

            return oPermisosBE;
        }

        public List<EmpresaBE> ObtenerEmpresaSedesPorUsuario(int usuarioId, int tipoUsuario)
        {
            if (tipoUsuario == (int)TipoUsuario.RecursosHumanos || tipoUsuario == (int)TipoUsuario.AdministradorGeneral)
            {
                //var empresas = (from A in db.AccesoInformacion
                //                join B in db.Empresa on A.EmpresaId equals B.Id
                //                where A.UsuarioId == usuarioId
                //                select new EmpresaBE
                //                {
                //                    EmpresaId = B.Id,
                //                    EmpresaNombre = B.NombreEmpresa,
                //                    Sedes = (from A1 in db.AccesoInformacion
                //                             join B1 in db.Sede on A1.SedeId equals B1.Id
                //                             where A1.UsuarioId == usuarioId
                //                             select new SedeBE
                //                             {
                //                                 SedeId = B1.Id,
                //                                 SedeNombre = B1.NombreSede
                //                             }).ToList()
                //                }).ToList();


                var empresas = (from A in db.AccesoInformacion
                                join B in db.Empresa on A.EmpresaId equals B.Id
                                where A.UsuarioId == usuarioId
                                group B by new
                                {
                                    B.Id,
                                    B.CodigoEmpresa,
                                    B.NombreEmpresa,
                                    B.OrganizationId
                                } into C
                                select new EmpresaBE
                                {
                                    EmpresaId = C.Key.Id,
                                    EmpresaNombre = C.Key.NombreEmpresa,
                                    Sedes = (from A1 in db.Sede
                                             where A1.EmpresaId == C.Key.Id
                                             select new SedeBE
                                             {
                                                 SedeId = A1.Id,
                                                 SedeNombre = A1.NombreSede
                                             }).ToList()
                                }).ToList();
                return empresas;
            }
            //else if(tipoUsuario == (int)TipoUsuario.Enfermera)
            //{
            //    var empresas = (from A in db.AccesoInformacion
            //                    join B in db.Empresa on A.EmpresaId equals B.Id
            //                    where A.UsuarioId == usuarioId
            //                    select new EmpresaBE
            //                    {
            //                        EmpresaId = B.Id,
            //                        EmpresaNombre = B.NombreEmpresa,
            //                        Sedes = (from A1 in db.AccesoInformacion
            //                                 join B1 in db.Sede on A1.SedeId equals B1.Id
            //                                 where A1.UsuarioId == usuarioId
            //                                 select new SedeBE
            //                                 {
            //                                     SedeId = B1.Id,
            //                                     SedeNombre = B1.NombreSede
            //                                 }).ToList()
            //                    }).ToList();
            //    return empresas;
            //}
            else
            {
                //var empresas = (from A in db.AccesoInformacion
                //                join B in db.Empresa on A.EmpresaId equals B.Id
                //                where A.UsuarioId == usuarioId
                //                select new EmpresaBE
                //                {
                //                    EmpresaId = B.Id,
                //                    EmpresaNombre = B.NombreEmpresa,
                //                    Sedes = (from A1 in db.AccesoInformacion
                //                             join B1 in db.Sede on A1.SedeId equals B1.Id
                //                             where A1.UsuarioId == usuarioId
                //                             select new SedeBE
                //                             {
                //                                 SedeId = B1.Id,
                //                                 SedeNombre = B1.NombreSede
                //                             }).ToList()
                //                }).ToList();
                //return empresas;
                var empresas = (from A in db.AccesoInformacion
                                join B in db.Empresa on A.EmpresaId equals B.Id
                                where A.UsuarioId == usuarioId
                                group B by new
                                {
                                    B.Id,
                                    B.CodigoEmpresa,
                                    B.NombreEmpresa,
                                    B.OrganizationId
                                } into C
                                select new EmpresaBE
                                {
                                    EmpresaId = C.Key.Id,
                                    EmpresaNombre = C.Key.NombreEmpresa,
                                    OrganizationId = C.Key.OrganizationId,
                                    Sedes = (from A1 in db.Sede
                                             where A1.EmpresaId == C.Key.Id
                                             select new SedeBE
                                             {
                                                 SedeId = A1.Id,
                                                 SedeNombre = A1.NombreSede
                                             }).ToList()
                                }).ToList();
                return empresas;
            }
        }

        public List<Usuario> ListarUsuarios()
        {
            var usuarios = (from A in db.Usuario where A.TipoUsuarioId == (int)TipoUsuario.MedicoVigilancia || A.TipoUsuarioId == (int)TipoUsuario.MedicoAdministrador select A).ToList();

            return usuarios;
        }

        public bool EsPropietario(int UsuarioLogeadoId, int TrabajadorId)
        {
            var trabajador = (from A in db.RegistroTrabajador where A.Id == TrabajadorId select A).FirstOrDefault();

            if (trabajador.MedicoVigilaId == UsuarioLogeadoId)
                return true;

            return false;
        }
    }
}