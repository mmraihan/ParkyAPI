﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appsettings)
        {
            _db = db;
            _appSettings = appsettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(c => c.UserName == username && c.Password == password);
           
            if (user==null)
            {
                return null;
            }

            //if user was found, then generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials
                                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";

            return user;

        }

        public bool IsUniqueUser(string userName)
        {
            var user = _db.Users.SingleOrDefault(x => x.UserName == userName);
            if (user==null)
            {
                return true;
            }
            return false;
        }

        public User Register(string username, string password)
        {
            User userObj = new User()
            {
                UserName= username,
                Password=password,
            };

            _db.Users.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";
            return userObj;

        }
    }
}
