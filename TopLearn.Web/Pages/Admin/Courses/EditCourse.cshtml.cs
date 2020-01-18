using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class EditCourseModel : PageModel
    {

        private ICourseService _CourseService;
        public EditCourseModel(ICourseService courseService)
        {
            _CourseService = courseService;
        }

        [BindProperty]
        public Course Course { get; set; }

        public void OnGet(int id)
        {
            Course = _CourseService.GetCourseById(id);
        
           var groups= _CourseService.GetGroupForManageCourse();
           ViewData["Groups"] = new SelectList(groups,"Value","Text", Course.GroupId);

           var subGroups = _CourseService.GetSubGroupForManageCourse(Course.GroupId);
           ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text", Course.SubGroup??0);

           var teachers = _CourseService.GetTeachers();
           ViewData["Teachers"] = new SelectList(teachers, "Value", "Text", Course.TeacherId);

           var level = _CourseService.GetLevels();
           ViewData["Levels"]=new SelectList(level,"Value","Text", Course.LevelId);

           var statues = _CourseService.GetStatues();
           ViewData["Statues"]=new SelectList(statues,"Value","Text", Course.StatusId);


        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid)
                return Page();

            _CourseService.UpdateCourse(Course,imgCourseUp,demoUp);
            return RedirectToPage("Index");
        }
    }
}