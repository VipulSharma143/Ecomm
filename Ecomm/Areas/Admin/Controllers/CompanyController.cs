using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.DataAccess.Repository;
using Ecomm.Models;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Ecomm.Utility;

namespace Ecomm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin+","+SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
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
            return Json(new {data=_unitOfWork.Company.GetAll()});
        }
        [HttpGet]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CompanyInDb = _unitOfWork.Company.Get(id);
            if (CompanyInDb == null) return Json(new { success = false, message="Something's Wrong" });
            _unitOfWork.Company.Remove(CompanyInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Succesful" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id== null) return View(company);
            company=_unitOfWork.Company.Get(id.GetValueOrDefault());
            if(company==null) return NotFound();    
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (company == null) return BadRequest();
            if (!ModelState.IsValid) return View(company);
            if (company.Id == 0)
                _unitOfWork.Company.Add(company);
            else
                _unitOfWork.Company.Update(company);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
    }
}
