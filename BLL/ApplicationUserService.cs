using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using BLL.Database;
using BLL.Database.Entities;
using Microsoft.AspNetCore.Http;

namespace BLL
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly SyncDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationUserService(SyncDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public UserEntity Get(string login)
        {
            var foundUser = _context.Users.FirstOrDefault(
                u => u.Email == login);

            return foundUser;
        }

        public int AddUserIfNecessary(string login)
        {
            var user = Get(login);
            if (user != null)
            {
                return user.Id;
            }

            var newUser = new UserEntity
            {
                Email = login
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Get(login).Id;
        }

        public void SaveTogglTokenForCurrentUser(string togglToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
            if (userIdClaim == null)
            {
                throw new NullReferenceException($"{ClaimsIdentity.DefaultNameClaimType} is not presented in claims");
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                throw new InvalidOperationException($"{ClaimsIdentity.DefaultNameClaimType} has not integer value");
            }

            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            user.TogglToken = togglToken;
            _context.Update(user);

            _context.SaveChanges();
        }

        public string GetTogglTokenForCurrentUser()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
            if (userIdClaim == null)
            {
                throw new NullReferenceException($"{ClaimsIdentity.DefaultNameClaimType} is not presented in claims");
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                throw new InvalidOperationException($"{ClaimsIdentity.DefaultNameClaimType} has not integer value");
            }

            var user = _context.Users.SingleOrDefault(u => u.Id == userId);

            return user.TogglToken;
        }
    }
}