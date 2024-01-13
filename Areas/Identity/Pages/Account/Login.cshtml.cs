using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tangehrine.DataLayer.DbContext;
using Microsoft.AspNetCore.Http;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.DataLayer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
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
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly TangehrineDbContext _db;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly IErrorLogService _errorLogService;
        private readonly ICalendarService _calendarService;
        private readonly ICommonService _commonService;
        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            TangehrineDbContext db,IWebHostEnvironment WebHostEnvironment,IConfiguration config,
             IErrorLogService errorLogService,UserManager<ApplicationUser> userManager,
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

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
           
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            var requestPath = HttpContext.Request.QueryString.Value;
     
            if (requestPath != "")    
            {
                if (requestPath.Contains("yes") || requestPath.Contains("not"))
                {
                    var queryString1 = requestPath.Remove(0, 6);
                    var decryptQuery = CommonMethods.DecryptData(queryString1, GlobalConstant.EncryptionCode);
                    var queryString = requestPath.ToString().Split("/");
                    var decryptedQuery = decryptQuery.Split(",");

                    if (decryptedQuery[0] != null && decryptedQuery[1] != null && decryptedQuery[2] != null)
                    {
                        var patient = await _db.Patients.Where(x => x.Id == Convert.ToUInt32(decryptedQuery[0])).FirstOrDefaultAsync();
                        var doctor = await _db.Users.Where(u => u.Email == decryptedQuery[1].ToString() && u.IsActive == true).FirstOrDefaultAsync();
                        var date = decryptedQuery[2].ToString().Split("%")[0];
                        var calendars = await _db.Calenders.Where(x => x.IsActive == true && x.IsDelete == false && x.DoctorId == doctor.Id && x.PatientId == patient.Id && x.Date == Convert.ToDateTime(date)).FirstOrDefaultAsync();
                        if (calendars.Title == "Not confirm")
                        {
                            string physicalUrl = _config.GetValue<string>("CommonProperty:PhysicalUrl");
                            string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLogService, _WebHostEnvironment.WebRootPath + "\\" + "EmailTemplate", EmailTemplateFileList.PatientConfirmAppointment, physicalUrl);

                            //emailTemplate = emailTemplate.Replace("{email}", doctor.FullName);
                            emailTemplate = emailTemplate.Replace("{physicalUrl}", physicalUrl);
                            emailTemplate = emailTemplate.Replace("{PatientEmail}", patient.FirstName + " " + patient.LastName);
                            emailTemplate = emailTemplate.Replace("{UserName}", doctor.FullName);
                            emailTemplate = emailTemplate.Replace("{date}", calendars.Date.ToString("MM/dd/yyyy"));
                            emailTemplate = emailTemplate.Replace("{start}", calendars.StartTime);
                            emailTemplate = emailTemplate.Replace("{end}", calendars.EndTime);
                            emailTemplate = emailTemplate.Replace("{response}", queryString[1].ToString() == "yes" ? "confirmed" : "Canceled");
                            await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                            {
                                ToAddress = decryptedQuery[1].ToString(),
                                Subject = "Appointment Confirmation",
                                BodyText = emailTemplate
                            }); 


                            Calender calendar = new Calender();
                            if (calendars != null)
                            {
                                calendars.ModifiedBy = Convert.ToInt64(_commonService.GetCurrentLoggedInUserId());
                                calendars.ModifiedDate = DateTime.Now;
                                calendars.Id = calendars.Id;
                                if (queryString[1].ToString() == "yes")
                                {
                                    calendars.ThemeColor = "Green";
                                    calendars.Title = "Confirmed";
                                }
                                else
                                {
                                    calendars.ThemeColor = "Red";
                                    calendars.Title = "Canceled";
                                }
                                _db.Calenders.Update(calendars);
                                await _db.SaveChangesAsync();
                            }

                            TempData["Request"] = queryString[1].ToString() == "yes" ? "We are proccessing your request for Confirmation" : "We are proccessing your request for Cancelation";
                            TempData["Home"] = "";
                        }
                        else
                        {
                            TempData["Home"] = "You'r response is alredy submited.[Appointment Note: If you canceled you'r appointment please contact to doctor for reschedule.]";
                        }
                    }
                }
            }


            var cookieEmail = Request.Cookies["UserName"];
            var cookiePassword = Request.Cookies["Password"];
            var cookieRemember = Request.Cookies["RememberMe"];
            if (cookieEmail != null && cookiePassword != null)
            {
                byte[] email = Convert.FromBase64String(cookieEmail);
                byte[] password = Convert.FromBase64String(cookiePassword);
                byte[] remeber = Convert.FromBase64String(cookieRemember);

                ViewData["UserName"]= Encoding.UTF8.GetString(email);
                ViewData["Password"] = Encoding.UTF8.GetString(password);
                ViewData["RememberMe"] = Encoding.UTF8.GetString(remeber);
            }
           
          
            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //await _db.Users.Where(x=>x.UserName==Input.Email && x.Password==Convert.ToInt16(Input.Password)).FirstOrDefaultAsync();
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    CookieOptions cookieOptions = new CookieOptions();

                                 
                    string email= Convert.ToBase64String(Encoding.UTF8.GetBytes(Input.Email));
                    string password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Input.Password));
                    string rememberMe= Convert.ToBase64String(Encoding.UTF8.GetBytes(Input.RememberMe.ToString()));

                    if (Input.RememberMe)
                    {
                        cookieOptions.Expires = DateTime.Now.AddMinutes(30);
                        Response.Cookies.Append("UserName", email, cookieOptions);
                        Response.Cookies.Append("Password", password, cookieOptions);
                        Response.Cookies.Append("RememberMe",rememberMe, cookieOptions);

                    }
                    else
                    {
                        cookieOptions.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Delete("UserName");
                        Response.Cookies.Delete("Password");
                        Response.Cookies.Delete("RememberMe");
                    }
                    var user = await _userManager.Users.Where(x => x.Email == Input.Email && x.IsActive == true && x.IsDelete == false).FirstOrDefaultAsync();
                    var userAccessDetail = await _db.UserAccesses.Where(x => x.UserId == user.Id && x.IsActive==true && x.IsDelete == false).FirstOrDefaultAsync();


                    var sharePatientDetail = await (from urole in _db.UserRoles
                                                   join role in _db.Roles on urole.RoleId equals role.Id
                                                   join users in _db.Users on urole.UserId equals users.Id
                                                   where users.IsActive && !users.IsDelete && users.Email == user.Email
                                                   select new DesignationDto
                                                   {
                                                       ShareAllPatientDetails= role.ShareAllPatientDetails 

                                                   }).ToListAsync();
                    if (sharePatientDetail != null) {

                        foreach (var item in sharePatientDetail)
                        {
                            HttpContext.Session.SetString("ShareAllPatientDetails", item.ShareAllPatientDetails.ToString());
                        }
                    }            

                    //Storing values in session
                    HttpContext.Session.SetString("FirstName", user.FirstName);
                    if(userAccessDetail != null)
                    {
               
                        HttpContext.Session.SetString("IsAllowForMedicineMaster", userAccessDetail.IsAllowForMedicineMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForPatientList", userAccessDetail.IsAllowForPatientList.ToString());
                        HttpContext.Session.SetString("IsAllowForPatientDetails", userAccessDetail.IsAllowForPatientDetails.ToString());
                        HttpContext.Session.SetString("IsAllowForDesignationMaster", userAccessDetail.IsAllowForDesignationMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForLabReportMaster", userAccessDetail.IsAllowForLabReportMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForUserMaster", userAccessDetail.IsAllowForUserMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForRelatioshipMaster", userAccessDetail.IsAllowForRelatioshipMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForLetterFormatMaster", userAccessDetail.IsAllowForLetterFormatMaster.ToString());
                        HttpContext.Session.SetString("IsAllowForUserAccessMaster", userAccessDetail.IsAllowForUserAccessMaster.ToString());
                        HttpContext.Session.SetString("IsAllowToAppoint", userAccessDetail.IsAllowToAppoint.ToString());
                        HttpContext.Session.SetString("IsAllowForTodoList", userAccessDetail.IsAllowForTodoList.ToString());
                    }
                                 
                    if (user.ProfileImageUrl != null)
                    {
                        HttpContext.Session.SetString("ProfileImageUrl", user.ProfileImageUrl);
                    }
                    else
                    {
                        HttpContext.Session.SetString("ProfileImageUrl", "../assets/images/avatar-3.png");
                    }                   


                    if (user != null && await _userManager.CheckPasswordAsync(user, Input.Password))
                    {
                        _logger.LogInformation("User logged in.");
                        if (returnUrl != "/")
                        {
                            return Redirect(returnUrl);
                        }
                        //var userRoles = await _userManager.GetRolesAsync(user);
                        //return Redirect(@"Home/Index");
                        //if (userRoles[0] == "Administrator")
                        //{
                            return Redirect(@"Admin/Patient/Index");
                        
                        //}
                        //else
                        //{
                        //    return Redirect(@"Client/ClientDashBoard/Index");
                        //}

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
