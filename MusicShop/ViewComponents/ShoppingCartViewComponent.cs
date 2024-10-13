using Microsoft.AspNetCore.Mvc;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Utility;
using System.Security.Claims;
using System.Web.Providers.Entities;

namespace MusicShop.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(StaticData.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(StaticData.SessionCart,
                  _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(StaticData.SessionCart));
            }
            else
            {

                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
