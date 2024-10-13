using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicShop.DataAccess.Repository.IRepository
{
    public interface IProductRepository: IRepository<Product>
    {
        void Upadate(Product product);
    }
}
