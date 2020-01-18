using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs.Course;
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
      

        public IActionResult Index(int pageId, string filterTitle, string getType, string sortByType, int startPrice, int endPrice, List<int> selectedGroups, int take)
        {
             
            return View();
        }
    }
}