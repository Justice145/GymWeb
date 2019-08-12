using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication1.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [DataType(DataType.Text)]
        public string Address { get; set; }
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication1.Models.Branch> Branches { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Class> Classes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Class>(c => c.Classes).WithMany(i => i.Trainees)
                .Map(t => t.MapLeftKey("TraineeID")
                    .MapRightKey("ClassID")
                    .ToTable("TraineeClass"));
        } 
    }

    public class RoleNames
    {
        public const string ROLE_ADMINISTRATOR = "Admin";
        public const string ROLE_TRAINEE = "Trainee";
        public const string ROLE_TRAINER = "Trainer";
    }

}