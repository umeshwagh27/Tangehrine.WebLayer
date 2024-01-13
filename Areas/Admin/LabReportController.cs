using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class LabReportController : BaseController<LabReportController>
    {
        #region Fields

        private readonly ILabReportService _labReportService;

        #endregion

        #region Ctor
        public LabReportController(ILabReportService labReportService)
        {
            _labReportService = labReportService;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View("LabReportList");
        }

        [HttpGet]
        public async Task<IActionResult> LabReportList()
        {
            var labReportData = await _labReportService.GetLabReportList();
            return Json(labReportData);
        }

       
        [HttpGet]
        public async Task<IActionResult> AddEditLabReport(long id)
        {
            LabReportDto labReportDto = new LabReportDto();
 
            if (id > 0)
            {
            
                labReportDto = await _labReportService.GetLabReportById(id);
            }
            return Json(labReportDto);
        }    
      
        [HttpPost]
        public async Task<IActionResult> AddEditLabReport(LabReportDto input)
        {
            if (input.LabReportName != null)
            {
                if (input.Id > 0)
                {
                    var labReportData = await _labReportService.UpdateLabReport(input);
                    return Json(labReportData);
                }
                else
                {
                    
                    var labReportData = await _labReportService.AddLabReport(input);
                    return Json(labReportData);
                }
            }

            return Json(new { message = "Please check the form again" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteLabReport(long id)
        {
            bool isLabReportDeleted = await _labReportService.DeleteLabReport(id);
            return Json(isLabReportDeleted);
        }
        #endregion
    }
}
