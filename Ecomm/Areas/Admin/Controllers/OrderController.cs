using Ecomm.Areas.Customer.Controllers;
using Ecomm.DataAccess.Data;
using Ecomm.DataAccess.Repository;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Ecomm.Models.ViewModels;

namespace Ecomm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
    //    private readonly ApplicationDbContext _context;
        public OrderController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)

        {
            _unitOfWork = unitOfWork;
            //_emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            // Fetch all users who have added items to their carts
            var Productlist = _unitOfWork.OrderHeader
                .GetAll(IncludeProperties: "ApplicationUserId");
                return Json(new { data = Productlist });

        }
        #endregion
        public IActionResult Details(int?id)
        {
            OrderDetail orderDetails = new OrderDetail();
            if (id == null) return View(orderDetails);
            orderDetails = _unitOfWork.OrderDetailRepo.FirstOrDefalut(or => or.Id == id ,includeProperties: "Product,orderHeader,orderHeader.ApplicationUser");
            if (orderDetails == null) return NotFound();
            return View(orderDetails);
        }
    }
}