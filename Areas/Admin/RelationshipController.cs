using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]    
    public class RelationshipController : BaseController<RelationshipController>
    {
        #region Fields

        private readonly IRelationshipService _relationshipService;

        #endregion

        #region Ctor
        public RelationshipController(IRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            //return View();
            return View("RelationshipList");
        }

        [HttpGet]
        public async Task<IActionResult> RelationshipList()
        {
            var relationshipData = await _relationshipService.GetRelationshipList();
            return Json(relationshipData);
        }

        [HttpGet]
        public async Task<IActionResult> AddEditRelationship(long id)
        {
            RelationshipDto relationshipDto = new RelationshipDto();
            if (id > 0)
            {
                relationshipDto = await _relationshipService.GetRelationshipById(id);

            }
            return Json(relationshipDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditRelationship(RelationshipDto input)
        {
            if (input.RelationshipName != null)
            {
                if (input.Id > 0)
                {
                    var relationshipData = await _relationshipService.UpdateRelationship(input);
                    return Json(relationshipData);
                }
                else
                {
                    var relationshipData = await _relationshipService.AddRelationship(input);
                    return Json(relationshipData);
                }
            }

            return Json(new { message = "Please check the form again" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteRelationship(long id)
        {
            bool isRelationshipDeleted = await _relationshipService.DeleteRelationship(id);
            return Json(isRelationshipDeleted);
        }
        #endregion
    }
}
