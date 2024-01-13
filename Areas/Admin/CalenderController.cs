using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tangehrine.ServiceLayer;
using Tangehrine.ServiceLayer.Dtos;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Enums;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.ServiceLayer.Utility;
using Tangehrine.WebLayer.Common;
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class CalenderController : Controller
    {

        private readonly ICalendarService _calendarService;
        private readonly IPatientService _patientService;
        private readonly ICommonService _commonService;
        private readonly INotyfService _notyf;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IErrorLogService _errorLogService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public CalenderController(ICalendarService calendarService, INotyfService notyfService,
            IPatientService patientService, ICommonService commonService, IErrorLogService errorLogService, IConfiguration config, IOptions<EmailSettingsGmail> emailSettingsGmail, IWebHostEnvironment WebHostEnvironment)
        {
            _calendarService = calendarService;
            _notyf = notyfService;
            _patientService = patientService;
            _config = config;
            _emailService = new EmailService(emailSettingsGmail);
            _commonService = commonService;
            _errorLogService = errorLogService;
            _WebHostEnvironment = WebHostEnvironment;
        }


        //public IActionResult Index()
        //{
        //    //try
        //    //{
        //    //UserCredential credential;

        //    using (var stream =
        //        new FileStream("wwwroot/Utility/credentials.json", FileMode.Open, FileAccess.Read))
        //    {
        //        // The file token.json stores the user's access and refresh tokens, and is created
        //        // automatically when the authorization flow completes for the first time.
        //        string credPath = "wwwroot/Utility/token.json";
        //        ClientSecrets sec = new ClientSecrets()
        //        {
        //            ClientId = "2831048579-r4p00egclvjsh9o56pjacvuuhq7f922k.apps.googleusercontent.com",
        //            ClientSecret = "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"
        //        };
        //        //OAuth2Parameters parameters = new OAuth2Parameters()
        //        //{
        //        //    ClientId = clientId,
        //        //    ClientSecret = clientSecret,
        //        //    RedirectUri = redirectUri,
        //        //    Scope = Scopes[0],
        //        //};
        //        //parameters.AccessCode = code;
        //        //OAuthUtil.GetAccessToken(parameters);

        //        //credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        //    //sec,
        //        //    //Scopes,
        //        //    parameters,
        //        //    "usertest",
        //        //    CancellationToken.None,
        //        //    new FileDataStore(credPath, true)).Result;
        //        //Console.WriteLine("Credential file saved to: " + credPath);
        //    }
        //    //    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //    //           new ClientSecrets
        //    //           {
        //    //               ClientId = "2831048579-r4p00egclvjsh9o56pjacvuuhq7f922k.apps.googleusercontent.com",
        //    //               ClientSecret = "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"
        //    //           }, new string[] { CalendarService.Scope.CalendarReadonly },
        //    //            "user",
        //    //            CancellationToken.None).Result;
        //    //    // Create Google Calendar API service.
        //    //   var service = new CalendarService(new BaseClientService.Initializer()
        //    //    {
        //    //        HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential,
        //    //        ApplicationName = ApplicationName,
        //    //    });

        //    //    // Define parameters of request.
        //    //    EventsResource.ListRequest request = service.Events.List("primary");
        //    //    request.TimeMin = DateTime.Now;
        //    //    request.ShowDeleted = false;
        //    //    request.SingleEvents = true;
        //    //    request.MaxResults = 10;
        //    //    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        //    //    // List events.
        //    //    Events events = request.Execute();
        //    //    Console.WriteLine("Upcoming events:");
        //    //    if (events.Items != null && events.Items.Count > 0)
        //    //    {
        //    //        foreach (var eventItem in events.Items)
        //    //        {
        //    //            string when = eventItem.Start.DateTime.ToString();
        //    //            if (String.IsNullOrEmpty(when))
        //    //            {
        //    //                when = eventItem.Start.Date;
        //    //            }
        //    //            Console.WriteLine("{0} ({1})", eventItem.Summary, when);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        Console.WriteLine("No upcoming events found.");
        //    //    }
        //    //    Console.Read();

        //    //}
        //    //catch(Exception ex)
        //    //{
        //    //}
        //    return View();
        //}
        public async Task<IActionResult> Index()
        {
            //    try
            //    {
            //        string[] Scopes = { CalendarService.Scope.CalendarReadonly };
            //        string ApplicationName = "Tangerine Calender Application";

            //        UserCredential credential;

            //        var cs = new ClientSecrets()
            //        {
            //           // ClientId = "623723676551-8c27kehjfn2pkhjuvnucfct7kj7lk611.apps.googleusercontent.com",
            //            ClientId="64058399290-e5a4iolmia12ulcthrmg2rvb3ruteq3e.apps.googleusercontent.com",
            //            ClientSecret= "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"
            //            //ClientSecret = "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"
            //        };

            //        // The file token.json stores the user's access and refresh tokens, and is created
            //        // automatically when the authorization flow completes for the first time.
            //        string credPath = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "token.json");
            //        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(cs, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));

            //        // Create Google Calendar API service.
            //        Service = new CalendarService(new BaseClientService.Initializer()
            //        {
            //            HttpClientInitializer = credential,
            //            ApplicationName = ApplicationName,
            //        });

            //        // Define parameters of request.
            //        EventsResource.ListRequest request = Service.Events.List("primary");
            //        request.TimeMin = DateTime.Now;
            //        request.ShowDeleted = false;
            //        request.SingleEvents = true;
            //        request.MaxResults = 10;
            //        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            //        // List events.
            //        Events events = request.Execute();
            //        if (events.Items != null && events.Items.Count > 0)
            //        {
            //            foreach (var eventItem in events.Items)
            //            {
            //                string when = eventItem.Start.DateTime.ToString();
            //                if (string.IsNullOrEmpty(when))
            //                {
            //                    when = eventItem.Start.Date;
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            CalendarDto data = new();
            data.Patients = await _calendarService.GetPatientList();
            data.Doctors = await _commonService.GetUserWithRoleDoctor();
            return PartialView(data);

        }

        [HttpGet]
        public async Task<List<CalendarDto>> GetCalender()
        {
            return await _calendarService.GetCalendar();

        }

        [HttpGet]
        public async Task<IActionResult> _AddOrUpdateCalendar(long Id)
        {
            CalendarDto data = new();
            if (Id > 0)
            {
                data = await _calendarService.GetCalendarById(Id);
            }
            data.Patients = await _calendarService.GetPatientList();
            data.Doctors = await _commonService.GetUserWithRoleDoctor();
            return PartialView(data);
        }

        [HttpPost]
        public async Task<IActionResult> _AddOrUpdateCalendar(CalendarDto calendarDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dateSting = calendarDto.Date;
                    if (calendarDto.Date != null)
                    {
                        if (calendarDto.Date.Contains("-") && HttpContext.Request.Host.ToString().Contains(GlobalConstant.localhost.ToString()) || HttpContext.Request.Host.ToString().Contains(GlobalConstant.clientapp.ToString()))
                        {
                            calendarDto.Date = CommonMethods.CovertDateFormate(calendarDto.Date);
                        }
                    }
                    ServiceResponse response = new ServiceResponse();
                    if (calendarDto.Id > 0)
                    {

                        var appointmentExist = await _calendarService.GetCalendarById(Convert.ToInt32(calendarDto.Id));

                        if (appointmentExist.Date == dateSting && appointmentExist.StartTime == calendarDto.StartTime && appointmentExist.EndTime == calendarDto.EndTime)
                        {
                            response = await _calendarService.UpdateCalendar(calendarDto);
                        }
                        else
                        {
                            response = await _calendarService.UpdateCalendar(calendarDto);
                            await Sendemail(response, calendarDto);
                        }
                    }
                    else
                    {
                        response = await _calendarService.AddCalendar(calendarDto);
                        await Sendemail(response, calendarDto);
                    }

                    if (response.IsSuccess == true)
                    {
                        _notyf.Success(response.Message);
                    }
                    else
                    {
                        _notyf.Error(response.Message);
                    }
                    return RedirectToAction("Index", "Calender");
                }             
                else
                {
                    CalendarDto data = new CalendarDto();             
                    return View(data);
                }
            }       
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                _notyf.Error("Error in validating Form");
                return PartialView("Index", "Calender");
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteCalendar(long Id)
        {
            try
            {
                var calendar = await _calendarService.DeleteCalendar(Id);
                return Json(calendar);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public async Task Sendemail(ServiceResponse response, CalendarDto calendarDto)
        {
            var patientsList = await _calendarService.GetPatientList();
            var patient = patientsList.Where(x => x.Id == calendarDto.PatientId).FirstOrDefault();

            string ActionUrl = _config.GetValue<string>("CommonProperty:ActionUrl");
            string Actionaccept = _config.GetValue<string>("CommonProperty:ActionUrlAcceptReject");
            string Actionreject = _config.GetValue<string>("CommonProperty:ActionUrlAcceptReject");
            Actionaccept += "?/yes/" + CommonMethods.EncryptData(calendarDto.PatientId + "," + response.JsonObj, GlobalConstant.EncryptionCode);
            Actionreject += "?/not/" + CommonMethods.EncryptData(calendarDto.PatientId + "," + response.JsonObj, GlobalConstant.EncryptionCode);
                    

            string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLogService, _WebHostEnvironment.WebRootPath + "\\" + "EmailTemplate", EmailTemplateFileList.ConfirmationAppointment, ActionUrl);
            //Conversion of object type to Dto type
            emailTemplate = emailTemplate.Replace("{UserName}", patient.FullName);
            emailTemplate = emailTemplate.Replace("{action_urlaccept}", Actionaccept);
            emailTemplate = emailTemplate.Replace("{action_urlreject}", Actionreject);
            emailTemplate = emailTemplate.Replace("{date}", Convert.ToDateTime(calendarDto.Date).ToString("MM/dd/yyyy"));
            emailTemplate = emailTemplate.Replace("{start}", calendarDto.StartTime);
            emailTemplate = emailTemplate.Replace("{end}", calendarDto.EndTime);

            await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
            {
                ToAddress = patient.Email,
                Subject = "Confirm Appointment",
                BodyText = emailTemplate
            });
        }
    }
}
