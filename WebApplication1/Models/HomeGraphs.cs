using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public class NameAndCount
    {
        public String Name { get; set; }
        public float Count { get; set; }
    }


    public class HomeGraphs
    {
        public List<NameAndCount> ClassesPerBranch { get; set; }
    }
}