using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleApplication.Repo
{
    public interface IUserRepository : IRepository<User>
    {
    }
}