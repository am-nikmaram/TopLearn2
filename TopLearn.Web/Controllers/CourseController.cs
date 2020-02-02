using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        private ICourseService _courseService;
        private IOrderService _orderService;
        private IUserService _userService;

        public CourseController(ICourseService courseService, IOrderService orderService, IUserService userService)
        {
            _courseService = courseService;
            _orderService = orderService;
            _userService = userService;
        }
        public IActionResult Index(int pageId = 1, string filterTitle = "", string getType = "all", string sortByType = "Data", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.pageId = pageId;
            ViewBag.courseType = getType;
            ViewBag.sortType = sortByType;
            ViewBag.filter = filterTitle;
            ViewBag.selectedgroups = selectedGroups;
            ViewBag.groups = _courseService.GetAllGroup();
            return View(_courseService.GetCourse( pageId , filterTitle ,  getType , sortByType ,  startPrice , endPrice, selectedGroups ,9));
        }

        [Route("ShowCourse/{id}")]
        public IActionResult ShowCourse(int id)
        {
            var result = _courseService.GetCourseForShow(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [Authorize]
        public IActionResult BuyCourse(int id)
        {
            int uId = _userService.GetUserIdByUserName(User.Identity.Name);
            _orderService.AddOrder(uId, id);
            return Redirect("/");
        }
    }
}