using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class CreateCourseModel : PageModel
    {
        private ICourseService _CourseService;
        public CreateCourseModel(ICourseService courseService)
        {
            _CourseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet()
        {
            var groups = _CourseService.GetGroupForManageCourse();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text");

            var subgroups = _CourseService.GetSubGroupForManageCourse(int.Parse(groups.First().Value));
            ViewData["SubGroups"] = new SelectList(subgroups, "Value", "Text");

            var Teachers = _CourseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(Teachers, "Value", "Text");

            var Levels = _CourseService.GetLevels();
            ViewData["Levels"] = new SelectList(Levels, "Value", "Text");

            var Statues = _CourseService.GetStatues();
            ViewData["Statues"] = new SelectList(Statues, "Value", "Text");
        }
        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            if(!ModelState.IsValid)
                return Page();

            _CourseService.AddCourse(Course, imgCourseUp, demoUp);

            return RedirectToPage("Index");
        }

    }
}