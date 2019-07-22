using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class RegisterToClassViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Class> Classes { get; set; }
    }
}