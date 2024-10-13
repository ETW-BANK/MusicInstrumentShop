
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;
using MusicShop.ViewModels;
using System.Diagnostics;
using System.Security.Claims;


namespace Traditional_Cloth_Shop_App.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
          

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(productList);
        }

        //public IActionResult Details(int productId)
        //{
        //    ShoppingCart cart = new()
        //    {
        //        Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category"),
        //        Count = 1,
        //        ProductId = productId
        //    };
         

        //    return View(cart);
        //}

        //[HttpPost]
        //[Authorize]
        //public IActionResult Details(ShoppingCart shoppingCart)
        //{
          
        //    var ClaimsIdentity=(ClaimsIdentity)User.Identity;
        //    var userId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    shoppingCart.ApplicationUserId= userId; 

        //    ShoppingCart cartfromdb=_unitOfWork.ShoppingCart.GetFirstOrDefault(u=>u.ApplicationUserId==userId 
        //    && u.ProductId==shoppingCart.ProductId);

        //    if(cartfromdb!=null)
        //    {
        //        //shopping cart exists
        //        cartfromdb.Count += shoppingCart.Count;
        //        _unitOfWork.ShoppingCart.Update(cartfromdb);
        //        _unitOfWork.Save();
        //    }

        //    else
        //    {
        //        _unitOfWork.ShoppingCart.Add(shoppingCart);
             
        //        _unitOfWork.Save();
              
        //        HttpContext.Session.SetInt32(StaticData.SessionCart,
                    
        //            _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == userId).Count());
        //    }

        //    TempData["success"] = "Item Added To Shpping Cart Successfully";
        

        //    return RedirectToAction(nameof(Index));

        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}