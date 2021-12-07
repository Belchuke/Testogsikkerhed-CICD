using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Testogsikkerhed_CICD.Models;

namespace Testogsikkerhed_CICD.DataAccess
{
    public interface IContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public DbSet<User> Users { get; set; }
    }
}
