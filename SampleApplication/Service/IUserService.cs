using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApplication.Models;

namespace SampleApplication.Service
{
    public interface IUserService
    {
        AccountModel.LoginModel IsLoginValid(string username, string password);
    }
}