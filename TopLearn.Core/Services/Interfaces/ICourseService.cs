using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TopLearn.Core.DTOs.Course;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Core.Services.Interfaces
{
  public  interface ICourseService
    {
        #region Group
        List<CourseGroup> GetAllGroup();
        List<SelectListItem> GetGroupForManageCourse();
        List<SelectListItem> GetSubGroupForManageCourse(int groupId);
        List<SelectListItem> GetTeachers();
        List<SelectListItem> GetLevels();
        List<SelectListItem> GetStatues();


        #endregion

        #region Course
        List<ShowCourseForAdminViewModel> GetCoursesForAdmin();
        int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Course GetCourseById(int courseId);
        void UpdateCourse(Course course, IFormFile imgCourse, IFormFile courseDemo);
        Tuple<List<ShowCourseListItemViewModel>,int>  GetCourse(int pageId=1,string filterTitle = "",string getType="all",string sortByType="Date",int startPrice=0,int endPrice=0,List<int> selectedGroups = null,int take=0);
        Course GetCourseForShow(int id);

        #endregion

        #region Episode
        List<CourseEpisode> GetEpisodesByCourseId(int courseId);
        int AddEpisode(CourseEpisode courseEpisode, IFormFile episodeFile);
        bool EpisodeFileNameExist(string fileName);
        CourseEpisode GetCourseEpisodeById(int episodeId);
        void UpdateEpisode(CourseEpisode courseEpisode,IFormFile episodeFile);
        

        #endregion
    }
}
