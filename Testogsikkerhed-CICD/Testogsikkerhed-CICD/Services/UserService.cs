using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testogsikkerhed_CICD.DataAccess;
using Testogsikkerhed_CICD.Exceptions;
using Testogsikkerhed_CICD.Models;

namespace Testogsikkerhed_CICD.Services
{
    public class UserService : IUserService
    {
        private readonly IContext _context;
        public UserService(IContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
            => await _context.Users.AsNoTracking().ToListAsync();

        public async Task<User> GetOne(int id)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException();

        public async Task<User> Authenticate(string name, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                throw new ServiceException("Username or password is incorrect.");

            User user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == name);

            if (user == null)
                throw new ServiceException("Username or password is incorrect.");

            try
            {
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    throw new ServiceException("Username or password is incorrect.");
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceException(ex.Message);
            }
            catch (ArgumentException)
            {
                //TODO: This exception means that we probably stored a wrong hash or salt - or maybe changed hashing algorithm.
                //      These should probably be logged or something.
            }

            return user;
        }

        public async Task Create(User user, string password)
        {
            if (user.Id != default)
                throw new ServiceException("Please remove the Id. It is generated, not set.");

            if (await _context.Users.AnyAsync(u => u.Name == user.Name))
                throw new ServiceException("Name \"" + user.Name + "\" is already taken.");
            try
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceException(ex.Message);
            }

            _context.Users.Attach(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            User user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new NotFoundException();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Hashes the <paramref name="password"/> and outputs the hash in <paramref name="passwordHash"/> and hash salt in <paramref name="passwordSalt"/>.
        /// </summary>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password), "Please enter a valid password.");

            //HMAC: Hash-based Message Authentication Code
            var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        /// <summary>
        /// Hashes the <paramref name="password"/> with the given <paramref name="storedSalt"/> and compares it to <paramref name="storedHash"/>.
        /// </summary>
        /// <returns>Whether or not the <paramref name="password"/> is valid.</returns>
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password), "Please enter a valid password.");
            if (storedHash.Length != 64)  throw new ArgumentException("Invalid length of password hash (64 bytes expected)." , nameof(storedHash));
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

            //HMAC: Hash-based Message Authentication Code
            //Init a HMAC with the same salt and compute the passwords hash - if it's the same as the stored hash, the password is valid.
            var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return Enumerable.SequenceEqual(computedHash, storedHash);
        }
    }
}
