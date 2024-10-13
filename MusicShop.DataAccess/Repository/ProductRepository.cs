using MusicShop.DataAccess.Data;
using MusicShop.DataAccess.Repository.IRepository;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicShop.DataAccess.Repository
{
   public class ProductRepository: Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Upadate(Product product)
        {
            _context.Products.Update(product);
        }
    }
}
