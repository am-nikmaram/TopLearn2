using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class EditEpisodeModel : PageModel
    {
        private ICourseService _courseService;

        public EditEpisodeModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [BindProperty]
        public CourseEpisode CourseEpisode { get; set; }      
        public void OnGet(int id)
        {
            CourseEpisode = _courseService.GetCourseEpisodeById(id);
        }

        public IActionResult OnPost(IFormFile episodefile)
        {
            if (!ModelState.IsValid)
                return Page();

            _courseService.UpdateEpisode(CourseEpisode,episodefile);

            return Redirect("/Admin/Courses/IndexEpisode/" + CourseEpisode.CourseId);
         
        }
        
    }
}