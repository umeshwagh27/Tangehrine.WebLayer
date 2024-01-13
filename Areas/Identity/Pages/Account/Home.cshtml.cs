using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tangehrine.DataLayer.DbContext;
using Microsoft.AspNetCore.Http;
using Tangehrine.DataLayer;
using Microsoft.EntityFrameworkCore;
using Tangehrine.WebLayer.Common;
using Tangehrine.ServiceLayer.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.ServiceLayer.Utility;
using Tangehrine.ServiceLayer.Enums;
using Tangehrine.ServiceLayer.Interface.Admin;
using Microsoft.Extensions.Options;
using Tangehrine.DataLayer.Model.Admin;
using Microsoft.AspNetCore.Authorization;

namespace Tangehrine.WebLayer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class HomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeModel> _logger;
        private readonly TangehrineDbContext _db;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly IErrorLogService _errorLogService;
        private readonly ICalendarService _calendarService;
        private readonly ICommonService _commonService;
        public HomeModel(SignInManager<ApplicationUser> signInManager,
            ILogger<HomeModel> logger,
            TangehrineDbContext db, IWebHostEnvironment WebHostEnvironment, IConfiguration config,
             IErrorLogService errorLogService, UserManager<ApplicationUser> userManager,
             ICalendarService calendarService, IOptions<EmailSettingsGmail> emailSettingsGmail, ICommonService commonService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _config = config;
            _WebHostEnvironment = WebHostEnvironment;
            _errorLogService = errorLogService;
            _emailService = new EmailService(emailSettingsGmail);
            _calendarService = calendarService;
            _commonService = commonService;
        }
        public void OnGet()
        {

        }
        //public async Task OnGetAsync14(string? returnUrl = null)
        //{
        //    //var requestPath = HttpContext.Request.QueryString.Value;
        //    //if (requestPath != "")
        //    //{
        //    //    var queryString = requestPath.Split("/");
        //    //    if (queryString[1] != null && queryString[2] != null && queryString[3] != null)
        //    //    {


        //    //        var patient = await _db.Patients.Where(x => x.Id == Convert.ToUInt32(queryString[2])).FirstOrDefaultAsync();
        //    //        var doctor = await _db.Users.Where(u => u.Email == queryString[3].ToString() && u.IsActive == true).FirstOrDefaultAsync();
        //    //        var calendars = await _db.Calenders.Where(x => x.IsActive == true && x.IsDelete == false && x.DoctorId == doctor.Id && x.PatientId == patient.Id).FirstOrDefaultAsync();

        //    //        string physicalUrl = _config.GetValue<string>("CommonProperty:PhysicalUrl");
        //    //        string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLogService, _WebHostEnvironment.ContentRootPath + "\\" + "Views", EmailTemplateFileList.PatientConfirmAppointment, physicalUrl);
        //    //        //emailTemplate = emailTemplate.Replace("{email}", doctor.FullName);
        //    //        emailTemplate = emailTemplate.Replace("{physicalUrl}", physicalUrl);
        //    //        emailTemplate = emailTemplate.Replace("{PatientEmail}", patient.FirstName + " " + patient.LastName);
        //    //        emailTemplate = emailTemplate.Replace("{UserName}", doctor.FullName);
        //    //        emailTemplate = emailTemplate.Replace("{date}", calendars.Date.ToString("MM/dd/yyyy"));
        //    //        emailTemplate = emailTemplate.Replace("{start}", calendars.StartTime);
        //    //        emailTemplate = emailTemplate.Replace("{end}", calendars.EndTime);
        //    //        emailTemplate = emailTemplate.Replace("{response}", queryString[1].ToString() == "yes" ? "confirmed" : "rejected");
        //    //        await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
        //    //        {
        //    //            ToAddress = "ugw@narola.email",//queryString[3].ToString(),
        //    //            Subject = "Appointment Confirmation",
        //    //            BodyText = emailTemplate
        //    //        });

        //    //        Calender calendar = new Calender();
        //    //        if (calendars != null)
        //    //        {
        //    //            calendars.ModifiedBy = Convert.ToInt64(_commonService.GetCurrentLoggedInUserId());
        //    //            calendars.ModifiedDate = DateTime.Now;
        //    //            calendars.Id = calendars.Id;
        //    //            if (queryString[1].ToString() == "yes")
        //    //            {
        //    //                calendars.ThemeColor = "Green";
        //    //                calendars.Title = "Confirmed";
        //    //            }
        //    //            else
        //    //            {
        //    //                calendars.ThemeColor = "Red";
        //    //                calendars.Title = "Rejected";
        //    //            }
        //    //            calendars.IsConfirmAppointment = true;
        //    //            _db.Calenders.Update(calendars);
        //    //            await _db.SaveChangesAsync();
                       
        //    //        }
        //    //    }
        //    //}
        //}
    }
}
