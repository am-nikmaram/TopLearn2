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
        private IUserService _userService;

        public OrderService(ICourseService courseService, IOrderDetailService orderDetailService, TopLearnContext context, IUserService userService)
        {
            _courseService = courseService;
            _orderDetailService = orderDetailService;
            _context = context;
            _userService = userService;
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
                order.OrderSum += course.Courseprice;

            }
            _context.SaveChanges();
            return order.OrderId;

        }

        public bool FinalOrder(string userName, int orderId)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            Order order = _context.Orders.Include(o => o.OrderDetail).FirstOrDefault(o => o.OrderId == orderId && !o.IsFinaly && o.UserId==userId);
            if (order == null)
                return false;

            if(_userService.BalanceUserWallet(userName)<order.OrderSum)
            {
                return false;
            }
            else
            {
                order.IsFinaly = true;
                _userService.AddWallet(new DataLayer.Entities.Wallet.Wallet()
                {
                    Amount = order.OrderSum,
                    CreateDate = DateTime.Now,
                    TypeId = 2,
                    UserId = userId,
                    Description = "تراکنش برداشت از حساب بابت فاکتورشماره :" + orderId,
                    IsPay=true
                });
                foreach(var detail in order.OrderDetail)
                {
                    _context.UserCourses.Add(new UserCourse()
                    {
                        UserId = userId,
                        CourseId = detail.CourseId
                    });
                }
                _context.SaveChanges();
                return true;
            }
        }

        public Order GetActiveOrderForShow(int orderId, int userId)
        {
            return _context.Orders.Include(o => o.OrderDetail).ThenInclude(od => od.Course)
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        }

        public List<Order> GetUserOrders(string userName)
        {
            int userId = _userService.GetUserIdByUserName(userName);
            return _context.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderId).ToList();
        }
    }
}
