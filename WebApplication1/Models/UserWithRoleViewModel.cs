using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UserWithRoleViewModel
    {
        public String Id { get; set; }
        public String PhoneNumber { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public string UserName { get; set; }
        public List<string> RoleNames { get; set; }
        public string Address { get; set; }
        public ICollection<Class> Classes { get; set; }
    }
}