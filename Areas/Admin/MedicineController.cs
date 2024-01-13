using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]    
    public class MedicineController : BaseController<MedicineController>
    {
        #region Fields

        private readonly IMedicineService _medicineService;

        #endregion

        #region Ctor
        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View("MedicineList");
        }

        [HttpGet]
        public async Task<IActionResult> MedicineList()
        {
            var medicineData = await _medicineService.GetMedicineList();
            return Json(medicineData);
        }

        [HttpGet]
        public async Task<IActionResult> AddEditMedicine(long id)
        {
            MedicineDto medicineDto = new MedicineDto();
            if (id > 0)
            {
                medicineDto = await _medicineService.GetMedicineById(id);

            }
            return Json(medicineDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditMedicine(MedicineDto input)
        {
            if (input.MedicineName != null)
            {
                if (input.Id > 0)
                {
                    var medicineData = await _medicineService.UpdateMedicine(input);
                    return Json(medicineData);
                }
                else
                {
                    var medicineData = await _medicineService.AddMedicine(input);
                    return Json(medicineData);
                }
            }

            return Json(new { message = "Please check the form again" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteMedicine(long id)
        {
            bool isMedicineDeleted = await _medicineService.DeleteMedicine(id);
            return Json(isMedicineDeleted);
        }
        #endregion
    }
}