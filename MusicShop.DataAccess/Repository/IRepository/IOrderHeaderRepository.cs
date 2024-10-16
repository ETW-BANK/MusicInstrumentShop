﻿using ClothShop.Models;
using MusicShop.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothShop.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update (OrderHeader orderHeader);  

        void UpdateStatus (int id,string orderStatus,string? paymentStatus=null);

        void UpdateStripePaymentID (int id,string sessionId,string PaymentIntentId);
    }
}
