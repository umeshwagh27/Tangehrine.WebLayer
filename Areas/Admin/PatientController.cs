using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tangehrine.ServiceLayer;
using Tangehrine.ServiceLayer.Dtos;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IRelationshipService _relationShipService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly INotyfService _notyf;
        public PatientController(IPatientService patientService, IRelationshipService relationShipService,
            IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _patientService = patientService;
            _WebHostEnvironment = webHostEnvironment;
            _relationShipService = relationShipService;
            _notyf = notyf;
        }

        #region Methods
        public async Task<IActionResult> Index(string GlobalFilter, int PageIndex)
        {
            if (GlobalFilter == null) ViewBag.GlobalFilter = "";
            PaginationPatient paginationPatient = await _patientService.GetPatientList(GlobalFilter, PageIndex);
            ViewBag.GlobalFilter = GlobalFilter;
            return View("PatientList", paginationPatient);
        }
        [HttpGet]
        public async Task<IActionResult> _AddOrEditPatient(long Id)
        {
            try
            {
                PatientMasterDto data = new PatientMasterDto();
                data.patientDto = new PatientDto();
                if (Id > 0)
                {
                    data.patientDto = await _patientService.GetPatientById(Id);
                }
                //Get data for dropdown of Relationship
                data.relationshipDtos = await _relationShipService.GetRelationshipList();
                return View(data);
            }
            catch (Exception ex)
            {
                PatientMasterDto data = new PatientMasterDto();
                data.patientDto = new PatientDto();
                data.response = new ServiceResponse();
                data.response.IsSuccess = false;
                data.response.Message = "Error in opening Patient form";
                _notyf.Error(data.response.Message);
                List<RelationshipDto> relationshipDtos = new List<RelationshipDto>();
                return View(data);
            }
        }
        [HttpPost]
        public async Task<IActionResult> _AddOrEditPatient(PatientMasterDto input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (input.patientDto.BirthDateString != null)
                    {
                        if (input.patientDto.BirthDateString.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            input.patientDto.BirthDateString = CommonMethods.CovertDateFormate(input.patientDto.BirthDateString);
                        }
                    }
                    var path = Path.Combine(_WebHostEnvironment.WebRootPath, "PatientImages");
                    var exists = Directory.Exists(path);
                    var emailExist = _patientService.CheckEmailExistOrNot(input.patientDto.Email);

                    if (input.patientDto.PatientPhoto != null)
                    {
                        if (exists)
                        {
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "Patient", input.patientDto.PatientPhoto);
                            input.patientDto.PatientPhotoUrl = filename;
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, path, "Patient", input.patientDto.PatientPhoto);
                            input.patientDto.PatientPhotoUrl = filename;
                        }

                    }                  
                    if (input.patientDto.Id > 0)
                    {                        
                        var patientData = await _patientService.UpdatePatient(input.patientDto);
                        _notyf.Success(patientData.Message);
                        return RedirectToAction("Index", "Patient");
                    }
                    else
                    {
                        if (emailExist == false)
                        {
                            _notyf.Error("Email alredy Exist");
                        }
                        else
                        {
                            var patientData = await _patientService.AddPatient(input.patientDto);
                            _notyf.Success(patientData.Message);
                        }
                        return RedirectToAction("Index", "Patient");
                    }
                }
                else
                {
                    PatientMasterDto data = new PatientMasterDto();
                    data.patientDto = new PatientDto();
                    data.response = new ServiceResponse();
                    data.response.IsSuccess = false;
                    data.response.Message = "Invalid Patient Form";
                    _notyf.Error(data.response.Message);
                    //Get data for dropdown of Relationship
                    data.relationshipDtos = await _relationShipService.GetRelationshipList();
                    return View(data);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("msg", ex.Message);
                PatientMasterDto data = new PatientMasterDto();
                data.patientDto = new PatientDto();
                data.response = new ServiceResponse();
                data.response.IsSuccess = false;
                data.response.Message = ex.Message;
                _notyf.Error(data.response.Message);
                List<RelationshipDto> relationshipDtos = new List<RelationshipDto>();
                return View(data);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeletePatient(long patientId)
        {
            try
            {
                var patient = await _patientService.DeletePatient(patientId);
                return Json(patient);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        [HttpGet]
        public async Task<IActionResult> _SharePatient(long patientId)
        {
            List<TherapistHelperModel> patientDto = new();
            try
            {
                ViewBag.Id = patientId;
                patientDto = await _patientService.GetDoctorList(patientId);
                return View(patientDto);
            }
            catch (Exception)
            {
                return View(patientDto);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddEditPatientToShare(long[] doctor,long patientId)
        {
            List<TherapistDto> therapistDtos = new();
            List<TherapistDto> editTherapistDtos = new();
            for (int i=0;i<doctor.Length;i++)
            {               
                var patientExist = await _patientService.GetTherapistExistById(doctor[i],patientId);
                if(patientExist.Id !=null)
                {
                    var response = await _patientService.EditTherapist(patientExist);                
                }
                else
                {
                    TherapistDto data = new();
                    data.Doctor = doctor[i];
                    data.PatientId = patientId;
                    therapistDtos.Add(data);
                }             
            }     
            var result=await _patientService.AddTherapist(therapistDtos);
            _notyf.Success(result.Message);
            return View();         
        }

        [HttpGet]
        public async Task<IActionResult> PatientReportDownload(long id)
        {
            var response = new ServiceResponse();
            try
            {
                List<PatientReportHelperModel> patientModels = new();
                if (id > 0)
                {                
               
                    var url = $"{this.Request.Scheme}://{this.Request.Host}";
                    var headerHtmlForDownload = "<!DOCTYPE html> <html dir='ltr' lang='en-US'> <head><title>Patient Report | TangEHRine</title><link type='image/x-icon' rel='shortcut icon' href='../assets/images/favicon.png' /> <!-- Required meta tags -->  <meta charset='UTF-8' /> <meta name='HandheldFriendly' content='true' />         <meta name='viewport' content='width=device-width, initial-scale=1.0' />          <link rel='preconnect' href='https://fonts.googleapis.com' />         <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />         <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700;800;900&display=swap' rel='stylesheet' />          <style type='text/css'>             /*********************  Default-CSS  *********************/              input[type='file']::-webkit-file-upload-button {                 cursor: pointer;             }              input[type='file']::-moz-file-upload-button {                 cursor: pointer;             }              input[type='file']::-ms-file-upload-button {                 cursor: pointer;             }              input[type='file']::-o-file-upload-button {                 cursor: pointer;             }              input[type='file'],             a[href],             input[type='submit'],             input[type='button'],             input[type='image'],             label[for],             select,             button,             .pointer {                 cursor: pointer;             }              ::-moz-focus-inner {                 border: 0px solid transparent;             }              ::-webkit-focus-inner {                 border: 0px solid transparent;             }              *::-moz-selection {                 color: #fff;                 background: #000;             }              *::-webkit-selection {                 color: #fff;                 background: #000;             }              *::-webkit-input-placeholder {                 color: #333333;                 opacity: 1;             }              *:-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *::-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *:-ms-input-placeholder {                 color: #333333;                 opacity: 1;             }             html {                 font-size: 16px;             }             html body {                 font-family: 'Poppins', sans-serif;                 margin: 0;                 line-height: 1.5;                 font-size: 16px;             }              a,             div a:hover,             div a:active,             div a:focus,             button {                 text-decoration: none;                 -webkit-transition: all 0.5s ease 0s;                 -moz-transition: all 0.5s ease 0s;                 -ms-transition: all 0.5s ease 0s;                 -o-transition: all 0.5s ease 0s;                 transition: all 0.5s ease 0s;             }              a,             span,             div a:hover,             div a:active,             button {                 text-decoration: none;             }              *::after,             *::before,             * {                 -webkit-box-sizing: border-box;                 -moz-box-sizing: border-box;                 -ms-box-sizing: border-box;                 -o-box-sizing: border-box;                 box-sizing: border-box;             }              .no-list li,             .no-list ul,             .no-list ol,             footer li,             footer ul,             footer ol,             header li,             header ul,             header ol {                 list-style: inside none none;             }              .no-list ul,             .no-list ol,             footer ul,             footer ol,             header ul,             header ol {                 margin: 0;                 padding: 0;             }              a {                 outline: none;                 color: #555;             }              a:hover {                 color: #000;             }              body .clearfix,             body .clear {                 clear: both;                 line-height: 100%;             }              body .clearfix {                 height: auto;             }              * {                 outline: none !important;             }              table {                 border-collapse: collapse;                 border-spacing: 0;             }              ul:after,             li:after,             .clr:after,             .clearfix:after,             .container:after,             .grve-container:after {                 clear: both;                 display: block;                 content: '';             }              div input,             div select,             div textarea,             div button {                 font-family: 'Poppins', sans-serif;             }              body h1,             body h2,             body h3,             body h4,             body h5,             body h6 {                 font-family: 'Poppins', sans-serif;                 line-height: 1.5;                 color: #333;                 font-weight: 600;                 margin: 0 0 15px;             }              body h1:last-child,             body h2:last-child,             body h3:last-child,             body h4:last-child,             body h5:last-child,             body h6:last-child {                 margin-bottom: 0;             }              .h2,             h2 {                 font-size: 1.7rem;             }              div select {                 overflow: hidden;                 text-overflow: ellipsis;                 white-space: nowrap;             }              img,             svg {                 margin: 0 auto;                 max-width: 100%;                 max-height: 100%;                 width: auto;                 height: auto;                 vertical-align: top;             }              body p {                 margin: 0 0 25px;                 padding: 0;             }              body p:empty {                 margin: 0;                 line-height: 0;             }              body p:last-child {                 margin-bottom: 0;             }              p strong {                 font-weight: 600;             }              strong {                 font-weight: 600;             }              .a-left {                 text-align: left;             }              .a-right {                 text-align: right;             }              .a-center {                 text-align: center;             }              .hidden {  display: none !important; } body .container .container { width: 100%; max-width: 100%; }              /*********************  Default-CSS close  *********************/    .back-btn {width: 48px;fill: #fff;margin-right: auto;} .back-btn:hover, .action-btns:hover {opacity: 0.5; }   .action-btns { background: transparent;border: none;height: 30px;fill: #fff;padding: 0 25px;} main {padding:50px;}footer img{width:100%;vertical-align: top;}</style> </head><body>";
                    patientModels = await _patientService.GetPatientReport(id);
                    var medicine = await _patientService.GetPatientMedicine(id);
                    var vital = await _patientService.GetVitals(id);
                    var checkRecoerd = await _patientService.GetPatientCheckupRecord(id);
                    var chekupData = "";
                    //chekup record Dynamic Table
                    for (var i = 0; i < checkRecoerd.GetLength(0); i++)
                    {
                        chekupData += "<tr>";
                        for(var j = 0; j < checkRecoerd.GetLength(1); j++)
                        {
                            if(checkRecoerd[i, j] == null)
                            {
                                chekupData += "<td style='word-break:break-all;border: 1px solid #000;padding: 5px;'> - </td>";
                            }
                            else
                            {
                                chekupData += "<td style='word-break:break-all;border: 1px solid #000; padding: 5px;'>";
                                chekupData += checkRecoerd[i, j];
                                chekupData += "</td>";
                            }
                        }
                        chekupData += "</tr>";
                    }           
                    var medicineData = "";
                    //medicine Dynamic Table
                        medicineData += "<tr><td style='padding:10px;'><table cellpadding='0' cellspacing='0' style='width:100%;'>";
                    for (var i = 0; i < medicine.GetLength(0); i++)
                    {
                        medicineData += "<tr>";
                        for (var j = 0; j < medicine.GetLength(1); j++)
                        {
                            if (medicine[i, j] == null)
                            {
                                medicineData += "<td  style='word-break:break-all;border: 1px solid #000;padding: 5px;'>-</td>";
                            }
                            else
                            {
                                medicineData += "<td  style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                                medicineData += medicine[i, j];
                                medicineData+= "</td>";
                            }
                        }
                        medicineData += "</tr>";
                    }
                        medicineData += "</table></td></tr>";
                    //Vital Dynamic Table
                    var vitalData = "";
                    if(vital.Count > 0)
                    {

                        foreach (var item in vital)
                        {

                            vitalData += "<td style='word-break:break-all;'><table cellpadding='0' cellspacing='0' style='width:100%;'>";
                            vitalData += "<thead><tr><th style='padding: 5px;border: 1px solid #000;'>";
                            vitalData += item.VitalDate != null ? item.VitalDate : "-";
                            vitalData += "</th></tr></thead><tbody><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.Systolic != null ? item.Systolic : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.Diastolic != null ? item.Diastolic : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.HeartRate != null ? item.HeartRate : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.Weight != null ? item.Weight : "-";
                            vitalData += "</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.Height != null ? item.Height : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.BMI != null ? item.BMI : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.BMIRange != null ? item.BMIRange : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.PainScore != null ? item.PainScore : "-";
                            vitalData += "</td></tr>";
                            vitalData += "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>";
                            vitalData += item.Source != null ? item.Source : "-";
                            vitalData += "</td></tr></tbody></table></td>";
                        }
                    }
                    //middle body
                    var middleHtmlForDownload = "";
                    foreach (var item in patientModels)
                    {
                        if (item.PId != 0)
                        {
                            if (item.NextOfKin != null)
                            {
                                middleHtmlForDownload = "<main>" + "<center style='padding-bottom:20px;'>" +
                                "<img src= '" + url + "/assets/images/logo.png' alt='logo' />"
                                + "<br/><h3>A PSYCHIATRIC CLINIC</h3>" + "DOSSIER : <u>" + item.FullName + "</u>" + "</br>DOB :" + item.BirthDateString
                                + "</br> ADDRESS:" + item.Address + "</br>  [+91]" + item.Mobile + "***" + item.Email + "</br> CONTACT :" + item.NextOfKin + "[" + item.RelationShipName + "] [+91]" + item.RelativePhoneNo + "</center>";
                            }
                            else
                            {
                                middleHtmlForDownload = "<main>" + "<center style='padding-bottom:20px;'>" +
                               "<img src= '" + url + "/assets/images/logo.png' alt='logo' />"
                               + "<br/><h3>A PSYCHIATRIC CLINIC</h3>" + "DOSSIER : <u>" + item.FullName + "</u>" + "</br>DOB :" + item.BirthDateString
                               + "</br> ADDRESS:" + item.Address + "</br>  [+91]" + item.Mobile + "***" + item.Email + "</center>";
                            }
                            
                        }
                    }
                    //footer html
                    var footerHtmlForDownload = "<table  cellpadding='0' cellspacing='0' style='width: 100%;'><tbody><tr><td style='padding:50px 0 20px;'><img src='" + url + "/assets/images/signature.png' alt='signature' /></td></tr><tr><td><strong>Sohrab Zahedi , M.D.</strong></td></tr><tr><td>Doctor</td></tr></tbody></table></main>   <footer style='height: 148px;'>             <img  src='" + url + "/assets/images/file-footer.png' alt='file-footer'/> </footer> </body> </html>";
                    var data = "";
                    foreach (var item in patientModels)
                    {
                        if (item.VId != 0)
                        {
                            data = "<tr>"
                                       + "<td style='border: 1px solid #000;width:50%;padding:5px;'>" + item.VisitDay
                                       + "</td>"
                                       + "<td style='border: 1px solid #000;width:50%;padding:5px;'>" + item.VisitDate
                                       + "</td>"
                                       + "</tr>"
                                       + "<tr>"
                                       + "<td style='border: 1px solid #000;width:50%;padding:5px;'>" + item.FromTime
                                       + "</td>"
                                       + "<td style='border: 1px solid #000;width:50%;padding:5px;'>" + item.ToTime
                                       + "</td>"
                                     + "</tr>";                            
                        }
                    }
                    var data1 = "";               
                    foreach(var item in patientModels)
                    {
                        if (item.PrId != 0)
                        {
                            data1 += " <tr><td colspan='3' style='padding: 0 10px 18px;'><table cellpadding='0' cellspacing='0' style='width:100%;'><tbody><tr><td style='word-break:break-all;padding:5px;border: 1px solid #000;width:30%;'>" + item.Date + " " + item.Time + "</td><td style='word-break:break-all;padding:5px;border: 1px solid #000;width:70%;'>" + item.DoctorName + "</td></tr><tr><td colspan='3' style='word-break:break-all;padding:5px;border: 1px solid #000;'>" + item.Remark + "</td></tr></tbody></table></td></tr>";

                                

                            
                        }   
                    }
                    var history = "";
                    foreach(var item in patientModels)
                    {
                        if(item.HId!=0)
                        {
                            history = "<p>" + item.HistoryNote + "</p>";
                        }
                    }
                    var pdfString = "";
                   
                    
                    //Main HTML
                    var htmlContent = headerHtmlForDownload + middleHtmlForDownload +

                                "<table cellpadding='0' cellspacing='0' style='width:100%;border: 1px solid #000;'>"
                                + "<tr>"

                                + "<td colspan='3' align='center'>"
                                        + "<table cellpadding='0' cellspacing='0' style='width: 50%;'>"
                                            + "<tr>"
                                                + "<td colspan='2' style='border: 1px solid #000;border-top:0;padding:5px;'>"
                                                        + "<center style='font-weight: 600;font-size: 18px; '>Tele-Clinic Visit" + "</center><p id='demo'></p>"
                                                + "</td>"
                                            + "</tr>"
                                            + data
                                        + "</table>" 
                                + "</td>"
                              + "</tr>"
                              + "<tr> <td colspan='3'>" + "<u style='font-weight: 600;padding:5px;'>Pre-Visit Notes: </u>" + "</td></tr>"
                                 + data1
                            + "<tr><td colspan='3' style='word-break:break-all;border: 1px solid #000;padding:5px;'><u style='font-weight: 600;padding-bottom: 4px;'>Patient History</u></td><tr>"
                            + "<tr >"
                                + "<td colspan='3' style='padding:5px;'>" + history + "</td>"
                               + "</tr>"
                            + "</table><br>"
                            + "<table cellpadding='0' cellspacing='0' style='width:100%;border: 1px solid #000;'><tr><td style='padding:10px;'><u style='font-weight: 600;padding-bottom: 4px;'>Medicine:</u></td></tr> " + medicineData + "</table><br>"

                            + "<table cellpadding='0' cellspacing='0' style='width:100%;border: 1px solid #000;'><tr><td style='padding:10px;'><u style='font-weight: 600;padding-bottom: 4px;'>Vital:</u></td></tr><tr><td style='padding:10px;'>" +
                            "<table style='width:100%'><tr><td style='word-break:break-all;'>" +
                            "<table cellpadding='0' cellspacing='0' style='width:100%;'><thead><tr><th style='padding: 5px;border: 1px solid #000;'>Date</th></tr></thead><tbody><tr>"
                                    + "<td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>Systolic</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;' >Diastolic</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>HeartRate</td></tr>"
                                    + "<tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>Weight</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>Height</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>BMI</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>BMI Range</td>"
                                    + "</tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>Pain Score</td></tr><tr><td style='word-break:break-all;border: 1px solid #000;padding: 5px;'>Source</td></tr></tbody></table>"
                                    + vitalData
                                    + "</tr>"
                           + "</table></td></tr></table><br>"
                           
                            + "<table cellpadding='0' cellspacing='0' style='width:100%;border: 1px solid #000;'><tr><td style='padding:10px;'><u style='font-weight: 600;padding-bottom: 4px;'>Checkup Record:</u></td></tr>" +
                            "<tr><td style='padding: 10px;'><table  cellpadding='0' cellspacing='0' style='width:100%'>"
                              + chekupData

                             + "</table></td></tr></table>"


                        + footerHtmlForDownload;
                    HtmlToPdf converter = new HtmlToPdf();
                    PdfDocument doc = converter.ConvertHtmlString(htmlContent);       
                    pdfString = Convert.ToBase64String(doc.Save());
                    return Json(new { message = pdfString });
                }
                return null;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return null;
            }
        }
        #endregion
    }
}
