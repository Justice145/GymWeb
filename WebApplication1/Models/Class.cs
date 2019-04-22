using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.DateTime), Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }
        
        public int TrainerID { get; set; }

        public virtual User Trainer { get; set; }

        public virtual ICollection<User> Trainees { get; set; }
        
        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }
    }
}