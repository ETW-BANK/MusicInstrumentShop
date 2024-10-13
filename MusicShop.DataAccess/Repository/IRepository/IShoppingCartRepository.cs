using ClothShop.Models;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothShop.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {

        void Update(ShoppingCart shoppingCart);
    }
}
