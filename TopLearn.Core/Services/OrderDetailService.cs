using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services
{
    public class OrderDetailService: IOrderDetailService
    {
        private TopLearnContext _context;

        public OrderDetailService(TopLearnContext topLearnContext)
        {
            _context = topLearnContext;
        }

        public void AddOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public OrderDetail GetOrderDetail(int orderId, int courseId)
        {
            return _context.OrderDetails.FirstOrDefault(o => o.OrderId == orderId && o.CourseId == courseId);
        }
    }
}
