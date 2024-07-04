using Ecomm.DataAccess.Data;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork=unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList=_context.ApplicationUsers.ToList(); //ASPNET Users
            var roles=_context.Roles.ToList(); //ASPNET Roles
            var userRoles=_context.UserRoles.ToList(); //ASPNEt  UserRoles
            foreach (var user in userList) 
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.companyId !=null)
                {
                    user.company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.companyId)).Name
                    };
                }
                if(user.companyId==null)
                {
                    user.company = new Company()
                    {
                      Name=""
                    };
                }
            }
            //Remove Admin User
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser); 
            return Json(new {data=userList});
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id) 
        {
            bool IsLocked=false;
            var userinDb = _context.ApplicationUsers.FirstOrDefault(au => au.Id == id);
            if (userinDb == null)
                return Json(new { success = false, message = "Something Went Wrong  while Locking oR Unlocking" });
            if(userinDb!=null && userinDb.LockoutEnd>DateTime.Now) 
            {
                userinDb.LockoutEnd = DateTime.Now;
                IsLocked = false;
            }
            else
            {
                userinDb.LockoutEnd= DateTime.Now.AddYears(100);
                IsLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = IsLocked == true ?
                "User Locked" : "User Unlocked" });
        }
        #endregion
    }
}