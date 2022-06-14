using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Chat.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<Friends> Friends { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

                builder.Entity<ApplicationUser>()
                    .Property(user => user.Discriminator).HasDefaultValue("ApplicationUser");
            }
    }
}
