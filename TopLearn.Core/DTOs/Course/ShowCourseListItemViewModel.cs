using System;

namespace TopLearn.Core.DTOs.Course
{
   public class ShowCourseListItemViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int Courseprice { get; set; }
        public string CourseImageName { get; set; }
        public TimeSpan CourseTime { get; set; }
    }
}
