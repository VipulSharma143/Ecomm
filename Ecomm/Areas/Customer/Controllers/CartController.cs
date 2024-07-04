using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Utility;
using Ecomm.Models.ViewModels;
using Ecomm.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Ecomm_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool IsEMailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;

            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            //*
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefalut(au => au.Id == claim.Value);//jis user nai log in kiya ye uski deatil yha dal dega
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);//list price ke liye loop lgai hai or ye price jo method bnaya hai us se calculate hoga quantiy mai count aaega aur price product model se milega  aur ye tbhi milega agr aapne uper includeproperties add kiya hoga to
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }

            return View(ShoppingCartVM);
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefalut(c => c.Id == id);
            cart.Count += 1;
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult Checkbox(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefalut(c => c.Id == id);
            _unitOfWork.ShoppingCart.Remove(cart);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claims.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.SS_SessionCount, count);
            return RedirectToAction("Summary");
        }


        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefalut(c => c.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.FirstOrDefalut(c => c.Id == id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.save();
            ////apply session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.SS_SessionCount, count);
            return RedirectToAction("Index");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(int[] selectedproducts)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitOfWork.ApplicationUser.FirstOrDefalut
                (au => au.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email Is Empty");
            }
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                IsEMailConfirm = true;
            }
            return RedirectToAction("Summary", new { selectedproducts });
        }

        public IActionResult Summary(int[] selectedproducts)
        {
            var userAddresses = _unitOfWork.ApplicationUser.GetAll();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var selectedproductsIds = selectedproducts ?? new int[0];
            var selectedproduct = _unitOfWork.ShoppingCart.GetAll(sc => selectedproductsIds.Contains(sc.Id) && sc.ApplicationUserId == claims.Value, includeProperties: "Product");
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = selectedproduct,
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefalut(au => au.Id == claims.Value);

            // Pass addresses to the view
            ViewBag.UserAddresses = userAddresses;

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
            }

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            if (!IsEMailConfirm)
            {
                ViewBag.EmailMessage = "Sent";
                ViewBag.EmailCSS = "text-success";
                IsEMailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Confirm The Email";
                ViewBag.EmailCSS = "text-success";
            }

            return View(ShoppingCartVM);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken, int[] selectedproducts)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var userAddress = _unitOfWork.ApplicationUser.FirstOrDefault(a => a.Id == claim.Value,includeProperties:"StreetAddress");

            // If the user has provided an address, assign it to the order
            //if (userAddress != null)
            {

            }
            var selectedproductsId = selectedproducts ?? new int[0];//

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefalut(au => au.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value && selectedproducts.Contains(sc.Id), includeProperties: "Product");
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.save();
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetail OrderDetailRepo = new OrderDetail()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                _unitOfWork.OrderDetailRepo.Add(OrderDetailRepo);
                _unitOfWork.save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.save();
            #region Stripe
            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApporved;
            }
            else
            {
                var options = new ChargeCreateOptions()//ye srripe ke he class hai
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "USD",
                    Description = "order Id:" + ShoppingCartVM.OrderHeader.Id.ToString(),
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)//iska mtlb payment nhi hui hai 
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;//yha pr transactionid generate hota hai
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if (charge.Status.ToLower() == "succeeded")//ab pta chlga payment hui hai ke nhi
                {
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApporved;
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }
                _unitOfWork.save();
            }
            #endregion


            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefalut(au => au.Id == Claims.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is empty");
            }

            else
            {


                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);


                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                    $"Your order has been placed successfully. Thank you for shopping with us by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                string accountSid = "AC198deaa21ee871031d58083bb790318b";
                string authToken = "66c4673d4f279f396378b3cce1971585";


                // Initialize the Twilio client
                TwilioClient.Init(accountSid, authToken);

                // The phone number to send the message from (Twilio phone number)
                string fromPhoneNumber = "+12183167899"; // Replace with your Twilio phone number

                // The phone number to send the message to (destination phone number)
                string toPhoneNumber = "+917018324640"; // Replace with the destination phone number

                try
                {
                    var message = MessageResource.Create(
                        body: " Babe Ressa Hoi Skdiyaan Mukable Ne!!!!",
                        from: new PhoneNumber(fromPhoneNumber),
                        to: new PhoneNumber(toPhoneNumber)
                    );

                    Console.WriteLine("Message SID: " + message.Sid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            return View(id);
        }
    }
}
