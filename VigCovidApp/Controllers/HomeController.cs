using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using VigCovid.Common.AccessData;
using VigCovid.Common.BE;
using VigCovid.Dashboard.BL;
using VigCovid.Security;
using VigCovid.Worker.BL;
using VigCovidApp.Controllers.Base;
using VigCovidApp.Models;
using VigCovidApp.ViewModels;
using VigCovid.MedicalMonitoring.BL;
using VigCovid.Report.BL;
using static VigCovid.Common.Resource.Enums;
using System.Data;


namespace VigCovidApp.Controllers
{
    public class HomeController : GenericController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var empresasAsignadas = sessione.EmpresasAsignadas;

            var empresasCodigos = new List<int>();
            foreach (var item in empresasAsignadas)
            {
                var sedeId = item.EmpresaIdSedeId.Split('-')[1];

               // var sedeId = item.EmpresaIdSedeId.Split('-')[4];

                empresasCodigos.Add(int.Parse(sedeId));
            }

            ViewBag.MEDICOSVIGILA = ListarUsuarios();
            ViewBag.EMPRESASASIGANADAS = empresasAsignadas;
            ViewBag.EmpresasRegistro = GetEmpresasRegistro(sessione.IdUser);
            ViewBag.INDICADORES = IndicadoresDashboard(empresasCodigos, sessione.IdUser, sessione.IdTipoUsuario);
            var sedesAsignadas = new List<SedeBE>();
            foreach (var item in empresasAsignadas)
            {
                var oSedeBE = new SedeBE();
                oSedeBE.SedeId = int.Parse(item.EmpresaIdSedeId.Split('-')[1]);
                oSedeBE.SedeNombre = item.EmpresaSede.Split('-')[1];
                sedesAsignadas.Add(oSedeBE);
            }
            ViewBag.SEDESASIGNADAS = sedesAsignadas;

            var listaTrabajadores = ListaTrabajadores(empresasCodigos, sessione.IdUser).ToList();
            int index = 1;
            listaTrabajadores.ToList().ForEach(x =>
            {
                x.Indice = index;
                index += 1;
            });
            return View(listaTrabajadores);
        }

        private List<EmpresaBE> GetEmpresasRegistro(int IdUser)
        {
            var oSedeBE = new SedeBE();
            var oAccessBL = new AccessBL();

            var result = oAccessBL.GetEmpresasRegistro(IdUser);

            return result;
        }

        public JsonResult GetSedesEmpresaPrincipal(int empresaId)
        {
            var oEmpresaSedeBL = new EmpresaSedeBL();

            var result = oEmpresaSedeBL.GetSedeEmpresaPrincipal(empresaId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        //public ActionResult RegistrarTrabajador(RegistroTrabajador registrarTrabajador)
        //{
        //    try
        //    {
        //        var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

        //        if (!sessione.AsignarPacientes) return null;

        //        var oWorkerRegisterBL = new WorkerRegisterBL();
        //        //registrarTrabajador.Sexo = "MASCULINO";
        //        registrarTrabajador.UsuarioIngresa = sessione.IdUser;

        //        if (oWorkerRegisterBL.VerificarDuplicidadRegistro(registrarTrabajador))
        //        {
        //            oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.Reingreso)
        //        {
        //            oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
        //            return RedirectToAction("Index", "Home");
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public ActionResult RegistrarTrabajador(RegistroTrabajador registrarTrabajador)
        {
            try
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

                if (!sessione.AsignarPacientes) return null;

                var oWorkerRegisterBL = new WorkerRegisterBL();
                //Modificado por Saul Ramos Vega - 06042021 - Valida si se realiza el registro y envía la notificaiones respectivas

                //Se quita la opción ed Reingreso  -  Creado por Saul Ramos Vega - 20210827

                registrarTrabajador.UsuarioIngresa = sessione.IdUser;
                string rpta = oWorkerRegisterBL.VerificarDuplicidadRegistro(registrarTrabajador);

                if (rpta == "OK")
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.Sospechoso)
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.AsintomaticoPositivo)
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.ContactoDirecto)
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.CovidConfirmadoSintomatico)
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.Sintomatico)
                {
                    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                    return RedirectToAction("Index", "Home");
                }

                //Se quita la opción ed Reingreso  -  Creado por Saul Ramos Vega - 20210827
                //else if (registrarTrabajador.ModoIngreso == (int)ModoIngreso.Reingreso)
                //{
                //    oWorkerRegisterBL.WorkerRegister(registrarTrabajador);
                //    GenerarNotificaciondeIngreso(registrarTrabajador.Id);
                //    GenerarNotificacionContactosDirectos(registrarTrabajador.Id);
                //    return RedirectToAction("Index", "Home");
                //}

                return Json(rpta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditarTrabajador(RegistroTrabajador registrarTrabajador)
        {
            try
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];

                var oWorkerRegisterBL = new WorkerRegisterBL();

                registrarTrabajador.UsuarioActualiza = sessione.IdUser;

                oWorkerRegisterBL.WorkerUpdate(registrarTrabajador);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public byte[] generateExcel(List<ReporteAcumuladoManualBE> datos)
        //{
        //    try
        //    {
        //        int rowIndex = 2;
        //        ExcelRange cell;
        //        ExcelFill fill;
        //        Border border;

        //        using (var excelPackage = new ExcelPackage())
        //        {
        //            excelPackage.Workbook.Properties.Author = "SL";
        //            excelPackage.Workbook.Properties.Title = "SL";
        //            var sheet = excelPackage.Workbook.Worksheets.Add("Reporte Excel");
        //            sheet.Name = "Acumulado Covid";

        //            sheet.Column(2).Width = 50;
        //            sheet.Column(3).Width = 50;
        //            sheet.Column(4).Width = 50;
        //            sheet.Column(5).Width = 50;
        //            sheet.Column(6).Width = 50;
        //            sheet.Column(7).Width = 50;
        //            sheet.Column(8).Width = 50;
        //            sheet.Column(9).Width = 50;
        //            sheet.Column(10).Width = 50;
        //            sheet.Column(11).Width = 50;

        //            sheet.Column(12).Width = 50;
        //            sheet.Column(13).Width = 50;
        //            sheet.Column(14).Width = 50;
        //            sheet.Column(15).Width = 50;
        //            sheet.Column(16).Width = 50;
        //            sheet.Column(17).Width = 50;
        //            sheet.Column(18).Width = 50;
        //            sheet.Column(19).Width = 50;
        //            sheet.Column(20).Width = 50;
        //            sheet.Column(21).Width = 50;

        //            sheet.Column(22).Width = 50;
        //            sheet.Column(23).Width = 50;
        //            sheet.Column(24).Width = 50;
        //            sheet.Column(25).Width = 50;
        //            sheet.Column(26).Width = 50;
        //            sheet.Column(27).Width = 50;
        //            sheet.Column(28).Width = 50;
        //            sheet.Column(29).Width = 50;
        //            sheet.Column(30).Width = 50;
        //            sheet.Column(31).Width = 50;

        //            sheet.Column(32).Width = 50;
        //            sheet.Column(33).Width = 50;
        //            sheet.Column(34).Width = 50;
        //            sheet.Column(35).Width = 50;
        //            sheet.Column(36).Width = 50;

        //            sheet.Column(37).Width = 50;
        //            sheet.Column(38).Width = 50;
        //            //sheet.Column(38).Width = 50;
        //            //sheet.Column(39).Width = 50;
        //            //sheet.Column(40).Width = 50;

        //            #region Report Header

        //            sheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
        //            cell = sheet.Cells[rowIndex, 2];
        //            cell.Value = "Reporte Acumulado al " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        //            cell.Style.Font.Bold = true;
        //            cell.Style.Font.Size = 20;
        //            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            rowIndex = rowIndex + 1;

        //            #endregion Report Header

        //            #region Table header

        //            cell = sheet.Cells[rowIndex, 2];
        //            cell.Value = "Trabajador";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 3];
        //            cell.Value = "Idhmc";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 4];
        //            cell.Value = "Dni";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 5];
        //            cell.Value = "Edad";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 6];
        //            cell.Value = "Fecha Registro";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 7];
        //            cell.Value = "Telefono";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 8];
        //            cell.Value = "Sede";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 9];
        //            cell.Value = "Puesto";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 10];
        //            cell.Value = "Divison Personal";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 11];
        //            cell.Value = "Centro Coste Area";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 12];
        //            cell.Value = "Motivo Ingreso";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 13];
        //            cell.Value = "Via Ingreso";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 14];
        //            cell.Value = "Estado Actual";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 15];
        //            cell.Value = "Fecha Ultimo Dia Trabajo";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 16];
        //            cell.Value = "Antecedente Patológico";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 17];
        //            cell.Value = "Tipo Contacto";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 18];
        //            cell.Value = "Nombre Contacto";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 19];
        //            cell.Value = "Tipo Examen";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 20];
        //            cell.Value = "Fecha Examen";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 21];
        //            cell.Value = "Resultado Examen";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 22];
        //            cell.Value = "";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 23];
        //            cell.Value = "";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 24];
        //            cell.Value = "";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 25];
        //            cell.Value = "Fecha Inicio Sintomas";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 26];
        //            cell.Value = "Fecha Fin Síntomas";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 27];
        //            cell.Value = "Nro Días Sin Sintomas";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 28];
        //            cell.Value = "Fecha Último Día Trabajo";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 29];
        //            cell.Value = "Fecha aislamiento / cuarentena";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 30];
        //            cell.Value = "Fecha Posible Alta 1";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 31];
        //            cell.Value = "N° Días de descanso Médico 1";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 32];
        //            cell.Value = "Fecha Posible Alta 2";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 33];
        //            cell.Value = "N° Días de descanso Médico 2";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 34];
        //            cell.Value = "Fecha Posible Alta 3";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 35];
        //            cell.Value = "N° Días de descanso Médico 3";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 36];
        //            cell.Value = "Fecha Alta Médica";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 37];
        //            cell.Value = "N° Días de descanso tras alta Médica";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            cell = sheet.Cells[rowIndex, 38];
        //            cell.Value = "Médico";
        //            cell.Style.Font.Bold = true;
        //            fill = cell.Style.Fill;
        //            fill.PatternType = ExcelFillStyle.Solid;
        //            fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //            border = cell.Style.Border;
        //            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

        //            #endregion Table header

        //            rowIndex = rowIndex + 1;
        //            foreach (var item in datos)
        //            {
        //                cell = sheet.Cells[rowIndex, 2];
        //                cell.Value = item.ApellidosNombres;

        //                cell = sheet.Cells[rowIndex, 3];
        //                cell.Value = item.Idhmc;

        //                cell = sheet.Cells[rowIndex, 4];
        //                cell.Value = item.Dni;

        //                cell = sheet.Cells[rowIndex, 5];
        //                cell.Value = item.Edad;

        //                cell = sheet.Cells[rowIndex, 6];
        //                cell.Value = item.FechaRegistro;

        //                cell = sheet.Cells[rowIndex, 7];
        //                cell.Value = item.Telefono;

        //                cell = sheet.Cells[rowIndex, 8];
        //                cell.Value = item.Sede;

        //                cell = sheet.Cells[rowIndex, 9];
        //                cell.Value = item.Puesto;

        //                cell = sheet.Cells[rowIndex, 10];
        //                cell.Value = item.DivisionPersonal;

        //                cell = sheet.Cells[rowIndex, 11];
        //                cell.Value = item.CentroCosteArea;

        //                cell = sheet.Cells[rowIndex, 12];
        //                cell.Value = item.ModoIngreso;

        //                cell = sheet.Cells[rowIndex, 13];
        //                cell.Value = item.ViaIngreso;

        //                cell = sheet.Cells[rowIndex, 14];
        //                cell.Value = item.EstadoActual;

        //                cell = sheet.Cells[rowIndex, 15];
        //                cell.Value = item.FechaUltimoDiaTrabajo;

        //                cell = sheet.Cells[rowIndex, 16];
        //                cell.Value = item.AntecedentePatologico;

        //                cell = sheet.Cells[rowIndex, 17];
        //                cell.Value = item.TipoContacto;

        //                cell = sheet.Cells[rowIndex, 18];
        //                cell.Value = item.NombreContacto;

        //                cell = sheet.Cells[rowIndex, 19];
        //                cell.Value = item.TipoExamen;

        //                cell = sheet.Cells[rowIndex, 20];
        //                cell.Value = item.FechaExamen;

        //                cell = sheet.Cells[rowIndex, 21];
        //                cell.Value = item.ResultadoExamen;

        //                cell = sheet.Cells[rowIndex, 22];
        //                cell.Value = "";

        //                cell = sheet.Cells[rowIndex, 23];
        //                cell.Value = "";

        //                cell = sheet.Cells[rowIndex, 24];
        //                cell.Value = "";

        //                cell = sheet.Cells[rowIndex, 25];
        //                cell.Value = item.FechaInicioSintomas;

        //                cell = sheet.Cells[rowIndex, 26];
        //                cell.Value = item.FechanFinSintomas;

        //                cell = sheet.Cells[rowIndex, 27];
        //                cell.Value = item.NroDiasSinSintomas;

        //                cell = sheet.Cells[rowIndex, 28];
        //                cell.Value = item.FechaUltimoDiaTrabajo;

        //                //cell = sheet.Cells[rowIndex, 29];
        //                //cell.Value = item.InicioDescansoMedico;

        //                //cell = sheet.Cells[rowIndex, 30];
        //                //cell.Value = item.NroDiasDescansoMedico;

        //                //cell = sheet.Cells[rowIndex, 31];
        //                //cell.Value = item.NroDiasSegundoDescansoMedico;

        //                cell = sheet.Cells[rowIndex, 29];
        //                cell.Value = item.FechaAislaminetoCuarentena;

        //                cell = sheet.Cells[rowIndex, 30];
        //                cell.Value = item.FechaPosibleAltaA;

        //                cell = sheet.Cells[rowIndex, 31];
        //                cell.Value = item.NroDiasPosibleAltaA;

        //                cell = sheet.Cells[rowIndex, 32];
        //                cell.Value = item.FechaPosibleAltaB;

        //                cell = sheet.Cells[rowIndex, 33];
        //                cell.Value = item.NroDiasPosibleAltaB;

        //                cell = sheet.Cells[rowIndex, 34];
        //                cell.Value = item.FechaPosibleAltaC;

        //                cell = sheet.Cells[rowIndex, 35];
        //                cell.Value = cell.Value = item.NroDiasPosibleAltaC;

        //                cell = sheet.Cells[rowIndex, 36];
        //                cell.Value = item.FechaAltaMedica;

        //                cell = sheet.Cells[rowIndex, 37];
        //                cell.Value = item.NroDiasAltaMedica;

        //                cell = sheet.Cells[rowIndex, 38];
        //                cell.Value = item.MedicoVigila;

        //                rowIndex = rowIndex + 1;
        //            }

        //            return excelPackage.GetAsByteArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //NOTIFICACION DE INGRESO A VIGCOVID  - Creado por Saul Ramos 
        public void GenerarNotificaciondeIngreso(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
           var datosAlta = oReportAltaBL.ObtenerDatosIngreso(id, sessione.IdUser);

            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.CorreosTrabajador))
            {
                datosAlta.CorreosTrabajador = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosChampios))
            {
                datosAlta.CorreosChampios = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreoSede))
            {
                datosAlta.CorreoSede = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            {
                datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosSeguridad))
            {
                datosAlta.CorreosSeguridad = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosMedicoZona))
            {
                datosAlta.CorreosMedicoZona = "administrador@saluslaboris.com.pe";
            }
            if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            {
                datosAlta.CorreosMedicoCoord = "administrador@saluslaboris.com.pe";
            }

            //SmtpClient smtpClient = new SmtpClient


            #region NOTIFICACIÓN  SEGÚN MATRIZ
            MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreoSede + "," + datosAlta.CorreosBP + "," + datosAlta.CorreosChampios + "," + datosAlta.CorreosSeguridad + "," + datosAlta.CorreosMedicoZona + "," + datosAlta.CorreosMedicoCoord)
            {
                Subject = "INGRESO A VIGILANCIA, " + datosAlta.Trabajador,
                IsBodyHtml = true,
                Body = "Se informa mediante la presente que el Sr(a) " + datosAlta.Trabajador + "--ha sido ingresado a Vigilancia" + "<br>" + "<br>" + "Este es un correo electrónico exclusivamente de notificación, por favor no responda este mensaje"
            };

            //SmtpClient smtpClientNotif = new SmtpClient

            SmtpClient smtpClient = new SmtpClient

            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessageNotif);
            #endregion

        }


        public void DATOSVACIOS()
        {
        }
        
        //NOTIFICACION A BP Búsqueda de contactos directos- Creado por Saul Ramos 28-05-2021

        public void GenerarNotificacionContactosDirectos(int id)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oReportAltaBL = new ReportAltaBL();
            var datosAlta = oReportAltaBL.ObtenerDatosContactosDirectos(id, sessione.IdUser);

            var configEmail = new ReportAltaBL().ParametroCorreo();
            string smtp = configEmail[0].v_Value1.ToLower();
            int port = int.Parse(configEmail[1].v_Value1);
            string from = configEmail[2].v_Value1.ToLower();
            string fromPassword = configEmail[4].v_Value1;

            if (string.IsNullOrEmpty(datosAlta.ModoIngreso))
            {
                DATOSVACIOS();
            }

           
            if (string.IsNullOrEmpty(datosAlta.CorreosBP))
            {
                datosAlta.CorreosBP = "administrador@saluslaboris.com.pe";
            }
           
            if (string.IsNullOrEmpty(datosAlta.CorreosMedicoCoord))
            {
                datosAlta.CorreosMedicoCoord = "francisco.pinto@saluslaboris.com.pe";
            }

            //SmtpClient smtpClient = new SmtpClient


            #region NOTIFICACIÓN  SEGÚN MATRIZ
            MailMessage mailMessageNotif = new MailMessage(from, datosAlta.CorreosBP + "," + datosAlta.CorreosMedicoCoord)
            {
                Subject = "CASO QUE REQUIERE IDENTIFICACIÓN DE CONTACTOS", 
                IsBodyHtml = true,
                Body = "Estimada(o) Bussiness Partner:" + "<br>" + "Se hace de su conocimiento que el trabajador" + " " + datosAlta.Trabajador + "<br>" + " ha sido ingresado a la Vigilancia Médica como caso" + datosAlta.ModoIngreso + " "+  datosAlta.ViaIngreso + "<br>" + "por lo que requiere que se identifiquen los trabajadores que cumplen con los criterios de contacto directo establecidos" + "<br>" + "en la RM - 972 - 2020 - MINSA: trabajos a distancia menor de un metro por más de 15 minutos." + "<br>" + "<br>" + "Este es un correo electrónico exclusivamente de notificación, por favor no responda este mensaje"
            };

            //SmtpClient smtpClientNotif = new SmtpClient

            SmtpClient smtpClient = new SmtpClient

            {
                Host = smtp,
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(from, fromPassword)
            };

            smtpClient.Send(mailMessageNotif);
            #endregion

        }

        public byte[] generateExcel(List<ReporteAcumuladoManualBE> datos)
        {
            try
            {
                int rowIndex = 2;
                ExcelRange cell;
                ExcelFill fill;
                Border border;

                using (var excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "SL";
                    excelPackage.Workbook.Properties.Title = "SL";
                    var sheet = excelPackage.Workbook.Worksheets.Add("Reporte Excel");
                    sheet.Name = "Acumulado Covid";

                    sheet.Column(2).Width = 40;
                    sheet.Column(3).Width = 10;
                    sheet.Column(4).Width = 10;
                    sheet.Column(5).Width = 10;
                    sheet.Column(6).Width = 15;
                    sheet.Column(7).Width = 15;
                    sheet.Column(8).Width = 20;
                    sheet.Column(9).Width = 40;
                    sheet.Column(10).Width = 15;
                    sheet.Column(11).Width = 15;

                    sheet.Column(12).Width = 35;
                    sheet.Column(13).Width = 20;
                    sheet.Column(14).Width = 40;
                    sheet.Column(15).Width = 20;
                    sheet.Column(16).Width = 15;
                    sheet.Column(17).Width = 50;
                    sheet.Column(18).Width = 50;
                    sheet.Column(19).Width = 50;
                    sheet.Column(20).Width = 50;
                    sheet.Column(21).Width = 20;

                    sheet.Column(22).Width = 20;
                    sheet.Column(23).Width = 20;
                    sheet.Column(24).Width = 20;
                    sheet.Column(25).Width = 20;
                    sheet.Column(26).Width = 20;
                    sheet.Column(27).Width = 20;
                    sheet.Column(28).Width = 20;
                    //sheet.Column(27).Width = 50;
                    //sheet.Column(28).Width = 50;
                    //sheet.Column(29).Width = 50;
                    //sheet.Column(30).Width = 50;
                    //sheet.Column(31).Width = 50;

                    //sheet.Column(32).Width = 50;
                    //sheet.Column(33).Width = 50;
                    //sheet.Column(34).Width = 50;
                    //sheet.Column(35).Width = 50;
                    //sheet.Column(36).Width = 50;
                    //sheet.Column(37).Width = 50;
                    //sheet.Column(38).Width = 50;
                    //sheet.Column(39).Width = 50;
                    //sheet.Column(40).Width = 50;
                    //sheet.Column(41).Width = 50;

                    //sheet.Column(38).Width = 50;
                    //sheet.Column(39).Width = 50;
                    //sheet.Column(40).Width = 50;

                    #region Report Header

                    sheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
                    cell = sheet.Cells[rowIndex, 2];
                    //cell.Value = "Reporte Acumulado al " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                    cell.Value = "Reporte Acumulado";

                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 20;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowIndex = rowIndex + 1;

                    #endregion Report Header

                    #region Table header

                    cell = sheet.Cells[rowIndex, 2];
                    cell.Value = "TRABAJADOR";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 3];
                    cell.Value = "IDHMC";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 4];
                    cell.Value = "DNI";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 5];
                    cell.Value = "EDAD";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 6];
                    cell.Value = "FECHA REGISTRO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 7];
                    cell.Value = "TELEFONO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 8];
                    cell.Value = "SEDE";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 9];
                    cell.Value = "PUESTO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 10];
                    cell.Value = "DIVISION PERSONAL";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 11];
                    cell.Value = "CENTRO COSTE AREA";
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 12];
                    cell.Value = "MOTIVO INGRESO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 13];
                    cell.Value = "VIA INGRESO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    fill = cell.Style.Fill;
                    
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 14];
                    cell.Value = "ESTADO ACTUAL";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 15];
                    cell.Value = "Fecha Ultimo Dia Trabajo";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                     

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    //cell = sheet.Cells[rowIndex, 16];
                    //cell.Value = "Antecedente Patológico";
                    //cell.Style.Font.Bold = true;
                    //cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //fill = cell.Style.Fill;
                    //fill.PatternType = ExcelFillStyle.LightGray;
                    //fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    //border = cell.Style.Border;
                    //border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 16];
                    cell.Value = "Tipo Contacto";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 17];
                    cell.Value = "Nombre Contacto";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 18];
                    cell.Value = "Tipo Examen";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;

                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 19];
                    cell.Value = "Fecha Examen";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 20];
                    cell.Value = "Resultado Examen";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;


                    cell = sheet.Cells[rowIndex, 21];
                    cell.Value = "TIPO DE RANGO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    
                    cell = sheet.Cells[rowIndex, 22];
                    cell.Value = "FECHA DE INICIO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                                      

                    cell = sheet.Cells[rowIndex, 23];
                    cell.Value = "FECHA DE FIN";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;


                    
                    //Se modifica según cambios en DM - 04082021 - Saul Ramos Vega
                    

                    cell = sheet.Cells[rowIndex, 24];
                    cell.Value = "NRO DE DIAS DESCANSO MEDICO";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    
                    cell = sheet.Cells[rowIndex, 25];
                    cell.Value = "Fecha Alta Médica";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                   
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    



                    cell = sheet.Cells[rowIndex, 26];
                    cell.Value = "MEDICO";
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.LightGray;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    
                    #endregion Table header

                    rowIndex = rowIndex + 1;
                    foreach (var item in datos)
                    {
                        cell = sheet.Cells[rowIndex, 2];


//Worksheets("Sheet1").Range("A17").NumberFormat = "General"
//Worksheets("Sheet1").Rows(1).NumberFormat = "hh:mm:ss"
//Worksheets("Sheet1").Columns("C")._
//NumberFormat = "$#,##0.00_);[Red]($#,##0.00)"


                        cell.Value = item.ApellidosNombres;

                        cell = sheet.Cells[rowIndex, 3];
                        cell.Value = item.Idhmc;

                        cell = sheet.Cells[rowIndex, 4];
                        cell.Value = item.Dni;

                        cell = sheet.Cells[rowIndex, 5];
                        cell.Value = item.Edad;

                        cell = sheet.Cells[rowIndex, 6];
                        cell.Value = item.FechaRegistro;

                        cell = sheet.Cells[rowIndex, 7];
                        cell.Value = item.Telefono;

                        cell = sheet.Cells[rowIndex, 8];
                        cell.Value = item.Sede;

                        cell = sheet.Cells[rowIndex, 9];
                        cell.Value = item.Puesto;

                        cell = sheet.Cells[rowIndex, 10];
                        cell.Value = item.DivisionPersonal;

                        cell = sheet.Cells[rowIndex, 11];
                        cell.Value = item.CentroCosteArea;

                        cell = sheet.Cells[rowIndex, 12];
                        cell.Value = item.ModoIngreso;

                        cell = sheet.Cells[rowIndex, 13];
                        cell.Value = item.ViaIngreso;


                        //Agrgeado por Saul Ramos Vega  -- 04082021
                        cell = sheet.Cells[rowIndex, 14];
                        cell.Value = item.EstadoActual;

                        cell = sheet.Cells[rowIndex, 15];
                        cell.Value = item.FechaUltimoDiaTrabajo;

                        //cell = sheet.Cells[rowIndex, 16];
                        //cell.Value = item.AntecedentePatologico;

                        cell = sheet.Cells[rowIndex, 16];
                        cell.Value = item.TipoContacto;

                        cell = sheet.Cells[rowIndex, 17];
                        cell.Value = item.NombreContacto;

                        cell = sheet.Cells[rowIndex, 18];
                        cell.Value = item.TipoExamen;

                        cell = sheet.Cells[rowIndex, 19];
                        cell.Value = item.FechaExamen;

                        cell = sheet.Cells[rowIndex, 20];
                        cell.Value = item.ResultadoExamen;

                        
                        //Agregado por Saúl Ramos Vega -- 04082021 

                        cell = sheet.Cells[rowIndex, 21];
                        cell.Value = item.DescTipoRango;

                        cell = sheet.Cells[rowIndex, 22];
                        cell.Value = item.FechaInicio.ToString("dd/MM/yyyy");

                        cell = sheet.Cells[rowIndex, 23];
                        cell.Value = item.FechanFin.ToString("dd/MM/yyyy");

                        cell = sheet.Cells[rowIndex, 24];
                        cell.Value = item.NroDiasDescansoMedico;

                        cell = sheet.Cells[rowIndex, 25];
                        cell.Value = item.FechaAltaMedica;

                        //cell = sheet.Cells[rowIndex, 26];
                        //cell.Value = item.FechaUltimoDiaTrabajo;


                        cell = sheet.Cells[rowIndex, 26];
                        cell.Value = item.MedicoVigila;

                        rowIndex = rowIndex + 1;
                    }

                    return excelPackage.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] generateExcelAltas(List<ReporteAcumuladoManualBE> datos)
        {
            try
            {
                int rowIndex = 2;
                ExcelRange cell;
                ExcelFill fill;
                Border border;

                using (var excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "SL";
                    excelPackage.Workbook.Properties.Title = "SL";
                    var sheet = excelPackage.Workbook.Worksheets.Add("Reporte Excel");
                    sheet.Name = "Acumulado Covid";

                    sheet.Column(2).Width = 50;
                    sheet.Column(3).Width = 50;
                    sheet.Column(4).Width = 50;
                    sheet.Column(5).Width = 50;
                    sheet.Column(6).Width = 50;
                    sheet.Column(7).Width = 50;
                    sheet.Column(8).Width = 50;
                    sheet.Column(9).Width = 50;
                    sheet.Column(10).Width = 50;
                    sheet.Column(11).Width = 50;



                    #region Report Header

                    sheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
                    cell = sheet.Cells[rowIndex, 2];
                    cell.Value = "Reporte de altas del día " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 20;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowIndex = rowIndex + 1;

                    #endregion Report Header

                    #region Table header

                    cell = sheet.Cells[rowIndex, 2];
                    cell.Value = "Trabajador";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 3];
                    cell.Value = "Idhmc";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 4];
                    cell.Value = "Dni";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 5];
                    cell.Value = "Edad";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 6];
                    cell.Value = "Fecha Registro";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 7];
                    cell.Value = "Telefono";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 8];
                    cell.Value = "Sede";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 9];
                    cell.Value = "Puesto";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    cell = sheet.Cells[rowIndex, 10];
                    cell.Value = "Divison Personal";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;


                    cell = sheet.Cells[rowIndex, 11];
                    cell.Value = "Fecha Alta Médica";
                    cell.Style.Font.Bold = true;
                    fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    border = cell.Style.Border;
                    border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                    #endregion Table header

                    rowIndex = rowIndex + 1;
                    foreach (var item in datos)
                    {
                        cell = sheet.Cells[rowIndex, 2];
                        cell.Value = item.ApellidosNombres;

                        cell = sheet.Cells[rowIndex, 3];
                        cell.Value = item.Idhmc;

                        cell = sheet.Cells[rowIndex, 4];
                        cell.Value = item.Dni;

                        cell = sheet.Cells[rowIndex, 5];
                        cell.Value = item.Edad;

                        cell = sheet.Cells[rowIndex, 6];
                        cell.Value = item.FechaRegistro;

                        cell = sheet.Cells[rowIndex, 7];
                        cell.Value = item.Telefono;

                        cell = sheet.Cells[rowIndex, 8];
                        cell.Value = item.Sede;

                        cell = sheet.Cells[rowIndex, 9];
                        cell.Value = item.Puesto;

                        cell = sheet.Cells[rowIndex, 10];
                        cell.Value = item.DivisionPersonal;

                        cell = sheet.Cells[rowIndex, 11];
                        cell.Value = item.FechaAltaMedica;

                        rowIndex = rowIndex + 1;
                    }

                    return excelPackage.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


      //  [HttpPost]
      //  public JsonResult ReporteExcelManual(string saul, DateTime FechaInicio, DateTime FechaFin)
      //  {
      //      try
      //      {
      //          var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
      //          var empresasAsignadas = sessione.EmpresasAsignadas;

      //          var empresasCodigos = new List<int>();
      //          foreach (var item in empresasAsignadas)
      //          {
      //              var sedeId = item.EmpresaIdSedeId.Split('-')[1];
      //              empresasCodigos.Add(int.Parse(sedeId));
      //          }

      //          var xobj = saul;
      //          //var listaTrabajadores = ListaTrabajadoresReporte(empresasCodigos, sessione.IdUser).ToList();
      //          var listaTrabajadores = ListaTrabajadoresReporte(empresasCodigos, sessione.EmpresaId, sessione.IdUser, FechaInicio, FechaFin).ToList();
      //          listaTrabajadores = listaTrabajadores.OrderBy(x => x.ApellidosNombres).ToList();

      //          //Response.ClearContent();
      //          //Response.BinaryWrite(generateExcel(listaTrabajadores));
      //          //Response.AddHeader("content-disposition", "attachment; filename=Reporte.xlsx");
      //          //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
      //          //Response.Flush();
      //          //Response.End();


      //          ReporteExcelManualF(listaTrabajadores);


      //          //return Json.stringify(saul, JsonRequestBehavior.AllowGet);
      //          return Json(saul, JsonRequestBehavior.AllowGet);

                

      //      }

           
      //      catch (Exception ex)
      //      {
      //           throw;
                
      //      }
            
      //}





        //public void ReporteExcelManualF(List<ReporteAcumuladoManualBE>reporteAcumuladoManualBEs)
        //{


        //    Response.ClearContent();
        //    Response.BinaryWrite(generateExcel(reporteAcumuladoManualBEs));
        //    Response.AddHeader("content-disposition", "attachment; filename=Reporte.xlsx");
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.Flush();
        //    Response.End();
        //}


        //SE DEHABILITA PARA ENVIO DE DM - - 20210720  Saul Ramos Vega

        public void ReporteExcelManual(string cadena, string FechaInicio, string FechaFin, int EP)
        {
            var FechaFin1 = Convert.ToDateTime(FechaFin);
            var FechaInicio1 = Convert.ToDateTime(FechaInicio);

            try
            {
                var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
                var empresasAsignadas = sessione.EmpresasAsignadas;

                        var empresasCodigos = new List<int>();
                       foreach (var item in empresasAsignadas)
                       {
                            var sedeId = item.EmpresaIdSedeId.Split('-')[1];
                            empresasCodigos.Add(int.Parse(sedeId));
                      }
                                       
                 
                                       
                var xobj = cadena;
                var listaTrabajadores = ListaTrabajadoresReporte(empresasCodigos, EP, sessione.IdUser, FechaInicio1, FechaFin1).ToList();
                
                listaTrabajadores = listaTrabajadores.OrderBy(x => x.ApellidosNombres).ToList();

                       Response.ClearContent();
                       Response.BinaryWrite(generateExcel(listaTrabajadores));
                       Response.AddHeader("content-disposition", "attachment; filename=Reporte.xlsx");
                       Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                       
                       Response.Flush();
                       Response.End();
                   }
                   catch (Exception ex)
                   {
                        throw;
                    }
               }

                //public void ReporteExcelAltasHoy(string cadena)
                //{
                //    try
                //    {
                //        var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
                //        var empresasAsignadas = sessione.EmpresasAsignadas;

                //        var empresasCodigos = new List<int>();
                //        foreach (var item in empresasAsignadas)
                //        {
                //            var sedeId = item.EmpresaIdSedeId.Split('-')[1];
                //            empresasCodigos.Add(int.Parse(sedeId));
                //        }

                //        var xobj = cadena;

                //        var listaTrabajadores = ListaTrabajadoresReporte(empresasCodigos, sessione.EmpresaId,sessione.IdUser).ToList();
                //        listaTrabajadores = listaTrabajadores.OrderBy(x => x.ApellidosNombres).ToList();

                //        listaTrabajadores = listaTrabajadores.Where(x => x.FechaAltaMedica == DateTime.Now.ToString("dd/MM/yyyy")).ToList();

                //        Response.ClearContent();
                //        Response.BinaryWrite(generateExcelAltas(listaTrabajadores));
                //        Response.AddHeader("content-disposition", "attachment; filename=Reporte.xlsx");
                //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //        Response.Flush();
                //        Response.End();
                //    }
                //    catch (Exception ex)
                //    {
                //        throw;
                //    }
                //}

                //#region Private Methodos

                private List<ListaTrabajadoresViewModel> ToTrabajadoresViewModel(List<ListaTrabajadoresBE> trabajadores)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var listaTrabajadores = new List<ListaTrabajadoresViewModel>();
            foreach (var trabajador in trabajadores)
            {
                var oListaTrabajadoresViewModel = new ListaTrabajadoresViewModel();
                oListaTrabajadoresViewModel.RegistroTrabajadorId = trabajador.RegistroTrabajadorId;
                oListaTrabajadoresViewModel.NombreCompleto = trabajador.ApellidosNombres;
                oListaTrabajadoresViewModel.Dni = trabajador.Dni;
                oListaTrabajadoresViewModel.Empresa = trabajador.NombreEmpresa;
                oListaTrabajadoresViewModel.Sede = trabajador.NombreSede;
                oListaTrabajadoresViewModel.Edad = trabajador.Edad;
                oListaTrabajadoresViewModel.FechaIngreso = trabajador.FechaIngreso;
                oListaTrabajadoresViewModel.EstadoDiario = trabajador.EstadoDiario;
                oListaTrabajadoresViewModel.ModoIngreso = trabajador.ModoIngreso;
                oListaTrabajadoresViewModel.ContadorSeguimiento = trabajador.ContadorSeguimiento;
                oListaTrabajadoresViewModel.DiaSinSintomas = trabajador.DiaSinSintomas;
                oListaTrabajadoresViewModel.MedicoVigila = trabajador.MedicoVigila;
                oListaTrabajadoresViewModel.Propietario = trabajador.MedicoVigila == sessione.UserName;
                oListaTrabajadoresViewModel.EstadoClinicoId = trabajador.EstadoClinicoId;
                oListaTrabajadoresViewModel.FechaAlta = trabajador.FechaAlta;
                oListaTrabajadoresViewModel.EmpresaEmpleadora = trabajador.Empleadora;
                oListaTrabajadoresViewModel.EmpresaId = trabajador.EmpresaId;
                oListaTrabajadoresViewModel.TipoEmpresaId = trabajador.TipoEmpresaId;
                if (sessione.AccesoOtrosPacientesLectura)
                {
                    oListaTrabajadoresViewModel.PermisoLectura = true;
                }
                else
                {
                    oListaTrabajadoresViewModel.PermisoLectura = false;
                }
                listaTrabajadores.Add(oListaTrabajadoresViewModel);
            }
            return listaTrabajadores;
        }

        private IEnumerable<ListaTrabajadoresViewModel> ListaTrabajadores(List<int> sedesId, int usuarioId)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oWorkerRegisterBL = new WorkerRegisterBL();

            var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnSeguimiento(sessione.IdUser, sessione.IdTipoUsuario);

            var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

            return listaTrabajadores;
        }

        //private IEnumerable<ReporteAcumuladoManualBE> ListaTrabajadoresReporte(List<int> sedesId, int usuarioId)
        //{
        //    var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
        //    var oWorkerRegisterBL = new WorkerRegisterBL();
        //    var trabajadores = oWorkerRegisterBL.ListarTrabajadoresPorSedesReporteAcumulado(sedesId, usuarioId);

        //    if (sessione.IdTipoUsuario == (int)TipoUsuario.MedicoVigilancia)
        //    {
        //        trabajadores = trabajadores.FindAll(p => p.MedicoVigilaId == usuarioId);
        //    }

        //    return trabajadores;
        //}

        //Se modifica para que filtre por empresa -- Saul RV -- 13052021
        //Se añade opciones de filtro por Fecha de Inicio y Fecha de Fin -- Saul RV -- 20-07-2021
        private IEnumerable<ReporteAcumuladoManualBE> ListaTrabajadoresReporte(List<int> sedesId, int EP, int IdUser, DateTime FechaInicio, DateTime FechaFin)
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var oWorkerRegisterBL = new WorkerRegisterBL();
            var trabajadores = oWorkerRegisterBL.ListarTrabajadoresPorSedesReporteAcumulado(sedesId, EP, FechaInicio, FechaFin);

            if (sessione.IdTipoUsuario == (int)TipoUsuario.MedicoVigilancia)
            {
                trabajadores = trabajadores.FindAll(p => p.MedicoVigilaId == IdUser);
            }

            return trabajadores;
        }



        private int GetNroMonitoring(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).OrderByDescending(o => o.NroSeguimiento).ToList();
            if (seguimientos.Count == 0)
                return 0;

            return seguimientos[0].NroSeguimiento;
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
                return "Cuarentena";
            }
            else if (estadoId == 2)
            {
                return "Hospitalizado";
            }
            else if (estadoId == 3)
            {
                return "Fallecido";
            }
            else if (estadoId == 4)
            {
                return "Aislamiento";
            }
            else
            {
                return "";
            }
        }

        public JsonResult ActualizarGrillaListaTrabajadores(string sedesId, string option)
        {
            var oDashboardBL = new DashboardBL();
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var resultado = new List<ListaTrabajadoresViewModel>();
           


            if (sedesId == "")
                return Json(resultado, JsonRequestBehavior.AllowGet);

            var arrCodigos = sedesId.Split(',');
            var arrint = Array.ConvertAll(arrCodigos, int.Parse).ToList();
            //var lista = ListaTrabajadores(arrint, sessione.IdUser).ToList();

            var oWorkerRegisterBL = new WorkerRegisterBL();

            if (option == "btnProgramadosHoy")
            {
                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnSeguimientoHoy(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);
               
                resultado = listaTrabajadores;
            }
            else if (option == "btnAltasPendientes")
            {
                var trabajadores = new DashboardBL().Lista_AltasPendientes(arrint, sessione.IdUser, sessione.IdTipoUsuario);
                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }
            else if (option == "btnHospitalizados")
            {
                //lista = lista.FindAll(p => p.EstadoClinicoId == null);
                //var trabajadoresIds = oDashboardBL.ListarHospitalizadosHoy(arrint);
                //resultado = lista.FindAll(p => trabajadoresIds.Contains(p.RegistroTrabajadorId));

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresHospitalizadoHoy(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }
            else if (option == "btnModeradosCriticos")
            {
                //lista = lista.FindAll(p => p.EstadoClinicoId == null);
                //var trabajadoresIds = oDashboardBL.ListarModeradosCriticos(arrint);
                //resultado = lista.FindAll(p => trabajadoresIds.Contains(p.RegistroTrabajadorId));

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnModeradoCritico(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }
            else if (option == "btnCuarentena")
            {
                //    lista = lista.FindAll(p => p.EstadoClinicoId == null);
                //    var trabajadoresIds = oDashboardBL.ListarCuarentena(arrint);
                //    resultado = lista.FindAll(p => trabajadoresIds.Contains(p.RegistroTrabajadorId));

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnCuarentena(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }
            else if (option == "btnAltasDadas")
            {
                //var trabajadoresIds = oDashboardBL.ListarAltasDadas(arrint);
                //resultado = lista.FindAll(p => trabajadoresIds.Contains(p.RegistroTrabajadorId));

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnAlta(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }

            else if (option == "btnFallecidos")
            {
                //var trabajadoresIds = oDashboardBL.ListarAltasDadas(arrint);
                //resultado = lista.FindAll(p => trabajadoresIds.Contains(p.RegistroTrabajadorId));

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresFallecidos(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);

                resultado = listaTrabajadores;
            }


            else
            {
                //var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnSeguimiento(sessione.IdUser, sessione.IdTipoUsuario);
                                

                var trabajadores = oWorkerRegisterBL.ObtenerTrabajadoresEnSeguimiento(sessione.IdUser, sessione.IdTipoUsuario);

                var listaTrabajadores = ToTrabajadoresViewModel(trabajadores);
                //return listaTrabajadores;

                resultado = listaTrabajadores;
                             
                              


            }

            resultado = resultado.OrderBy(x => x.NombreCompleto).ToList();
            int index = 1;
            resultado.ToList().ForEach(x =>
            {
                x.Indice = index;
                index += 1;
            });


            return Json(resultado, JsonRequestBehavior.AllowGet);
            

        }

        public JsonResult IndicadoresDashboard(List<int> sedesId, int usuarioId, int tipoUsuarioId)
        {
            var indicadores = new DashboardBL().IndicadoresDashboard(sedesId, usuarioId, tipoUsuarioId);

            return Json(indicadores, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PermisoRegistrarTrabajador()
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            return Json(sessione.AsignarPacientes, JsonRequestBehavior.AllowGet);
        }

        private List<ListarUsuariosViewModels> ListarUsuarios()
        {
            var sessione = (SessionModel)Session[Resources.Constants.SessionUser];
            var lista = new AccessBL().ListarUsuarios();

            var result = new List<ListarUsuariosViewModels>();
            foreach (var item in lista)
            {
                var oListarUsuariosViewModels = new ListarUsuariosViewModels();
                oListarUsuariosViewModels.Id = item.Id;
                oListarUsuariosViewModels.NombreUsuario = item.NombreUsuario;

                if (item.NombreUsuario == sessione.UserName)
                {
                    oListarUsuariosViewModels.Propietario = true;
                }
                else
                {
                    oListarUsuariosViewModels.Propietario = false;
                }
                result.Add(oListarUsuariosViewModels);
            }

            return result;
        }

        //#endregion Private Methodos

        private bool GetNroDiasSinSintomas(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento).ToList();

            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == false
                    && item.Tos == false
                    && item.DolorGarganta == false
                    && item.DificultadRespiratoria == false
                    && item.CongestionNasal == false
                    && item.Cefalea == false
                    && item.MalestarGeneral == false
                    && item.PerdidaOlfato == false)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count >= 7)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetNumeroDiasSinSintomas(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento).ToList();

            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == false
                    && item.Tos == false
                    && item.DolorGarganta == false
                    && item.DificultadRespiratoria == false
                    && item.CongestionNasal == false
                    && item.Cefalea == false
                    && item.MalestarGeneral == false
                    && item.PerdidaOlfato == false)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private int GetNumeroDiasConSintomas(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento).ToList();

            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == true
                    || item.Tos == true
                    || item.DolorGarganta == true
                    || item.DificultadRespiratoria == true
                    || item.CongestionNasal == true
                    || item.Cefalea == true
                    || item.MalestarGeneral == true
                    || item.PerdidaOlfato == true)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private int GetNumeroDiasSinSintomasAcumulado(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento).ToList();

            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == false
                    && item.Tos == false
                    && item.DolorGarganta == false
                    && item.DificultadRespiratoria == false
                    && item.CongestionNasal == false
                    && item.Cefalea == false
                    && item.MalestarGeneral == false
                    && item.PerdidaOlfato == false)
                {
                    count++;
                }
            }

            return count;
        }

        private int GetNumeroDiasConSintomasAcumulado(int registroTrabajadorId)
        {
            var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == registroTrabajadorId).ToList().OrderByDescending(p => p.NroSeguimiento).ToList();

            var count = 0;

            foreach (var item in seguimientos)
            {
                if (item.SensacionFiebre == true
                    || item.Tos == true
                    || item.DolorGarganta == true
                    || item.DificultadRespiratoria == true
                    || item.CongestionNasal == true
                    || item.Cefalea == true
                    || item.MalestarGeneral == true
                    || item.PerdidaOlfato == true)
                {
                    count++;
                }
            }

            return count;
        }

        public JsonResult GetAltas(int Codigo)
        {
            var lista = new List<InfoAltas>();
            var TrabajadoresAltas = (from A in db.RegistroTrabajador
                                     where A.EmpresaId == Codigo && A.EstadoClinicoId == (int)EstadoClinico.alta
                                     select new InfoAltas
                                     {
                                         TrabajadorId = A.Id,
                                         Trabajador = A.NombreCompleto
                                     }).ToList();

            foreach (var item in TrabajadoresAltas)
            {
                var oInfoAltas = new InfoAltas();
                var seguimientos = db.Seguimiento.Where(w => w.RegistroTrabajadorId == item.TrabajadorId).ToList().OrderByDescending(o => o.NroSeguimiento).ToList();
                oInfoAltas.Trabajador = item.Trabajador;
                oInfoAltas.NroSeguimientos = seguimientos[0].NroSeguimiento.ToString();
                oInfoAltas.NroSinSintomas = GetNumeroDiasSinSintomasAcumulado(item.TrabajadorId).ToString();
                oInfoAltas.NroConSintomas = GetNumeroDiasConSintomasAcumulado(item.TrabajadorId).ToString();
                lista.Add(oInfoAltas);
            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarRegistro(int idRegistro)
        {
            WorkerRegisterBL oWorkerRegisterBL = new WorkerRegisterBL();
            var result = oWorkerRegisterBL.RemoveRegister(idRegistro);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchTrabajador(string filter)
        {
            var oWorkerRegisterBL = new WorkerRegisterBL();

            var result = oWorkerRegisterBL.BuscarTrabajador(filter);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrabajadorPorDNI(string dni)
        {
            var oWorkerRegisterBL = new WorkerRegisterBL();

            var result = oWorkerRegisterBL.BuscarTrabajadorPorDni(dni);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllEmpresas()
        {
            var oEmpresaBL = new EmpresaBL();

            var result = oEmpresaBL.GetAllEmpresas();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrabajador(int id)
        {
            var result = new WorkerRegisterBL().GetTrabajadorById(id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        

        //ACTIVARSE REPORTE DE MEDICOS
        public JsonResult TraerIndicadoresMedico(int codigoMedico)
        {
            
            var oTrerIndicadoresM = new DashboardBL();
            List<AltasBE> listadodealtas = oTrerIndicadoresM.AltasMedicoAtendidasHoy(codigoMedico);

            

            //ViewBag.ROLO = listadodealtas;
            return Json(listadodealtas, JsonRequestBehavior.AllowGet);
        }




        public class Indicadores
        {
            public string altasHoy { get; set; }
            public string altasTotal { get; set; }
            public string TotalIgM { get; set; }
            public string TotalIgG { get; set; }
            public string TotalIgMeIgG { get; set; }
        }

        public class InfoAltas
        {
            public int TrabajadorId { get; set; }
            public string Trabajador { get; set; }
            public string NroSeguimientos { get; set; }
            public string NroConSintomas { get; set; }
            public string NroSinSintomas { get; set; }
        }
    }
}