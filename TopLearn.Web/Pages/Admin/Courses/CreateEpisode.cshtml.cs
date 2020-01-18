using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class CreateEpisodeModel : PageModel
    {
        private ICourseService _courseService;

        public CreateEpisodeModel(ICourseService courseService)
        {
            _courseService = courseService; 
        }

        [BindProperty]
        public CourseEpisode CourseEpisode { get; set; }          
        public void OnGet(int id)
        {
            CourseEpisode=new CourseEpisode();
            CourseEpisode.CourseId = id;
        }

        public IActionResult OnPost(IFormFile episodefile)
        {
            if (!ModelState.IsValid || episodefile == null)
                return Page();
            if (_courseService.EpisodeFileNameExist(episodefile.FileName))
            {
                ViewData["EpisodeExist"] = true;
                return Page();
            }

            _courseService.AddEpisode(CourseEpisode, episodefile);
            return RedirectToPage("IndexEpisode");

        }
    }
}