﻿using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    interface IUserRepository
    {
        bool IsUniqueUser(string userName);
        User Authenticate(string username, string password);
        User Register(string username, string password);
    }
}
