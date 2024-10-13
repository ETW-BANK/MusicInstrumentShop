
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;
using MusicShop.ViewModels;

namespace Traditional_Cloth_Shop_App.Areas.Admin.Controllers
{

    [Area("Admin")]
    //[Authorize(Roles = StaticData.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {

            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           

            
            return View(productsList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
           {
               CategoryList =_unitOfWork.Category.GetAll().Select(u=>new SelectListItem
               {
                   Text = u.Name,
                   Value=u.Id.ToString()
               }),

             Product = new Product()
           };
           if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product=_unitOfWork.Product.GetFirstOrDefault(u=>u.Id==id);

                return View(productVM);
            }
          

        }

        [HttpPost]  

        public IActionResult Upsert(ProductVM Newproduct,IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath=_webHostEnvironment.WebRootPath;

                if(file!=null) 
                { 
                  string fileName=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                  string productPath=Path.Combine(wwwRootPath,@"images\product");

                    if(!string.IsNullOrEmpty(Newproduct.Product.ImageUrl))
                    {
                        var oldImagePath=Path.Combine(wwwRootPath,Newproduct.Product.ImageUrl.Trim('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var filStream=new FileStream(Path.Combine(productPath,fileName), FileMode.Create))
                    {
                        file.CopyTo(filStream); 
                    }

                    Newproduct.Product.ImageUrl = @"\images\product\" + fileName;
                
                }

                if(Newproduct.Product.Id==0)
                {

                    _unitOfWork.Product.Add(Newproduct.Product);
                }
                else
                {
                    _unitOfWork.Product.Upadate(Newproduct.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Procucte created Successfully";
                return RedirectToAction("Index");   
            }
            else
            {

                Newproduct.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(Newproduct);

            }

                
            
          
        }

       
        #region ApiCalls

        [HttpGet]

        public IActionResult GetAll()
        {

            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();



            return Json(new {data=productsList});
        }

        [HttpDelete]

        public IActionResult Delete(int? id)
        {

            var producttodelete = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (producttodelete==null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                producttodelete.ImageUrl.Trim('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(producttodelete);
            _unitOfWork.Save();


            return Json(new { success = true, message="Product Deleted Successfully" });
        }

        #endregion
    }
}
