using System;
using System.Collections.Generic;
using System.Text;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
   public interface IOrderDetailService
   {
       OrderDetail GetOrderDetail(int orderId, int courseId);
       void AddOrderDetail(OrderDetail orderDetail);
   }
}
