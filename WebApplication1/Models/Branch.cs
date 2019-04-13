using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public TimeSpan WeekDayOpen { get; set; }
        public TimeSpan WeekDayClose { get; set; }
        public TimeSpan FridayOpen { get; set; }
        public TimeSpan FridayClose { get; set; }
        public TimeSpan SaturdayOpen { get; set; }
        public TimeSpan SaturdayClose { get; set; }
    }
}