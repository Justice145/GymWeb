using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ClassSearchViewModel
    {
        public List<Branch> Branches { get; set; }   
        public List<String> ClassNames { get; set;}
    }
}