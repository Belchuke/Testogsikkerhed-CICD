using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testogsikkerhed_CICD.Models;

namespace Testogsikkerhed_CICD.DataAccess
{
    public class TestContext : DbContext, IContext
    {
        public TestContext(DbContextOptions<TestContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
