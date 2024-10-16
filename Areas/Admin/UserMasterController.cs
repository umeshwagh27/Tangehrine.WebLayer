﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.ServiceLayer.Interface.Admin.Master;
using Tangehrine.WebLayer.Utility;
using Microsoft.Extensions.Configuration;
using Tangehrine.ServiceLayer.Utility;
using Microsoft.Extensions.Options;
using Tangehrine.ServiceLayer.Dtos;
using Tangehrine.WebLayer.Common;
using Tangehrine.ServiceLayer.Enums;
using Tangehrine.WebLayer.Models;
using System.Transactions;
using System;
using System.Linq;
using Tangehrine.DataLayer.DbContext;
using Microsoft.AspNetCore.Identity;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.WebLayer.Areas.Identity.Pages.Account;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class UserMasterController : BaseController<UserMasterController>
    {
        #region Fields

        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IUserMasterService _userMasterService;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly IErrorLogService _errorLogService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPatientService _patientService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Ctor

        public UserMasterController(ICommonService commonService, IWebHostEnvironment webHostEnvironment, IUserMasterService userMasterService,
            IConfiguration config, IOptions<EmailSettingsGmail> emailSettingsGmail, IErrorLogService errorLogService, IPatientService patientService, SignInManager<ApplicationUser> signInManager)
        {
            _commonService = commonService;
            _WebHostEnvironment = webHostEnvironment;
            _userMasterService = userMasterService;
            _config = config;
            _emailService = new EmailService(emailSettingsGmail);
            _errorLogService = errorLogService;
            _patientService = patientService;
            _signInManager = signInManager;
        }

        #endregion

        #region Method

        public async Task<IActionResult> Index()
        {
            var userMasterData = await _userMasterService.GetUserMasterList();
            return View("UserMasterList", userMasterData);
        }     
        public async Task<IActionResult> GetDesignation(JQueryDataTableParamModel param)
        {

            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                
                    var allList = await _userMasterService.GetDesignation(parameters.ToArray());
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
        public async Task<IActionResult> GetAssociate(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                    var allList = await _userMasterService.GetAssociate(parameters.ToArray());
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
        public async Task<IActionResult> GetUserAccess(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                    var allList = await _userMasterService.GetUserAccess(parameters.ToArray());
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
        public async Task<IActionResult> AddEditUser(long id)
        {
            UserDto userDto = new UserDto();
            if (id > 0)
            {
                userDto = await _commonService.GetUserById(id);
            }
            return Json(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditUser(UserDto input)
        {         
            try
            {
                if (input.FirstName != null)
                {
                    if (input.Id > 0)
                    {
                        if (input.ProfileImage != null)
                        {
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, "Profiles", "Profile", input.ProfileImage);
                            input.ProfileImageUrl = filename;
                        }
                        var userData = await _commonService.UpdateUser(input);
                        if(userData.Message== "Role has been changed")
                        {
                            await _signInManager.SignOutAsync();                           
                        }
                        return Json(userData);
                    }
                    else
                    {
                        if (input.ProfileImage != null)
                        {
                            var filename = await CommonMethods.WriteFile(_WebHostEnvironment.WebRootPath, "Profiles", "Profile", input.ProfileImage);
                            input.ProfileImageUrl = filename;
                        }
                        var userData = await _commonService.AddUser(input);
                        if (userData.IsSuccess == true && userData.JsonObj != null)
                        {
                            //string physicalUrl = _config.GetValue<string>("CommonProperty:PhysicalUrl");
                            //string tamplate = "<!DOCTYPE html> <html> <head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /> <title>Confirm Email</title> <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\" /> <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin /> <link href=\"https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700;800;900&display=swap\" rel=\"stylesheet\" /></head> <body> <div style=\"max-width: 450px; margin: 0 auto;\"> <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"font-family: 'Poppins', sans-serif;\"> <thead> <tr> <td style=\"background: #ffdbc3; text-align: center; padding: 10px; border-radius: 15px 15px 0 0px;\"> <img src=\"data:image/png;base64,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\" alt=\"logo\" width=\"170\" style=\"vertical-align: top;\" /> </td> </tr> </thead> <tbody style=\"background: #F5F7F9;\"> <tr> <td style=\"padding: 40px 30px;\"> <p style=\"margin-top: 0;\">Hi There! This is your account information. please do not share it with anyone.</p> <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"text-align: left;\"> <tbody> <tr> <th style=\"padding: 10px 5px\">Portal link :-</th> <td style=\"padding: 10px 5px\"><a href=\"{physicalUrl}\">{physicalUrl}</a> </td> </tr> <tr> <th style=\"padding: 10px 5px\">Username :-</th> <td style=\"padding: 10px 5px\">{email}</td> </tr> <tr> <th style=\"padding: 10px 5px\">Password :-</th> <td style=\"padding: 10px 5px\">{password}</td> </tr> </tbody> </table> </td> </tr> </tbody> <tfoot> <tr> <td style=\"background: #ffdbc3; text-align: center; padding: 10px; font-size: 14px; line-height: 1.2; border-radius: 0 0 15px 15px;\"> <p style=\"margin: 0;\"><small>3 FOREST PARK FARMINGTON, CT 06032 <br>(888) 669 * 1158 <br> [F](888) 486 * 6210</small></p> </td> </tr> </tfoot> </table> </body> </html>";
                            //string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLogService, _WebHostEnvironment.ContentRootPath + "\\" + "Views", tamplate, physicalUrl);
                            ////Conversion of object type to Dto type
                            //UserDto user = (UserDto)(userData.JsonObj);
                            string physicalUrl = _config.GetValue<string>("CommonProperty:ActionUrl");
                            string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLogService, _WebHostEnvironment.WebRootPath + "\\" + "EmailTemplate", EmailTemplateFileList.ConfirmEmailOfUser, physicalUrl);
                            //Conversion of object type to Dto type
                            UserDto user = (UserDto)(userData.JsonObj);
                            emailTemplate = emailTemplate.Replace("{email}", user.Email);
                            emailTemplate = emailTemplate.Replace("{password}", user.Password);
                            emailTemplate = emailTemplate.Replace("{physicalUrl}", physicalUrl);
                            await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                            {
                                ToAddress = input.Email,
                                Subject = "Welcome to TangEhine",
                                BodyText = emailTemplate
                            });

                        }
                        return Json(userData);              
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return Json(new { message = "Please check the form again" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteUser(long id)
        {
            bool isUserDeleted = await _commonService.DeleteUser(id);
            return Json(isUserDeleted);
        }

        public async Task<IActionResult> GetRecoveryItems(JQueryDataTableParamModel param)
        {

            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                    var allList = await _userMasterService.GetRecoveryItem(parameters.ToArray());
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


        [HttpPost]
        public async Task<IActionResult> RecoverItemFromTrash(long id)
        {
            try
            {
                string type = await _commonService.GetTypeFromTrash(id);
                if (type == "Patient")
                {
                    bool patient = await _patientService.ActivatePatient(id);
                }
                if(type=="Associate")
                {
                    bool associate = await _commonService.ActivateUser(id);
                }
                await _commonService.EmptyTrash(id);                
                return Json(new { message = "Data recovered successfully" });  
            }
            catch (Exception ex)
            {
                return Json(new { message = "Please try again" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrash(long id)
        {
            try
            {
                bool isTrashEmpty = await _commonService.EmptyTrash(id);
                return Json(isTrashEmpty);
            }
            catch (Exception ex)
            {
                return Json(new { message = "Please try again" });
            }

        }

        #endregion
    }
}
