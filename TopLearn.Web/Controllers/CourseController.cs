using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        private ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public IActionResult Index(int pageId = 1, string filterTitle = "", string getType = "all", string sortByType = "Data", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.courseType = getType;
            ViewBag.sortType = sortByType;
            ViewBag.filter = filterTitle;
            ViewBag.selectedgroups = selectedGroups;
            ViewBag.groups = _courseService.GetAllGroup();
            return View(_courseService.GetCourse( pageId , filterTitle ,  getType , sortByType ,  startPrice , endPrice, selectedGroups ,9));
        }
    }
}