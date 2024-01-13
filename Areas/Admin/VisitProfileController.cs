using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;
using Tangehrine.WebLayer.Common;
using Tangehrine.WebLayer.Models;
using Tangehrine.WebLayer.Utility;
using Microsoft.AspNetCore.Http;

namespace Tangehrine.WebLayer.Areas.Receptionist
{
    [Authorize, Area("Admin")]
    public class VisitProfileController : BaseController<VisitProfileController>
    {
        private readonly IPatientService _patientService;
        private readonly IPatientVisitService _patientVisitService;
        private readonly IPreVisitNoteService _preVisitNoteService;
        private readonly ICommonService _commonService;
        private readonly IHistoryService _historyService;
        private readonly INotyfService _notyf;
        private readonly IIMEService _imeService;
        private readonly IPatientLabReportService _patientLabReportService;
        private readonly ILabReportService _labReportService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IVitalService _vitalService;
        private readonly IPatientMedicineService _patientMedicineService;
        private readonly IMedicineService _medicineService;
        private readonly IPatientStatementTrendsService _statementTrendsService;
        private readonly IPatientCheckupRecordService _patientCheckupRecordService;
        private readonly IPatientMasterService _patientMasterService;
        private readonly IPatientNoteService _patientNoteService;
        public VisitProfileController(IPatientService patientService, IPatientVisitService patientVisitService,
        IPreVisitNoteService preVisitNoteService, ICommonService commonService, IHistoryService historyService,
        INotyfService notyf, IIMEService imeService,
        IPatientLabReportService patientLabReportService, IWebHostEnvironment webHostEnvironment,
        ILabReportService labReportService,
        IVitalService vitalService,
        IPatientMedicineService patientMedicineService, IMedicineService medicineService,
        IPatientMasterService patientMasterService,
        IPatientStatementTrendsService statementTrendsService, IPatientCheckupRecordService patientCheckupRecordService, IPatientNoteService patientNoteService)
        {
            _patientService = patientService;
            _patientVisitService = patientVisitService;
            _preVisitNoteService = preVisitNoteService;
            _commonService = commonService;
            _notyf = notyf;
            _historyService = historyService;
            _imeService = imeService;
            _patientLabReportService = patientLabReportService;
            _labReportService = labReportService;
            _WebHostEnvironment = webHostEnvironment;
            _vitalService = vitalService;
            _patientMedicineService = patientMedicineService;
            _medicineService = medicineService;
            _statementTrendsService = statementTrendsService;
            _patientCheckupRecordService = patientCheckupRecordService;
            _patientMasterService = patientMasterService;
            _patientNoteService=patientNoteService;
        }
        
        public async Task<IActionResult> Index(long Id)
        {
            var names = string.Empty; var name = string.Empty;
            PatientDetailMasterDto patient = new();
            //Presonal details of Patient
            patient.PatientDto = await _patientService.GetPatientById(Id);
            //Patient visit details
            patient.PatientVisitDto = await _patientVisitService.GetVisitByPatientId(Id);
            patient.HistoryDto = await _historyService.GetHistoryByPatientId(Id);
            //Patient vitals
            patient.VitalListDto = await _vitalService.GetVitals(Id);
            patient.MedicineAndPatientList = await _patientMedicineService.GetPatientMedicine(Id);
            patient.PatientMedicineOutput = await _patientMedicineService.GetPatientMedicineById("", 0, Id);
            patient.CheckupRecordsList = await _patientCheckupRecordService.GetPatientCheckupRecord(Id);
            patient.PatientCheckupRecordsOutputDto = await _patientCheckupRecordService.GetPatientCheckupRecordById("", 0, Id);
            return View(patient);
        }
 
        #region Patient visit
        [HttpGet]
        public async Task<IActionResult> AddOrEditVisit(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();
            if (Id > 0)
            {
                data.VisitDto = await _patientVisitService.GetVisitById(Id);
            }
            else
            {
                PatientVisitDto visit = new();
                visit.PatientId = PatientId;
                data.VisitDto = visit;
            }
            return PartialView(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditVisit(PatientDetailMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (input.VisitDto.VisitDate != null)
                    {
                        if (input.VisitDto.VisitDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.VisitDto.VisitDate = CommonMethods.CovertDateFormate(input.VisitDto.VisitDate);
                        }
                    }

                    if (input.VisitDto.Id > 0)
                    {
                        var result = await _patientVisitService.UpdateVisit(input.VisitDto);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.VisitDto.PatientId });
                    }
                    else
                    {
                        var result = await _patientVisitService.AddVisit(input.VisitDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.VisitDto.PatientId });
                    }
                }
                else
                {
                    return PartialView(input);
                }
            }
            catch (Exception ex)
            {

                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);

            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteVisit(long Id)
        {
            try
            {
                var visit = await _patientVisitService.DeletePatientVisit(Id);
                return Json(visit);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        #endregion
        #region Pre visit notes
        public async Task<IActionResult> GetPreVisitNote(JQueryDataTableParamModel param, long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamListPatient(param, GetSortingColumnName(param.iSortCol_0), id).Parameters;
                    var allList = await _patientMasterService.GetPreVisitNote(parameters.ToArray());
                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> _AddOrEditPreVisitNote(long Id, long PatientId)
        {           
            PatientDetailMasterDto data = new();
            //Data of doctor
            data.Doctors = await _commonService.GetUserWithRoleDoctor();
            if (Id > 0)
            {
                data.AddEditPreVisitNoteDto = await _preVisitNoteService.GetPreVisitNoteById(Id);
            }
            else
            {
                PreVisitNoteDto note = new();
                note.PatientId = PatientId;
                data.AddEditPreVisitNoteDto = note;
            }
            return PartialView(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddOrEditPreVisitNote(PatientDetailMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "PreVisitNoteFiles");
                    var exists = Directory.Exists(path);
                    if (input.AddEditPreVisitNoteDto.File != null)
                    {
                        if (exists)
                        {
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PreVisitNote", input.AddEditPreVisitNoteDto.File);
                            input.AddEditPreVisitNoteDto.FileName = filename;
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PreVisitNote", input.AddEditPreVisitNoteDto.File);
                            input.AddEditPreVisitNoteDto.FileName = filename;
                        }

                    }
                    if (input.AddEditPreVisitNoteDto.Date != null)
                    {
                        if (input.AddEditPreVisitNoteDto.Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.AddEditPreVisitNoteDto.Date = CommonMethods.CovertDateFormate(input.AddEditPreVisitNoteDto.Date);
                        }
                    }
                    if (input.AddEditPreVisitNoteDto.Id > 0)
                    {
                        var result = await _preVisitNoteService.UpdatePreVisitNote(input.AddEditPreVisitNoteDto);
                        if (result.IsSuccess && result.Message!="Pre visit note updated")
                        {
                            bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, result.Message);
                            _notyf.Success("Pre visit note updated successfully");
                        }
                        else if(result.IsSuccess)
                        {
                            _notyf.Success(result.Message);
                        }
                        else
                        {
                            _notyf.Error(result.Message);
                        }
                        return RedirectToAction("Index", "VisitProfile", new { id = input.AddEditPreVisitNoteDto.PatientId });
                    }
                    else
                    {
                        var result = await _preVisitNoteService.AddPreVisitNote(input.AddEditPreVisitNoteDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.AddEditPreVisitNoteDto.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Invalid file extensions");
                    return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });
                }

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });
            }

        }

        [HttpPost]
        public async Task<JsonResult> DeletePreVisitNote(long Id)
        {
            try
            {
                var preVisitNote = await _preVisitNoteService.DeletePreVisitNote(Id);
                if (preVisitNote.IsSuccess)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "PreVisitNoteFiles");
                    bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, preVisitNote.Message);
                }
                return Json(preVisitNote);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        [HttpGet]
        public async Task<JsonResult> DownloadPrevisitNote(long Id)
        {
            var report = await _preVisitNoteService.GetPreVisitNoteById(Id);
            if (report != null && report.FileName != null)
            {

                var pathToRead = _WebHostEnvironment.WebRootPath + "\\" + "PreVisitNoteFiles" + "\\" + report.FileName;
                if (!System.IO.File.Exists(pathToRead))
                {
                    _notyf.Error("File does not exist");
                    return null;

                }

                var memory = new MemoryStream();
                await using (var stream = new FileStream(pathToRead, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }

                byte[] bytes = memory.ToArray();
                await memory.DisposeAsync();

                DownloadPdfDto downloadFileDto = new()
                {
                    FileBytes = bytes,
                    FileExtenstion = report.FileName,
                    Base64String = Convert.ToBase64String(bytes)
                };

                return Json(downloadFileDto);
            }
            else
            {
                _notyf.Error("File does not exist");
                return null;
            }
        }
        #endregion
        #region History

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateHistory(PatientDetailMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _historyService.UpdateHistory(input.HistoryDto);
                    _notyf.Success(result.Message);
                    return RedirectToAction("Index", "VisitProfile", new { id = input.HistoryDto.PatientId });
                }
                else
                {
                    return View("Index", input);
                }
            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return View(input);
            }
        }
        #endregion
        #region IMEs
        public async Task<IActionResult> GetIme(JQueryDataTableParamModel param, long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamListPatient(param, GetSortingColumnName(param.iSortCol_0), id).Parameters;
                    var allList = await _patientMasterService.GetIme(parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> _AddOrUpdateIMEs(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();           
            if (Id > 0)
            {
                data.IMEDto = await _imeService.GetIMEById(Id);
            }
            else
            {
                IMEDto ime = new();
                ime.PatientId = PatientId;
                data.IMEDto = ime;
            }

            return PartialView(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddOrUpdateIMEs(PatientDetailMasterDto input)
        {
            try
            {              
                if (ModelState.IsValid)
                {
                    if (input.IMEDto.SurgeryDate != null)
                    {
                        if (input.IMEDto.SurgeryDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.IMEDto.SurgeryDate = CommonMethods.CovertDateFormate(input.IMEDto.SurgeryDate);
                        }
                    }
                    if (input.IMEDto.EKGDate != null)
                    {
                        if (input.IMEDto.EKGDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.IMEDto.EKGDate =  CommonMethods.CovertDateFormate(input.IMEDto.EKGDate);
                        }
                    }
                       
                    if (input.IMEDto.Id > 0)
                    {
                        var result = await _imeService.UpdateIME(input.IMEDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.IMEDto.PatientId });

                    }
                    else
                    {
                        var result = await _imeService.AddIME(input.IMEDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.IMEDto.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Error in validating Form");
                    return PartialView(input);
                }

            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteIMEs(long Id)
        {
            try
            {

                var ime = await _imeService.DeleteIME(Id);
                return Json(ime);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        #endregion
        #region Patient Lab Reports

        public async Task<IActionResult> GetLabReport(JQueryDataTableParamModel param, long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamListPatient(param, GetSortingColumnName(param.iSortCol_0), id).Parameters;
                    var allList = await _patientMasterService.GetLabReport(parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> _AddOrUpdatePatientLabReport(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();
            data.LabReportDto = await _labReportService.GetLabReportList();
            if (Id > 0)
            {
                data.PatientLabReportDto = await _patientLabReportService.GetReportById(Id);
            }
            else
            {
                PatientLabReportDto report = new();
                report.PatientId = PatientId;
                data.PatientLabReportDto = report;
            }
            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> _AddOrUpdatePatientLabReport(PatientDetailMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "LabReports");
                    var exists = Directory.Exists(path);
                    if (input.PatientLabReportDto.Reportfile != null)
                    {
                        if (exists)
                        {
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PatientLabReport", input.PatientLabReportDto.Reportfile);
                            input.PatientLabReportDto.PatientReportPath = filename;
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PatientLabReport", input.PatientLabReportDto.Reportfile);
                            input.PatientLabReportDto.PatientReportPath = filename;
                        }

                    }
                    if (input.PatientLabReportDto.Date != null)
                    {
                        if (input.PatientLabReportDto.Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.PatientLabReportDto.Date = CommonMethods.CovertDateFormate(input.PatientLabReportDto.Date);
                        }
                    }
                 
                    if (input.PatientLabReportDto.Id > 0)
                    {
                        var result = await _patientLabReportService.UpdateReport(input.PatientLabReportDto);
                        if (result.IsSuccess)
                        {
                            bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, result.Message);
                            _notyf.Success("Lab report updated successfully");
                        }
                        else
                        {
                            _notyf.Success(result.Message);
                        }
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });
                    }
                    else
                    {
                        var result = await _patientLabReportService.AddReport(input.PatientLabReportDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Invalid file extensions");
                    return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });                   
                }

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return RedirectToAction("Index", "VisitProfile", new { id = input.PatientLabReportDto.PatientId });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeletePatientLabReport(long Id)
        {
            try
            {
                var report = await _patientLabReportService.DeleteReport(Id);
                if (report.IsSuccess)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "LabReports");
                    bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, report.Message);
                }
                return Json(report);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public async Task<JsonResult> DownloadPdf(long Id)
        {
            var report = await _patientLabReportService.GetReportById(Id);
            if (report != null && report.PatientReportPath != null)
            {

                var pathToRead = _WebHostEnvironment.WebRootPath + "\\" + "LabReports" + "\\" + report.PatientReportPath;
                if (!System.IO.File.Exists(pathToRead))
                {
                    _notyf.Error("File does not exist");
                    return null;

                }

                var memory = new MemoryStream();
                await using (var stream = new FileStream(pathToRead, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }

                byte[] bytes = memory.ToArray();
                await memory.DisposeAsync();

                DownloadPdfDto downloadFileDto = new()
                {
                    FileBytes = bytes,
                    FileExtenstion = report.PatientReportPath,
                    Base64String = Convert.ToBase64String(bytes)
                    //For different types of files,Get it through File.GetExtension()
                };

                return Json(downloadFileDto);
            }

            else
            {
                _notyf.Error("File does not exist");
                return null;
            }

        }
        #endregion
        #region Vitals

        [HttpGet]
        public async Task<IActionResult> _AddOrUpdateVital(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();

            if (Id > 0)
            {
                data.VitalDto = await _vitalService.GetVitalById(Id);
            }
            else
            {
                VitalDto vital = new();
                vital.PatientId = PatientId;
                data.VitalDto = vital;
            }

            return PartialView(data);
        }

        [HttpPost]
        public async Task<IActionResult> _AddOrUpdateVital(PatientDetailMasterDto input)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (input.VitalDto.VitalDate != null)
                    {
                        if (input.VitalDto.VitalDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.VitalDto.VitalDate = CommonMethods.CovertDateFormate(input.VitalDto.VitalDate);
                        }
                    }

                    if (input.VitalDto.Id > 0)
                    {
                        var result = await _vitalService.UpdateVital(input.VitalDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.VitalDto.PatientId });
                    }
                    else
                    {
                        var result = await _vitalService.AddVital(input.VitalDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.VitalDto.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Error in validating Form");
                    return PartialView(input);
                }

            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteVital(long Id)
        {
            try
            {

                var vital = await _vitalService.DeleteVital(Id);
                return Json(vital);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }


        #endregion
        #region Patient medicine

        [HttpGet]
        public async Task<IActionResult> _AddOrUpdatePatientMedicine(string Date, long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();
            if (Date != null)
            {
                if (Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                {
                    Date = CommonMethods.CovertDateFormate(Date);
                }
            }
            var medicineData = await _medicineService.GetMedicineList();
            var medicineDoseTimeData = await _medicineService.GetMedicineDoseTimeList();
            if (medicineData.Count != 0)
            {
                ViewBag.IsMedicineEmpty = false;
            }
            else
            {
                ViewBag.IsMedicineEmpty = true;
            }

            if (Id > 0)
            {
                var result = await _patientMedicineService.GetPatientMedicineById(Date, Id, PatientId);

                PatientMedicineOutputDto patientDetailMaster = new();
                patientDetailMaster.Id = result.Id;
                patientDetailMaster.PatientId = PatientId;
               //patientDetailMaster.MedicineId = result.MedicineId;
                patientDetailMaster.MedicineDate = result.MedicineDate;
               //patientDetailMaster.Dose = result.Dose;
                patientDetailMaster.patientMedicineDoseDtos = result.patientMedicineDoseDtos;
                patientDetailMaster.MedicineList = medicineData.Select(x => new SelectListItem()
                {
                    Text = x.MedicineName,
                    Value = x.Id.ToString()
                }).OrderBy(x => x.Text);

                patientDetailMaster.MedicineDoseTimeList = medicineDoseTimeData.Select(x => new SelectListItem()
                {
                    Text = x.DoseTime,
                    Value = x.Id.ToString()
                }).OrderBy(x => x.Text);
                data.PatientMedicineOutput = patientDetailMaster;
            }
            else
            {
                PatientMedicineOutputDto patientDetailMaster = new();
                patientDetailMaster.Id = Id;
                patientDetailMaster.PatientId = PatientId;
                patientDetailMaster.patientMedicineDoseDtos = new();
                patientDetailMaster.MedicineList = medicineData.Select(x => new SelectListItem()
                {
                    Text = x.MedicineName,
                    Value = x.Id.ToString()
                }).OrderBy(x => x.Text);
                patientDetailMaster.MedicineDoseTimeList = medicineDoseTimeData.Select(x => new SelectListItem()
                {
                    Text = x.DoseTime,
                    Value = x.Id.ToString()
                }).OrderBy(x => x.Text);
                data.PatientMedicineOutput = patientDetailMaster;
            }
            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> _AddOrUpdatePatientMedicine(PatientMedicineOutputDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<PatientMedicineDto> PatientMedicinList = new();
                    if (input.MedicineDate != null)
                    {
                        if (input.MedicineDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.MedicineDate = CommonMethods.CovertDateFormate(input.MedicineDate);
                        }
                    }

                    if (input.Id > 0)
                    {
                        //var result = await _patientMedicineService.UpdatePatientMedicine(input);
                        //_notyf.Success(result.Message);

                        return RedirectToAction("Index", "VisitProfile", new { id = input });
                    }
                    else
                    {
                        if (input.patientMedicineDoseDtos !=null && input.patientMedicineDoseDtos.Count > 0)
                        {
                            var isMedicineExist = await _patientMedicineService.GetPatientMedicineById(input.MedicineDate, input.MedicineId, input.PatientId);
                            if (isMedicineExist.patientMedicineDoseDtos == null)
                            {
                                foreach (var item in input.patientMedicineDoseDtos.Distinct())
                                {
                                    PatientMedicineDto medicineDto = new();
                                    medicineDto.MedicineDate = DateTime.Parse(input.MedicineDate);
                                    medicineDto.PatientId = input.PatientId.Value;
                                    medicineDto.MedicineId = item.MedicineId;
                                    medicineDto.DoseTimeId = item.DoseTimeId;
                                    medicineDto.Dose = item.Dose;
                                    PatientMedicinList.Add(medicineDto);
                                }
                                var result = await _patientMedicineService.AddPatientMedicine(PatientMedicinList);
                                _notyf.Success(result.Message);
                            }
                            else
                            {
                                if (isMedicineExist.patientMedicineDoseDtos.Count > 0)
                                {
                                    List<PatientMedicineDto> PatientMedicin= new();
                                    string mDate = input.MedicineDate;

                                    //add
                                    var getNewmedicineEditTime= input.patientMedicineDoseDtos.Where(x => x.Id == null).ToList();
                                    var isMadicineEditTime = getNewmedicineEditTime.Where(c => isMedicineExist.patientMedicineDoseDtos.All(w => w.MedicineId != c.MedicineId)).ToList();
             
                                    if (isMadicineEditTime.Count > 0) 
                                    {
                                        foreach (var i in isMadicineEditTime)
                                        {                                           
                                            PatientMedicineDto medicineDto = new();
                                            medicineDto.MedicineDate = DateTime.Parse(mDate);
                                            medicineDto.PatientId = input.PatientId.Value;
                                            medicineDto.MedicineId = i.MedicineId;
                                            medicineDto.DoseTimeId = i.DoseTimeId;
                                            medicineDto.Dose = i.Dose;
                                            PatientMedicin.Add(medicineDto); 

                                        } 
                                    }
                                
                                    //update
                                    foreach (var patient in isMedicineExist.patientMedicineDoseDtos)
                                    {

                                        PatientMedicineDto medicineDto = new();
                                        input.MedicineDate = isMedicineExist.MedicineDate;
                                        medicineDto.Dose = input.patientMedicineDoseDtos.Where(x=>x.Id==patient.Id).Select(x=>x.Dose).FirstOrDefault();
                                        medicineDto.Id = patient.Id;
                                        medicineDto.PatientId = input.PatientId.Value;
                                        medicineDto.MedicineId = patient.MedicineId;
                                        medicineDto.DoseTimeId = input.patientMedicineDoseDtos.Where(x => x.Id == patient.Id).Select(x => x.DoseTimeId).FirstOrDefault();
                                        PatientMedicinList.Add(medicineDto);                                            
                                    }
                                    
                                    if (PatientMedicin.Count > 0)
                                    {
                                        await _patientMedicineService.AddPatientMedicine(PatientMedicin);
                                    }
                                    var result = await _patientMedicineService.UpdatePatientMedicine(PatientMedicinList);
                                    _notyf.Success(result.Message);

                                 
                                }  
                            }
                        }
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientId });
                    }

                }
                else
                {
                    _notyf.Error("Error in validating Form");
                    return PartialView(input);
                }

            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);
            }
        }

        [HttpPost]
        public async Task<decimal> GetDose(long id, DateTime date, long patientId)
        {
            var dose = await _patientMedicineService.GetDoseByIdAndDate(id, date, patientId);
            return dose;
        }

        [HttpPost]
        public async Task<JsonResult> DeletePatientMedicine(string Date, long PatientId)
        {
            try
            {
                if (Date != null)
                {
                    if (Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                    {
                        Date = CommonMethods.CovertDateFormate(Date);
                    }
                }
                var patientMedicine = await _patientMedicineService.DeletePatientMedicine(Date, PatientId);
                return Json(patientMedicine);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        //[HttpGet]
        //public async Task UpdateMedicineList(string list)
        //{
        //    ViewBag.MedicineList = list;
        //}
        #endregion
        #region Statistics 

        [HttpGet]
        public async Task<JsonResult> GetDataForStatistics(long PatientId)
        {
            var result = await _patientMedicineService.GetDateWiseMedicineAndDose(PatientId);
            return new JsonResult(result) { StatusCode = 200 };
        }
        #endregion
        #region Patient Statement Trends
        public async Task<IActionResult> GetPatientStatement(JQueryDataTableParamModel param, long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamListPatient(param, GetSortingColumnName(param.iSortCol_0), id).Parameters;
                    var allList = await _patientMasterService.GetPatientStatement(parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> _AddOrEditPatientStatementTrends(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();
            if (Id > 0)
            {
                data.PatientStatementTrendsDto = await _statementTrendsService.GetPatientStatementTrendsById(Id);
            }
            else
            {
                PatientStatementTrendsDto patientStatement = new();
                patientStatement.PatientId = PatientId;
                data.PatientStatementTrendsDto = patientStatement;
            }

            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> _AddOrEditPatientStatementTrends(PatientDetailMasterDto input)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    if (input.PatientStatementTrendsDto.Id > 0)
                    {
                        var result = await _statementTrendsService.UpdatePatientStatementTrends(input.PatientStatementTrendsDto);
                        _notyf.Success(result.ToString());
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientStatementTrendsDto.PatientId });
                    }
                    else
                    {
                        input.PatientStatementTrendsDto.Count = 1;
                        var result = await _statementTrendsService.AddPatientStatementTrends(input.PatientStatementTrendsDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientStatementTrendsDto.PatientId });
                    }
                }
                else
                {
                    return PartialView(input);
                }

            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeletePatientStatementTrends(long Id)
        {
            try
            {

                var preStatementTrends = await _statementTrendsService.DeletePatientStatementTrends(Id);
                return Json(preStatementTrends);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        public async Task<List<PatientStatementTrendsDto>> GetStatement(long patientId, string statement)
        {
            List<PatientStatementTrendsDto> lst = new();
            lst = await _statementTrendsService.GetPatientStatementTrendsForSearch(patientId, statement);

            return lst;
        }
        #endregion
        #region Patient Checkup Record

        [HttpGet]
        public async Task<IActionResult> _AddOrUpdatePatientCheckupRecord(string Date, long Id, long PatientId)
        {
            try
            {
                PatientDetailMasterDto data = new();
                var labReports = await _labReportService.GetLabReportList();
                if (labReports.Count != 0)
                {
                    ViewBag.IsLabReportsEmpty = false;
                }
                else
                {
                    ViewBag.IsLabReportsEmpty = true;
                }
                if (Date != null || Date=="")
                {
                    if (Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                    {
                       Date = CommonMethods.CovertDateFormate(Date);
                    }
                }
                if (Id > 0)
                {
                    var result = await _patientCheckupRecordService.GetPatientCheckupRecordById(Date, Id, PatientId);

                    PatientCheckupRecordsOutputDto patientCheckupRecords = new();
                    patientCheckupRecords.Id = result.Id;
                    patientCheckupRecords.PatientId = PatientId;
                    patientCheckupRecords.CheckupDate = result.CheckupDate;
                    patientCheckupRecords.PatientCheckupObservationDtos = result.PatientCheckupObservationDtos;
                    patientCheckupRecords.LabReportList = labReports.Select(x => new SelectListItem()
                    {
                        Text = x.LabReportName,
                        Value = x.Id.ToString()
                    }).OrderBy(x => x.Text);
                    data.PatientCheckupRecordsOutputDto = patientCheckupRecords;
                }
                else
                {
                    PatientCheckupRecordsOutputDto patientCheckupRecords = new();
                    patientCheckupRecords.Id = Id;
                    patientCheckupRecords.PatientId = PatientId;
                    patientCheckupRecords.LabReportId = 0;
                    patientCheckupRecords.PatientCheckupObservationDtos = new();
                    patientCheckupRecords.LabReportList = labReports.Select(x => new SelectListItem()
                    {
                        Text = x.LabReportName,
                        Value = x.Id.ToString()
                    }).OrderBy(x => x.Text);
                    data.PatientCheckupRecordsOutputDto = patientCheckupRecords;
                }

                return PartialView(data);
            }
            catch (Exception ex)
            {
                PatientDetailMasterDto data = new();
                return PartialView(data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> _AddOrUpdatePatientCheckupRecord(PatientCheckupRecordsOutputDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<PatientCheckupRecordsDto> patientCheckupRecords = new();
                    if (input.CheckupDate != null)
                    {
                        if (input.CheckupDate.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.CheckupDate = CommonMethods.CovertDateFormate(input.CheckupDate);
                        }
                    }
                  
                    if (input.Id > 0)
                    {
                        return RedirectToAction("Index", "VisitProfile", new { id = input });
                    }
                    else
                    {
                        if (input.PatientCheckupObservationDtos.Count > 0)
                        {
                            var isRecordExist = await _patientCheckupRecordService.GetPatientCheckupRecordById(input.CheckupDate, input.LabReportId, input.PatientId);
                            if (isRecordExist.PatientCheckupObservationDtos == null)
                            {
                                foreach (var item in input.PatientCheckupObservationDtos)
                                {
                                    PatientCheckupRecordsDto checkupRecordsDto = new();
                                    checkupRecordsDto.CheckupDate = DateTime.Parse(input.CheckupDate);
                                    checkupRecordsDto.PatientId = input.PatientId.Value;
                                    checkupRecordsDto.LabReportId = item.LabReportId;
                                    checkupRecordsDto.Observations = item.Observations;
                                    patientCheckupRecords.Add(checkupRecordsDto);
                                }
                                var result = await _patientCheckupRecordService.AddPatientCheckupRecord(patientCheckupRecords);
                                _notyf.Success(result.ToString());
                            }
                            else
                            {
                                if (isRecordExist.PatientCheckupObservationDtos.Count > 0)
                                {
                                    foreach (var i in input.PatientCheckupObservationDtos)
                                    {
                                        PatientCheckupRecordsDto checkupRecordsDto = new();
                                        checkupRecordsDto.Observations = i.Observations;
                                        checkupRecordsDto.Id = i.Id;
                                        checkupRecordsDto.PatientId = input.PatientId.Value;
                                        checkupRecordsDto.LabReportId = i.LabReportId;
                                        patientCheckupRecords.Add(checkupRecordsDto);
                                    }
                                }
                                var result = await _patientCheckupRecordService.UpdatePatientCheckupRecord(patientCheckupRecords);
                                _notyf.Success(result.Message);
                            }
                        }
                        return RedirectToAction("Index", "VisitProfile", new { id = input.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Error in validating Form");
                    return PartialView(input);
                }

            }
            catch (Exception ex)
            {
                input.Response.IsSuccess = false;
                input.Response.Message = ex.Message;
                _notyf.Error(input.Response.Message);
                return PartialView(input);
            }
        }

        [HttpPost]
        public async Task<string> GetObservations(long id, DateTime date, long patientId)
        {
            var Observations = await _patientCheckupRecordService.GetObservationsByIdAndDate(id, date, patientId);
            return Observations;
        }

        [HttpPost]
        public async Task<JsonResult> DeletePatientCheckupRecord(string Date, long PatientId)
        {
            try
            {
                if (Date != null)
                {
                    if (Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                    {
                        Date = CommonMethods.CovertDateFormate(Date);
                    }
                }
                var patientObservations = await _patientCheckupRecordService.DeletePatientCheckupRecord(Date, PatientId);
                return Json(patientObservations);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        #endregion
        #region Patient Notes

        public async Task<IActionResult> GetNotes(JQueryDataTableParamModel param, long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamListPatient(param, GetSortingColumnName(param.iSortCol_0), id).Parameters;
                    var allList = await _patientMasterService.GetNotes(parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> _AddOrUpdatePatientNotes(long Id, long PatientId)
        {
            PatientDetailMasterDto data = new();
            if (Id > 0)
            {
                data.patientNotesDto = await _patientNoteService.GetNoteById(Id);
            }
            else
            {
                PatientNotesDto notes = new();
                notes.PatientId = PatientId;
                data.patientNotesDto = notes;
            }
            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> _AddOrUpdatePatientNotes(PatientDetailMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "NoteFiles");
                    var exists = Directory.Exists(path);
                    if (input.patientNotesDto.NotesFile != null)
                    {
                        if (exists)
                        {
                            string filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PatientNote", input.patientNotesDto.NotesFile);
                            input.patientNotesDto.Note = filename;
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            string filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "PatientNote", input.patientNotesDto.NotesFile);
                            input.patientNotesDto.Note = filename;
                        }

                    }
                    if (input.patientNotesDto.Date != null)
                    {
                        if (input.patientNotesDto.Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.patientNotesDto.Date = CommonMethods.CovertDateFormate(input.patientNotesDto.Date);
                        }
                    }

                    if (input.patientNotesDto.Id > 0)
                    {
                        var result = await _patientNoteService.UpdateNote(input.patientNotesDto);
                        if (result.IsSuccess)
                        {
                            bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, result.Message);
                            _notyf.Success("Note updated successfully");
                        }
                        else
                        {
                            _notyf.Error(result.Message);
                        }
                        return RedirectToAction("Index", "VisitProfile", new { id = input.patientNotesDto.PatientId });
                    }
                    else
                    {
                        var result = await _patientNoteService.AddNote(input.patientNotesDto);
                        _notyf.Success(result.Message);
                        return RedirectToAction("Index", "VisitProfile", new { id = input.patientNotesDto.PatientId });
                    }
                }
                else
                {
                    _notyf.Error("Invalid file extensions");
                    return RedirectToAction("Index", "VisitProfile", new { id = input.patientNotesDto.PatientId });
                }

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message);
                return RedirectToAction("Index", "VisitProfile", new { id = input.patientNotesDto.PatientId });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeletePatientNote(long Id)
        {
            try
            {
                var result = await _patientNoteService.DeleteNote(Id);
                if (result.IsSuccess)
                {
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "NoteFiles");
                    bool status = CommonMethods.DeleteFile(_WebHostEnvironment.WebRootPath, path, result.Message);
                }
                return Json(result);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public async Task<JsonResult> DownloadNotes(long Id)
        {
            var report = await _patientNoteService.GetNoteById(Id);
            if (report != null && report.Note != null)
            {

                var pathToRead = _WebHostEnvironment.WebRootPath + "\\" + "NoteFiles" + "\\" + report.Note;
                if (!System.IO.File.Exists(pathToRead))
                {
                    _notyf.Error("File does not exist");
                    return null;

                }

                var memory = new MemoryStream();
                await using (var stream = new FileStream(pathToRead, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }

                byte[] bytes = memory.ToArray();
                await memory.DisposeAsync();

                DownloadPdfDto downloadFileDto = new()
                {
                    FileBytes = bytes,
                    FileExtenstion = report.Note,
                    Base64String = Convert.ToBase64String(bytes)
                };

                return Json(downloadFileDto);
            }
            else
            {
                _notyf.Error("File does not exist");
                return null;
            }

        }
        #endregion
    }
}
