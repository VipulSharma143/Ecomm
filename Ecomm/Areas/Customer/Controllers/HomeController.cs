using Ecomm.Areas.Admin.Controllers;
using Ecomm.DataAccess.Repository;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Models.ViewModels;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecomm.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string search, string OrderHeader )
        {
            var claimsidentity = (ClaimsIdentity)(User.Identity);
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCount, count);
            }
            var productList = _unitOfWork.Product.GetAll();
            if (!string.IsNullOrEmpty(search))
            {
                productList = productList.Where(p => p.Title.Contains(search)).ToList();
                productList = productList.OrderByDescending(p => p.Title==OrderHeader).ToList();  
            }
            if (!string.IsNullOrEmpty(OrderHeader))
            {
                productList = productList.OrderByDescending(p => _unitOfWork.ShoppingCart.GetAll(sc => sc.ProductId == p.Id).Sum(sc => sc.Count)).ToList();
            }
            foreach (var product in productList)
            {
                product.Rating = _unitOfWork.OrderDetailRepo.GetAll(od => od.ProductId == product.Id).Sum(od => od.Count);
            }
            var productsWithOrderCounts = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType")
                  .Select(p => new ProductWithOrderCountViewModel
                  {
                      ProductId = p.Id,
                      Title = p.Title,
                      ImageUrl = p.ImageUrl,
                      ListPrice = p.ListPrice,
                      Price100 = p.Price100,

                      OrderCount = _unitOfWork.OrderDetailRepo
                          .GetAll(od => od.ProductId == p.Id)
                          .Sum(od => od.Count), // Use Sum() to calculate the total count

                      // Add a Quantity property to display the available quantity
                      //Quantity = p.Quantity
                  })
                  .OrderByDescending(p => p.OrderCount)
                  .ToList();

            // Assign ratings based on order count
            AssignRatings(productsWithOrderCounts);

            return View(productsWithOrderCounts);
        }

        private void AssignRatings(List<ProductWithOrderCountViewModel> products)
        {
            foreach (var product in products)
            {
                int orderCount = product.OrderCount;

                if (orderCount >= 20)
                {
                    product.Rating = "★★★★★"; // 5-star rating if 10 or more items bought
                }
                else if (orderCount >= 15)
                {
                    product.Rating = "★★★★"; // 4-star rating if 8 or more items bought
                }
                else if (orderCount >= 10)
                {
                    product.Rating = "★★★"; // 3-star rating if 6 or more items bought
                }
                else if (orderCount >= 5)
                {
                    product.Rating = "★★"; // 2-star rating if 4 or more items bought
                }
                else
                {
                    product.Rating = "★"; // 1-star rating if less than 4 items bought
                }
            }
        }
    
        //return View(productList);
    public IActionResult DownloadImage(int id)
        {
            var product = _unitOfWork.Product.FirstOrDefalut(p => p.Id == id); //Include ProductImages navigation property
            if (product == null || product.ImageUrl == null || !product.ImageUrl.Any())
            {
                return NotFound();
            }

            //  var image = _unitOfWork.Product.FirstOrDefalut(p => p.Id == id, includeProperties: "ProductId"); // Assuming the first image
            if (product == null)
            {
                return NotFound();
            }

            return File(product.ImageUrl, GetContentType(product.ImageUrl), product.ImageUrl + ".jpg");
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                // Add more cases for other image formats as needed
                default:
                    return "application/octet-stream"; // Default content type for unknown formats
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id)
        {
            var claimsidentity = (ClaimsIdentity)(User.Identity);
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCount, count);
            }

            var ProductInDb = _unitOfWork.Product.FirstOrDefalut(p => p.Id == id);
            if (ProductInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = ProductInDb,
                ProductId = id,
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)(User.Identity);
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shoppingCart.ApplicationUserId = claim.Value;
                var shoppingcardindb = _unitOfWork.ShoppingCart.FirstOrDefalut(sc => sc.ApplicationUserId == claim.Value
                && sc.ProductId == shoppingCart.ProductId);
                if (shoppingcardindb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingcardindb.Count += shoppingCart.Count;
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {
                var ProductInDb = _unitOfWork.Product.FirstOrDefalut(p => p.Id == shoppingCart.Id);
                if (ProductInDb == null) return NotFound();
                var shoppingCartED = new ShoppingCart()
                {
                    Product = ProductInDb,
                    ProductId = ProductInDb.Id,
                };
                return View(shoppingCartED);
            }
        }
        public IActionResult Trending()
        {
            var productByCounts = _unitOfWork.OrderDetailRepo.GetAll(IncludeProperties: "Product").OrderByDescending(x => x.Count).GroupBy(x => x.ProductId);
            List<Product> products = new List<Product>();
            foreach (var group in productByCounts)
            {
                var any = _unitOfWork.Product.FirstOrDefalut(x => x.Id == group.Key);
                products.Add(any);
            }
            return View("Index", products);
        }
    }
}


    

    

