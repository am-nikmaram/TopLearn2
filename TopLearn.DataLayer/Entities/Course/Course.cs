using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TopLearn.DataLayer.Entities.Course
{
    public class Course
    {
         [Key]
         public int CourseId { get; set; }
        [Required]
        public int GroupId { get; set; }

        public int? SubGroup { get; set; }
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int LevelId { get; set; }

        public bool  IsDelete { get; set; }



        [Display(Name ="عنوان دوره")]
        [Required(ErrorMessage ="لطفاً {0} را وارد کنید")]
        [MaxLength(450)]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage ="لطفاً {0} را وارد کنید")]
        [Display(Name ="شرح دوره")]
        public string CourseDescription { get; set; }

        [Display(Name ="قیمت دوره")]
        [Required(ErrorMessage ="لطفاً {0} را وارد کنید")]
        public int Courseprice { get; set; }

        public CourseStatus CourseStatus { get; set; }

        public CourseLevel CourseLevel { get; set; }

        [MaxLength(600)]
        public string Tags { get; set; }

        [MaxLength(50)]
        public string CourseImageName { get; set; }

        [MaxLength(100)]
        public string DemoFileName { get; set; }


        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }



        #region Relation


        [ForeignKey("TeacherId")]
        public User.User User { get; set; }

        [ForeignKey("GroupId")]
        public CourseGroup CourseGroup { get; set; }

        [ForeignKey("SubGroup")]
        public CourseGroup Group { get; set; }

        public List<CourseEpisode> CourseEpisodes { get; set; }
        public List<UserCourse> UserCourses { get; set; }


        #endregion

    }
}
