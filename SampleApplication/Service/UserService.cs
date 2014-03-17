using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SampleApplication.Models;
using SampleApplication.Repo;

namespace SampleApplication.Service
{
    public class UserService : IUserService, IDisposable
    {
        private readonly IUserRepository _userRepo;

        public UserService()
        {
            _userRepo = new UserRepository();
        }

        public AccountModel.LoginModel IsLoginValid(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && username.Trim() != "" && !string.IsNullOrEmpty(password) && password.Trim() != "")
            {
                username = username.Trim();
                password = password.Trim();
                var u = _userRepo.Find(us => us.UserName == username && us.Password == password);
                if (u != null)
                {
                    AccountModel.LoginModel model = new AccountModel.LoginModel();
                    model.UserId = u.Id;
                    model.FirstName = u.FirstName;
                    model.LastName = u.LastName;
                    model.UserTypeId = u.UserTypeId.HasValue ? u.UserTypeId.Value :0;

                    return model;
                }
            }

            return null;
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _userRepo.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}