
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;


namespace Traditional_Cloth_Shop_App.Areas.Admin.Controllers
{

    [Area("Admin")]
    //[Authorize(Roles = StaticData.Role_Admin)]
    public class CompanyController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Companies> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

       

     

        public IActionResult Upsert(int? id)
        {

           
       
            if (id == null || id == 0)
            {
                return View(new Companies());
            }
            else
            {
                Companies company=_unitOfWork.Company.GetFirstOrDefault(u=>u.Id == id);

                return View(company);
            }


        }


        [HttpPost]

        public IActionResult Upsert(Companies NewCompany)
        {
            if (ModelState.IsValid)
            {
                if(NewCompany.Id == 0) 
                {
                
                 _unitOfWork.Company.Add(NewCompany);
                
                }
                else
                {
                    _unitOfWork.Company.Upadate(NewCompany);    
                }

                _unitOfWork.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");   

            }
            else
            {
                return View(NewCompany);
            }

        }

        #region ApiCall
        [HttpGet]
        public IActionResult GetAll()
        {

            List<Companies> CompanyList = _unitOfWork.Company.GetAll().ToList();



            return Json(new { data = CompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companytodelete=_unitOfWork.Company.GetFirstOrDefault(u=>u.Id==id);

            if (companytodelete == null)
            {
                return Json(new {success= false, message="Faild to delete company"});
            }

            _unitOfWork.Company.Remove(companytodelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Company deleted Successfully" });
        }


        #endregion

    }


}