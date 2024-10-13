using ClothShop.Models;
using MusicShop.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothShop.DataAccess.Repository.IRepository
{
public interface IOrderDetailRepository:IRepository<OrderDetail>
    {

        void Update(OrderDetail orderDetail);   
    }
}
