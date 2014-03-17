using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleApplication.Repo
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
    }
}