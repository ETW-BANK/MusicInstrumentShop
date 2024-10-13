using ClothShop.DataAccess.Repository.IRepository;
using ClothShop.Models;
using MusicShop.DataAccess.Data;
using MusicShop.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClothShop.DataAccess.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private  ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}
