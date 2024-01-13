using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Common
{
    [Authorize, Area("Common")]

    public class CommonController : Controller
    {
        private readonly ICommonService _commonService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CommonController(ICommonService commonService, IWebHostEnvironment webHostEnvironment)
        {
            _commonService = commonService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View("EditProfile");
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrentUser()
        {
            var result = await _commonService.GetCurrentUserAsync();
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> EditProfile(UserDto input)
        {
            if (input.ProfileImage != null)
            {

                var filename = await CommonMethods.WriteFile(_webHostEnvironment.WebRootPath, "Profiles", "Profile", input.ProfileImage);
                input.ProfileImageUrl = filename;
                HttpContext.Session.SetString("ProfileImageUrl", input.ProfileImageUrl);
            }
            if (input.FirstName != null)
            {
                HttpContext.Session.SetString("FirstName", input.FirstName);
            }
            var userData = await _commonService.UpdateUser(input);
            return Json(userData);
        }

    }
}
