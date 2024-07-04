using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        #region API
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll() 
        {
            var categoryList=_unitOfWork.Category.GetAll();
            return Json(new {data=categoryList});
        }
        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null) return Json(new { success = false, message = "Kuch Galat Ho GYa" });
            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Kaam Ho Gya Boss" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Covertype category=new Covertype();
            if(id==null) return View(category);
            category=_unitOfWork.Category.Get(id.GetValueOrDefault());
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Covertype category)
        {
            if (category==null) return BadRequest(); 
            if(!ModelState.IsValid) return View(category);
            if(category.Id==0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
    }
}
