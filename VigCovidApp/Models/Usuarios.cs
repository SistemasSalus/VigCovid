using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VigCovid.Common.Resource;

namespace VigCovidApp.Models
{
    public class Usuarios
    {
        protected string _userName { get; set; }
        protected string _password { get; set; }
        public Usuarios(string userName, string password, int empresaCodigo)
        {
            _userName = userName;
            _password = password;
        }

        public List<EmpresaAsignada> ValidarUsuario()
        {
            var empresasAsignadas = new List<EmpresaAsignada>();
            //var     result = false;
            //if (_userName == "ricardo" && _password == "12345678" 
            //    && (_empresaCodigo == (int)Enums.CodigoEmpresa.SalusLaboris || _empresaCodigo == (int)Enums.CodigoEmpresa.Ambev))
            //{

            //    empresasAsignadas.Add(new
            //        EmpresaAsignada
            //    { Codigo = (int)Enums.CodigoEmpresa.SalusLaboris, Nombre = Enums.CodigoEmpresa.SalusLaboris.ToString() });

            //    empresasAsignadas.Add(new
            //        EmpresaAsignada
            //    { Codigo = (int)Enums.CodigoEmpresa.Ambev, Nombre = Enums.CodigoEmpresa.Ambev.ToString() });
            //}
            //else if (_userName == "luis" && _password == "12345678" && _empresaCodigo == (int)Enums.CodigoEmpresa.Backus)
            //{
            //    empresasAsignadas.Add(new
            //        EmpresaAsignada
            //    { Codigo = (int)Enums.CodigoEmpresa.Backus, Nombre = Enums.CodigoEmpresa.Backus.ToString() });
            //}

            return empresasAsignadas;

        }        
    }

    public class EmpresaAsignada
    {
        public int Codigo { get; set; }
        public string Empresa { get; set; }

    }
}