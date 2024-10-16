using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Tangehrine.DataLayer.DbContext;
using Tangehrine.ServiceLayer.Utility;
using Microsoft.Extensions.Configuration;
using Tangehrine.ServiceLayer.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Tangehrine.WebLayer.Common;
using Tangehrine.ServiceLayer.Enums;
using Tangehrine.ServiceLayer.Dtos;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IErrorLogService _errorLog;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IOptions<EmailSettingsGmail> emailSettingsGmail,
           IConfiguration config,
           IErrorLogService errorLog,
           IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _emailService = new EmailService(emailSettingsGmail);
            _config = config;
            _errorLog = errorLog;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }
  
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.Where(x=>x.Email==Input.Email && x.IsActive==true && x.IsDelete==false).FirstOrDefaultAsync();
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
               
                var email=CommonMethods.EncryptData(user.Email, GlobalConstant.EncryptionCode);
                callbackUrl += " " + email;
                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                string physicalUrl = _config.GetValue<string>("CommonProperty:PhysicalUrl");
                string EmailLogo = _config.GetValue<string>("CommonProperty:EmailLogo");

                //string tamplate = "<!DOCTYPE html> <html> <head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /> <title>Reset Password</title> <link href=\"http://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600&display=swap\" rel=\"stylesheet\"> <style type=\"text/css\" rel=\"stylesheet\" media=\"all\"> /* Base ------------------------------ */ *:not(br):not(tr):not(html) { font-family: 'Montserrat'; -webkit-box-sizing: border-box; box-sizing: border-box; }body { width: 50% !important; height: 100%; margin: 0 auto; line-height: 1.4; background-color: #F5F7F9; color: #839197; -webkit-text-size-adjust: none; }a { color: #414EF9; }a[href] { color: white; } /* Layout ------------------------------ */ .email-wrapper { width: 50%; margin: 0 auto; padding: 0; background-color: #F5F7F9; }.email-content { width: 50%; margin: 0; padding: 0; } /* Masthead ----------------------- */ .email-masthead { padding: 25px 0; text-align: center; }.email-masthead_logo { max-width: 400px; border: 0; }.email-masthead_name { font-size: 16px; font-weight: bold; color: #839197; text-decoration: none; text-shadow: 0 1px 0 white; } /* Body ------------------------------ */ .email-body { width: 50%; margin: 0; padding: 0; border-top: 1px solid #E7EAEC; border-bottom: 1px solid #E7EAEC; background-color: #FFFFFF; }.email-body_inner { width: 570px; margin: 0 auto; padding: 0; }.email-footer { width: 570px; margin: 0 auto; padding: 0; text-align: center; background: rgba(206, 36, 42,.05); }.email-footer p { color: #ce242a; font-size: 14px; } .body-action { width: 100%; margin: 30px auto; padding: 0; text-align: center; }.body-sub { margin-top: 25px; padding-top: 25px; border-top: 1px solid #E7EAEC; }.content-cell { padding: 35px; }.content-cell-footer { padding-top: 15px; padding-bottom: 10px; }.align-right { text-align: right; } /* Type ------------------------------ */ h1 { margin-top: 0; color: #292E31; font-size: 19px; font-weight: bold; text-align: left; }h2 { margin-top: 0; color: #292E31; font-size: 16px; font-weight: bold; text-align: left; }h3 { margin-top: 0; color: #292E31; font-size: 14px; font-weight: bold; text-align: left; }p { margin-top: 0; color: #839197; font-size: 16px; line-height: 1.5em; text-align: left; }p.sub { font-size: 14px; }p.sub a { color: #414EF9; }p.center { text-align: center; } /* Buttons ------------------------------ */ .button { display: inline-block; width: 200px; background-color: #414EF9; border-radius: 3px; color: #ffffff; font-size: 16px; line-height: 45px; font-weight: 500; text-align: center; text-decoration: none; -webkit-text-size-adjust: none; mso-hide: all; }.button--green { background-color: #28DB67; }.button--red { background-color: #FF3665; }.button--blue { background-color: #f93e3e; border:1px solid #f93e3e; color: #fff !important; transition: .3s all; font-size: 16px; } .button--blue:hover { background-color: #db2525; border: 1px solid #db2525; }/*Media Queries ------------------------------*/ @media(max-width: 1140px) { body { width: 100% !important; } } @media (max-width: 600px) { .email-wrapper,.email-content { width: 100%; } .email-body_inner, .email-footer { width: 100% !important; } }@media (max-width: 500px) { .button { width: 100% !important; } } </style> </head> <body><table class=\"email-wrapper\" width=\"50%\" cellpadding=\"0\" cellspacing=\"0\"> <tr> <td> <div style=\"text-align: center;background-color: rgba(206, 36, 42,.05);padding: 10px 0px;\"> <!--<img alt=\"JunTechnology\" src={junlogo} />--> <img alt=\"Tangehrine\" src=\"http://clientapp.narola.online:1160/JunTechfrontend/img/jun-logo.png\" /> </div> </td> </tr> <tr> <td align=\"center\"> <table class=\"email-content\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"> <!-- Email Body --> <tr> <td class=\"email-body\" width=\"50%\"> <table class=\"email-body_inner\" align=\"center\" width=\"570\" cellpadding=\"0\" cellspacing=\"0\"> <!-- Body content --> <tr> <td class=\"content-cell\"> <h1>Reset Password</h1> <p><b>Hello</b> {UserName},</p> <p>To reset your password, please click on the link below.</p> <!-- Action --> <table class=\"body-action\" align=\"center\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\"> <tr> <td align=\"center\"> <div> <a href=\"{action_url}\" class=\"button button--blue\">Click Here</a> </div> </td> </tr> </table> <p>Thanks,<br />The Tangehrine Team</p> <!-- Sub copy --> <!--<table class=\"body-sub\"> <tr> <td> <p class=\"sub\"> If you�re having trouble clicking the button, copy and paste the URL below into your web browser. </p> <p class=\"sub\"><a href=\"{action_url}\">{action_url}</a></p> </td> </tr> </table>--> </td> </tr> </table> </td> </tr> <tr> <td> <table class=\"email-footer\" align=\"center\" width=\"570\" cellpadding=\"0\" cellspacing=\"0\"> <tr> <td class=\"content-cell-footer\"> <p class=\"sub center\"> Tangehrine. <br /> </p> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </body> </html>";
                //string physicalUrl = _config.GetValue<string>("CommonProperty:PhysicalUrl");
                //string EmailLogo = _config.GetValue<string>("CommonProperty:EmailLogo");


                string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLog, _webHostEnvironment.WebRootPath + "\\" + "EmailTemplate", EmailTemplateFileList.ResetPassword, physicalUrl);
              //  string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLog, "", tamplate, physicalUrl);
                //emailTemplate = emailTemplate.Replace("{UserName}", user.UserName + " " + EmailLogo);
                emailTemplate = emailTemplate.Replace("{UserName}", user.UserName);
                emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);

                await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                {
                    ToAddress = Input.Email,
                    Subject = "Reset your email",
                    BodyText = emailTemplate

                });

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
