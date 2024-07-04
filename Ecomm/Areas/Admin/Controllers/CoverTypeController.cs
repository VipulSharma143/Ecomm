using Dapper;
using Ecomm.DataAccess.Repository;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin + "," + SD.Role_Employee)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.SPCall.List<CoverType>(SD.SP_GetCoverTypes) });

        }
        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            var covertypeInDb=_unitOfWork.CoverType.Get(id);
            if (covertypeInDb == null) return Json(new {success=false, message="Kuch To Gadbad Hai"});
            DynamicParameters param =new DynamicParameters();
            param.Add("id", id);
            _unitOfWork.SPCall.Execute(SD.SP_DeleteCoverType, param);
            return Json(new { success = true, message = "Job Is Done" });
        }
        #endregion
        public IActionResult Upsert(int?id)
        {
            CoverType CoverType= new CoverType();
            if(id==null) return View(CoverType);
            DynamicParameters param = new DynamicParameters();
            param.Add("id", id.GetValueOrDefault());
            CoverType = _unitOfWork.SPCall.OneRecord<CoverType>(SD.SP_GetCoverType,param);
            CoverType=_unitOfWork.CoverType.Get(id.GetValueOrDefault());
            return View(CoverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return BadRequest();
            if(!ModelState.IsValid) return View(coverType);
            DynamicParameters param=new DynamicParameters();
            param.Add("name",coverType.Name);
            if(coverType.Id==0)
                _unitOfWork.SPCall.Execute(SD.SP_CreateCoverType, param);
            else
            {
                param.Add("id",coverType.Id);
                _unitOfWork.SPCall.Execute(SD.SP_UpdateCoverType,param);
            }
            return RedirectToAction(nameof(Index));
         }
    }
}
