using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testogsikkerhed_CICD.Models;

namespace Testogsikkerhed_CICD.Services
{
    public interface IUserService
    {
        public Task<User> Authenticate(string username, string password);
        public Task Create(User user, string password);
        public Task<User> GetOne(int id);
        public Task Update(User user, string password);
        public Task Delete(int id);
    }
}
