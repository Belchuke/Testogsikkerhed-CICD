﻿using Microsoft.EntityFrameworkCore;
using Testogsikkerhed_CICD.Models;

namespace Testogsikkerhed_CICD.DataAccess
{
    public class Context : DbContext, IContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}