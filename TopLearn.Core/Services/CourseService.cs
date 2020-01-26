using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.Generator;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.Core.Security;

namespace TopLearn.Core.Services
{
    public class CourseService :ICourseService
    {
        private TopLearnContext _context;
        public CourseService(TopLearnContext context)
        {
            _context = context;
        }

        public int AddCourse(Course course, IFormFile imgCourse, IFormFile courseDemo)
        {
            course.CreateDate = DateTime.Now;
            course.CourseImageName = "no-photo.jpg";
            //TODO CHECK IMAGE
            if(imgCourse !=null&& imgCourse.IsImage())
            {
                course.CourseImageName= NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imgpath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/image";

                imgpath = Path.Combine(imgpath, course.CourseImageName);
                using (var stream = new FileStream(imgpath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                //TODO IMAGE RESIZE

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/thumb";
                thumbPath = Path.Combine(thumbPath, course.CourseImageName);

                imgResizer.Image_resize(imgpath,thumbPath,150);
            }


            //TODO UPLOAD DEMO
            if(courseDemo!=null)
            {
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demopath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/demoes";

                demopath = Path.Combine(demopath, course.DemoFileName);
                using (var stream = new FileStream(demopath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }

            }

            _context.Courses.Add(course);
            _context.SaveChanges();
            return course.CourseId;
        }

        public int AddEpisode(CourseEpisode courseEpisode, IFormFile episodeFile)
        {
            courseEpisode.EpisodeFileName = episodeFile.FileName;

               
                string filepath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/courseFiles";

                filepath = Path.Combine(filepath, episodeFile.FileName);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    episodeFile.CopyTo(stream);
                }

                Course course = GetCourseById(courseEpisode.CourseId);
                course.UpdateDate = DateTime.Now;
                _context.CourseEpisodes.Add(courseEpisode);
                UpdateCourse(course);
                
                _context.SaveChanges();
              return  courseEpisode.EpisodeId;

        }

        public bool EpisodeFileNameExist(string fileName)
        {
            return _context.CourseEpisodes.Any(e => e.EpisodeFileName == fileName);
        }

        public List<CourseGroup> GetAllGroup()
        {
            return _context.CourseGroups.ToList();
        }

        public Tuple<List<ShowCourseListItemViewModel>, int> GetCourse(int pageId = 1, string filterTitle = "", string getType = "all", string sortByType = "Date", int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 0)
        {
            if (take == 0)
                take = 8;

            IQueryable<Course> result = _context.Courses;
            if (filterTitle != "")
            {
                result = result.Where(c => c.CourseTitle.Contains(filterTitle));
            }

            switch (getType)
            {
                case "all":
                    break;
                case  "free":
                    result = result.Where(c => c.Courseprice == 0);
                    break;
                case "buy":
                    result = result.Where(c => c.Courseprice != 0);
                    break;
            }

            switch (sortByType)
            {
                case "Data":
                    break;
                case "createDate":
                    result = result.OrderByDescending(c => c.CreateDate);
                    break;
                case "price":
                    result = result.OrderByDescending(c => c.Courseprice);
                    break;
                case "time":
                    result = result.OrderByDescending(c => c.CourseEpisodes.Sum(e => e.EpisodeTime.TotalMilliseconds));
                    break;
            }

            if (startPrice != 0)
            {
                result = result.Where(c => c.Courseprice >= startPrice);
            }

            if (endPrice != 0)
            {
                result = result.Where(c => c.Courseprice <= endPrice);
            }

            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (int groupID in selectedGroups)
                {
                    result = result.Where(c => c.GroupId == groupID || c.SubGroup == groupID);
                }
            }



            int pageCount = result.Include(c => c.CourseEpisodes).Select(c =>
                                new ShowCourseListItemViewModel()
                                {
                                    CourseId = c.CourseId,
                                    Courseprice = c.Courseprice,
                                    CourseTitle = c.CourseTitle,
                                    CourseImageName = c.CourseImageName,
                                    CourseTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
                                }).Count()/take;
            var query = result.Include(c => c.CourseEpisodes).Skip((pageId - 1) * take).Take(take).Select(c =>
                new ShowCourseListItemViewModel()
                {
                    CourseId = c.CourseId,
                    Courseprice = c.Courseprice,
                    CourseTitle = c.CourseTitle,
                    CourseImageName = c.CourseImageName,
                    CourseTime = new TimeSpan(c.CourseEpisodes.Sum(e => e.EpisodeTime.Ticks))
                }).ToList();

            return Tuple.Create(query, pageCount);
        }

        public Course GetCourseById(int courseId)
        {
            return _context.Courses.Find(courseId);
        }

        public CourseEpisode GetCourseEpisodeById(int episodeId)
        {
            return _context.CourseEpisodes.Find(episodeId);
        }

        public List<ShowCourseForAdminViewModel> GetCoursesForAdmin()
        {
            return _context.Courses.Select(c => new ShowCourseForAdminViewModel()
            {
                CourseId = c.CourseId,
                Title = c.CourseTitle,
                ImageName = c.CourseImageName,
                EpisodeCount = c.CourseEpisodes.Count()
            }).ToList();
        }

        public List<CourseEpisode> GetEpisodesByCourseId(int courseId)
        {
            return _context.CourseEpisodes.Where(e => e.CourseId == courseId).ToList();
        }

        public List<SelectListItem> GetGroupForManageCourse()
        {
            return _context.CourseGroups.Where(g => g.ParentId == null)
                .Select(g => new SelectListItem()
                {
                    Text = g.GroupTitle,
                    Value = g.GroupId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetLevels()
        {
            return _context.CourseLevels.Select(l => new SelectListItem()
            {
                Text = l.LevelTitle,
                Value = l.LevelId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetStatues()
        {
            return _context.CourseStatuses.Select(s => new SelectListItem()
            {
                Text = s.StatusTitle,
                Value = s.StatusId.ToString()
            }).ToList();
        }

        public List<SelectListItem> GetSubGroupForManageCourse(int groupId)
        {
            return _context.CourseGroups.Where(g => g.ParentId == groupId)
    .Select(g => new SelectListItem()
    {
        Text = g.GroupTitle,
        Value = g.GroupId.ToString()
    }).ToList();
        }

        public List<SelectListItem> GetTeachers()
        {
            return _context.UserRoles.Where(r => r.RoleId == 2).Include(u => u.User)
                .Select(u => new SelectListItem()
                {
                    Value = u.User.UserId.ToString(),
                    Text = u.User.UserName
                }).ToList();
        }

        public void UpdateCourse(Course course, IFormFile imgCourse=null, IFormFile courseDemo=null)
        {
            course.UpdateDate = DateTime.Now;

            if (imgCourse != null && imgCourse.IsImage())
            {
                if (course.CourseImageName != "no-photo.jpg")
                {
                    string oldfile = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/image";
                    string deleteImage = Path.Combine(oldfile, course.CourseImageName);
                    if (File.Exists(deleteImage))
                    {
                        File.Delete(deleteImage);
                    }

                    string oldthumb = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/thumb";
                    string deleteThumb = Path.Combine(oldfile, course.CourseImageName);
                    if (File.Exists(deleteThumb))
                    {
                        File.Delete(deleteThumb);
                    }
                }
                course.CourseImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(imgCourse.FileName);
                string imgpath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/image";

                imgpath = Path.Combine(imgpath, course.CourseImageName);
                using (var stream = new FileStream(imgpath, FileMode.Create))
                {
                    imgCourse.CopyTo(stream);
                }

                //TODO IMAGE RESIZE

                ImageConvertor imgResizer = new ImageConvertor();
                string thumbPath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/thumb";
                thumbPath = Path.Combine(thumbPath, course.CourseImageName);

                imgResizer.Image_resize(imgpath, thumbPath, 150);
            }


            //TODO UPLOAD DEMO
            if (courseDemo != null)
            {
                if (course.DemoFileName != null)
                {
                    string oldfile = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/demoes";
                   string  deleteDemo = Path.Combine(oldfile, course.DemoFileName);
                    if (File.Exists(deleteDemo))
                    {
                        File.Delete(deleteDemo);
                    }
                }
                course.DemoFileName = NameGenerator.GenerateUniqCode() + Path.GetExtension(courseDemo.FileName);
                string demopath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/course/demoes";

                demopath = Path.Combine(demopath, course.DemoFileName);
                using (var stream = new FileStream(demopath, FileMode.Create))
                {
                    courseDemo.CopyTo(stream);
                }

            }

            _context.Courses.Update(course);
            _context.SaveChanges();

        }

        public void UpdateEpisode(CourseEpisode courseEpisode, IFormFile episodeFile)
        {
            #region DeleteOldFile

            if (episodeFile != null)
            {
                string oldfile = Directory.GetCurrentDirectory().ToString() + "/wwwroot/courseFiles";
                string deleteImage = Path.Combine(oldfile, courseEpisode.EpisodeFileName);
                if (File.Exists(deleteImage))
                {
                    File.Delete(deleteImage);
                }

                courseEpisode.EpisodeFileName = "";
            }

            #endregion

            #region SaveNewFile

            if (episodeFile != null)
            {
                string episodePath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/courseFiles";

                episodePath = Path.Combine(episodePath, episodeFile.FileName);
                using (var stream = new FileStream(episodePath, FileMode.Create))
                {
                    episodeFile.CopyTo(stream);
                }
                courseEpisode.EpisodeFileName = episodeFile.FileName;
            }

            #endregion

           
            _context.CourseEpisodes.Update(courseEpisode);
            _context.SaveChanges();

        }
    }
}
