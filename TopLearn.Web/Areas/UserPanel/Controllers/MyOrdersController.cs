using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    public class MyOrdersController : Controller
    {
        private IOrderService _orderService;
        private IUserService _userService;

        public MyOrdersController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowOrder(int id,bool isFinaly=false)
        {
            int userId = _userService.GetUserIdByUserName(User.Identity.Name);
            Order order = _orderService.GetActiveOrderForShow(id, userId);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.IsFinaly = isFinaly;

            return View(order);
        }
    }
}