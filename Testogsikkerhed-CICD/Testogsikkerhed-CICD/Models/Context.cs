﻿using Microsoft.EntityFrameworkCore;

namespace Testogsikkerhed_CICD.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<user> Users { get; set; }
    }
}