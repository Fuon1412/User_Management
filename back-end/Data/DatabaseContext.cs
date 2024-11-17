using Microsoft.EntityFrameworkCore;
using back_end.Models.Client;
using back_end.Models.Project;


namespace back_end.Data
{
    public class DatabaseContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {
        //Thiet lap cac bang trong database
        //Bang User
        public required DbSet<User> Accounts { get; set; }
        public required DbSet<UserInfor> UserInfors { get; set; }
        //Bang Project
        public required DbSet<Project> Projects { get; set; }
        public required DbSet<Stage> Stages  { get; set; }
        public required DbSet<Models.Project.Task> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(entity =>
            {
                entity.Ignore(e => e.PhoneNumber);
                entity.Ignore(e => e.PhoneNumberConfirmed);
                entity.Ignore(e => e.TwoFactorEnabled);
                entity.Ignore(e => e.LockoutEnd);
                entity.Ignore(e => e.LockoutEnabled);
                entity.Ignore(e => e.AccessFailedCount);
                entity.Ignore(e => e.NormalizedEmail);
                entity.Ignore(e => e.NormalizedUserName);
                entity.Ignore(e => e.ConcurrencyStamp);
                entity.Ignore(e => e.UserName);
                entity.Property(e => e.PasswordHash).IsRequired();
            });
        }
    }

}