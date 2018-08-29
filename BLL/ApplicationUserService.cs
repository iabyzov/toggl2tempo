using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Common;
using Data;
using Data.Entities;
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

        public void SaveTempoTokenForCurrentUser(string tempoToken)
        {
            var user = GetCurrentUser();
            user.TempoToken = tempoToken;
            _context.Update(user);

            _context.SaveChanges();
        }

        public void SaveTogglTokenForCurrentUser(string togglToken)
        {
            var user = GetCurrentUser();

            user.TogglToken = togglToken;
            _context.Update(user);

            _context.SaveChanges();
        }

        private UserEntity GetCurrentUser()
        {
            var loginName = _httpContextAccessor.HttpContext.User.Identity.GetNameId();

            var user = _context.Users.SingleOrDefault(u => u.Email == loginName);
            return user;
        }

        public string GetTogglTokenForCurrentUser()
        {
            var user = GetCurrentUser();

            return user.TogglToken;
        }

        public string GetTempoTokenForCurrentUser()
        {
            var user = GetCurrentUser();

            return user.TempoToken;
        }
    }
}