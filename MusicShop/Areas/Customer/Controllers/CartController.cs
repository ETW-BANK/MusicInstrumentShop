using ClothShop.Models.ViewModels;
using ClothShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;
using System.Security.Claims;
using MusicShop.Utility;
using Stripe.BillingPortal;
using Stripe.Checkout;
using SessionService = Stripe.Checkout.SessionService;
using Session = Stripe.Checkout.Session;

namespace MusicShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()

            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = (double)GetTotalPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal = (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var chartfromdb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            chartfromdb.Count += 1;
            _unitOfWork.ShoppingCart.Update(chartfromdb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var chartfromdb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, tracked: true);

            if (chartfromdb.Count <= 1)
            {
                HttpContext.Session.SetInt32(StaticData.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == chartfromdb.ApplicationUserId).Count() - 1);

                _unitOfWork.ShoppingCart.Remove(chartfromdb);

            }
            else
            {
                chartfromdb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(chartfromdb);
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var chartfromdb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(StaticData.SessionCart, _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == chartfromdb.ApplicationUserId).Count() - 1);

            _unitOfWork.ShoppingCart.Remove(chartfromdb);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summery()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()

            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = (double)GetTotalPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal = (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summery")]
        public IActionResult SummeryPOST()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");


            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);




            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = (double)GetTotalPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal = (cart.Price * cart.Count);
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.OrderStatus = StaticData.StatusPending;
                ShoppingCartVM.OrderHeader.paymentStatus = StaticData.PaymentStatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderStatus = StaticData.StatusApproved;
                ShoppingCartVM.OrderHeader.paymentStatus = StaticData.PaymentStatusDelayedPayment;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }



            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7286/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "SEK",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);


                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });

        }


        public ActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.paymentStatus != StaticData.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, StaticData.StatusApproved, StaticData.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

                HttpContext.Session.Clear();
            }

            List<ShoppingCart> shaoppingcart = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shaoppingcart);
            _unitOfWork.Save();
            return View(id);
        }

        private decimal GetTotalPrice(ShoppingCart shoppingCart)
        {



            return shoppingCart.Product.Price;


        }
    }

}
