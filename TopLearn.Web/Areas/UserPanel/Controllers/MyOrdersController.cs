using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Order;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
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
            return View(_orderService.GetUserOrders(User.Identity.Name));
        }

        public IActionResult ShowOrder(int id,bool isFinaly=false)
        {
            int userId = _userService.GetUserIdByUserName(User.Identity.Name);
            Order order = _orderService.GetActiveOrderForShow(id, userId);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.IsFinaly = order.IsFinaly;

            return View(order);
        }

        public IActionResult FinalOrder(int id)
        {
            if (!_orderService.FinalOrder(User.Identity.Name, id))
                return NotFound();
            ViewBag.IsFinaly = true;
           return Redirect("/UserPanel/MyOrders/ShowOrder/"+id+"?isFinaly=true");

        }
    }
}