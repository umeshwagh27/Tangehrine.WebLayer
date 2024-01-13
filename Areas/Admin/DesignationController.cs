using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]    
    public class DesignationController : BaseController<DesignationController>
    {
        #region Fields

        private readonly IDesignationService _designationService;

        #endregion

        #region Ctor
        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {            
            return View("DesignationList");
        }

        [HttpGet]
        public async Task<IActionResult> DesignationList()
        {
            var designationData = await _designationService.GetDesignationList();
          
            return Json(designationData);
        }

        [HttpGet]
        public async Task<IActionResult> AddEditDesignation(long id)
        {
            DesignationDto designationDto = new();
            if (id > 0)
            {
                designationDto = await _designationService.GetDesignationById(id);
            }
            return Json(designationDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditDesignation([FromForm] DesignationDto input)
        {
            if (input.Name != null)
            {
                if (input.Id > 0)
                {
                    var designationData = await _designationService.UpdateDesignation(input);
                    return Json(designationData);
                }
                else
                {
                    var designationData = await _designationService.AddDesignation(input);
                    return Json(designationData);
                }
            }

            return Json(new { message = "Please check the form again" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteDesignation(long id)
        {
            bool isDesignationDeleted = await _designationService.DeleteDesignation(id);
            return Json(isDesignationDeleted);
        }
        #endregion

    }
}
