using System;
using System.Collections.Generic;
using System.Text;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services.Interfaces
{
   public interface IOrderService
   {
       int AddOrder(int userId,int courseId);
       Order GetActiveOrderForShow(int orderId, int userId);
   }
}
