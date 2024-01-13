using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.WebLayer.Utility;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]
    public class ToDoController : BaseController<ToDoController>
    {
        #region Fields
        private readonly IToDoListService _toDoListService;
        #endregion

        #region Ctor
        public ToDoController(IToDoListService toDoListService)
        {
            _toDoListService = toDoListService;
        }
        #endregion
        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> ToDoList()
        {
            var todosData = await _toDoListService.GetAllToDoList();
            return Json(todosData);
        }

        [HttpPost]
        public async Task<IActionResult> AddToDoList(ToDoDto input)
        {
            var todosData = await _toDoListService.AddToDo(input);
            return Json(todosData);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteToDo(long id)
        {
            try
            {
                bool isDelete = await _toDoListService.DeleteTodo(id);
                return Json(isDelete);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
    }
}
