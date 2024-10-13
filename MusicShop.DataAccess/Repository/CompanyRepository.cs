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
    public class CompanyRepository: Repository<Companies>, ICompanyRepository
    {
        public readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Upadate(Companies companies)
        {
            _context.Companies.Update(companies);
        }
    }
}
