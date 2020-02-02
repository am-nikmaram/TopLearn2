using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Core.Services
{
   public class OrderService : IOrderService
    {
        private ICourseService _courseService;
        private IOrderDetailService _orderDetailService;
        private TopLearnContext _context;

        public OrderService(ICourseService courseService, IOrderDetailService orderDetailService, TopLearnContext context)
        {
            _courseService = courseService;
            _orderDetailService = orderDetailService;
            _context = context;
        }

        public int AddOrder(int userId, int courseId)
        {
            Order order = _context.Orders.FirstOrDefault(o => o.UserId == userId && !o.IsFinaly);
            Course course = _courseService.GetCourseById(courseId);
            if (order == null)
            {
                order = new Order()
                {
                    UserId = userId,
                    OrderSum = course.Courseprice,
                    OrderDate = DateTime.Now,
                    IsFinaly = false,
                    OrderDetail = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            CourseId = courseId,
                            Count = 1,
                            Price = course.Courseprice
                        }
                    }
                };
                _context.Orders.Add(order);
            }
            else
            {
                OrderDetail odetail = _orderDetailService.GetOrderDetail(order.OrderId, courseId);
                if (odetail ==null)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        CourseId = courseId,
                        Count = 1,
                        Price = course.Courseprice
                    };
                    _orderDetailService.AddOrderDetail(orderDetail);
              
                }
                else
                {
                    odetail.Count += 1;
             
                }


            }
            _context.SaveChanges();
            return order.OrderId;

        }

        public Order GetActiveOrderForShow(int orderId, int userId)
        {
            return _context.Orders.Include(o => o.OrderDetail).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId && o.IsFinaly==false);
        }
    }
}
