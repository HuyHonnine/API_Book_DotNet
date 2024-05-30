﻿using Microsoft.EntityFrameworkCore;
using TestWebAPI.Models;

namespace TestWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasOne(b=>b.Category).WithMany(c => c.Books).HasForeignKey(b=>b.CategoryId);
            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.roleCode).HasPrincipalKey(r => r.code);
            modelBuilder.Entity<JWT>().HasOne(j => j.user).WithMany(u => u.JWTs).HasForeignKey(j => j.user_id).HasPrincipalKey(u => u.id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<JWT> JWTs { get; set; }

    }
}
