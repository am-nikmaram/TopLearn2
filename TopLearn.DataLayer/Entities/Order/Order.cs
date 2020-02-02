using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopLearn.DataLayer.Entities.Order
{
   public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int OrderSum { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public bool IsFinaly { get; set; }


        public virtual User.User User { get; set; }
        public virtual List<OrderDetail> OrderDetail { get; set; }


    }
}
