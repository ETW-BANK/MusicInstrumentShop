using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicShop.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Companies> Companies { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(

                new Category { Id = 1, Name = "String Instruments", DisplayOrder = 1 },
                 new Category { Id = 2, Name = "Percussion Instruments", DisplayOrder = 2 },
                  new Category { Id = 3, Name = "Keyboard Instruments", DisplayOrder = 3 },
                   new Category { Id = 4, Name = "Wind Instruments", DisplayOrder = 4 },
                    new Category { Id = 5, Name = "Folk & Ethnic Instruments", DisplayOrder = 5 },
                    new Category { Id = 6, Name = "Electronic Instruments", DisplayOrder = 6 },
                    new Category { Id = 7, Name = "Recording & Studio Gear", DisplayOrder = 7 },
                    new Category { Id = 8, Name = "Pro Audio Equipment", DisplayOrder = 8 },
                    new Category { Id = 9, Name = "Accessories & Gear", DisplayOrder = 9 },
                     new Category { Id = 10, Name = "Bundles & Deals", DisplayOrder = 10 }



            );
        }
    }
}
