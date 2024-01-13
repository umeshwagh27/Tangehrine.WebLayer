using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class UserAccessController : BaseController<UserAccessController>
    {

        #region Fields

        private readonly IUserAccessService _userAccessService;

        #endregion

        #region Ctor

        public UserAccessController(IUserAccessService userAccessService)
        {
            _userAccessService = userAccessService;
        }

        #endregion

        #region Method

        public IActionResult Index()
        {
            return View();
        }     
        [HttpGet]
        public async Task<IActionResult> AddEditUserAccess(long id)
        {
            UserAccessDto userAccessDto = new UserAccessDto();
            if (id > 0)
            {
                userAccessDto = await _userAccessService.GetUserAccessById(id);

            }
            return Json(userAccessDto);
        }
        [HttpPost]
        public async Task<IActionResult> AddEditUserAccess([FromForm] UserAccessDto input)
        {
            if (input.UserId > 0)
            {
                if (input.Id > 0)
                {
                    var userAccessData = await _userAccessService.UpdateUserAccess(input);
                    return Json(userAccessData);
                }
                else
                {
                    var userAccessData = await _userAccessService.AddUserAccess(input);
                    return Json(userAccessData);
                }
            }
            return Json(new { message = "Please check the form again" });
        }
        [HttpPost]
        public async Task<JsonResult> DeleteUserAccess(long id)
        {
            bool isUserAccessDeleted = await _userAccessService.DeleteUserAccess(id);
            return Json(isUserAccessDeleted);
        }
        #endregion
    }
}
