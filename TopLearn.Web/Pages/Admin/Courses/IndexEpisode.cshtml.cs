using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Admin.Courses
{
    public class IndexEpisodeModel : PageModel
    {
         ICourseService _courseService;

         public IndexEpisodeModel(ICourseService courseService)
         {
             _courseService = courseService;        
         }
 
        public List<CourseEpisode> CourseEpisodes { get; set; }          

        public void OnGet(int id)
        {
            
            CourseEpisodes = _courseService.GetEpisodesByCourseId(id);
        }
    }
}