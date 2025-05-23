﻿using BannedBooks.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace BannedBooks.Data;

public class BannedBooksContext : IdentityDbContext<IdentityUser>
{
    public BannedBooksContext(DbContextOptions<BannedBooksContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
