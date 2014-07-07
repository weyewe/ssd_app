using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using SampleApplication.Models;
using SampleApplication.Service;

namespace SampleApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly static log4net.ILog LOG = log4net.LogManager.GetLogger("AccountController");

        private IUserService _userService;

        public AccountController()
        {
            _userService = new UserService();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountModel.LoginModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //TODO: validate user password, save to session, etc
                    string password = user.Password.Trim();
                    string strUser = user.UserName.Trim();
                    //password = StringEncryptor.Encrypt(password);


                    if (ModelState.IsValid)
                    {
                        var objUser = _userService.IsLoginValid(strUser, password);

                        if (objUser == null)
                        {
                            ModelState.AddModelError("", "Invalid credential");
                        }
                        else
                        {
                            int SessionTime = 120;
                            string strSessionTime = System.Configuration.ConfigurationManager.AppSettings["SessionTime"];
                            if (!String.IsNullOrEmpty(strSessionTime))
                            {
                                if (!int.TryParse(strSessionTime, out SessionTime))
                                    SessionTime = 120;
                            }

                            FormsAuthenticationTicket tkt;
                            string cookiestr;
                            HttpCookie ck;
                            tkt = new FormsAuthenticationTicket(1,
                                objUser.UserId.ToString(),
                                DateTime.Now,
                                DateTime.Now.AddMinutes(SessionTime),
                                user.RememberMe,
                                objUser.UserTypeId + "#" + objUser.FirstName + " " + objUser.LastName);
                            cookiestr = FormsAuthentication.Encrypt(tkt);
                            ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
                            if (user.RememberMe)
                                ck.Expires = tkt.Expiration;
                            ck.Path = FormsAuthentication.FormsCookiePath;
                            Response.Cookies.Add(ck);

                            LOG.Debug("Login Success, Username: "+ user.UserName);

                            // Redirect to requested URL, or homepage if no previous page requested
                            string returnUrl = Request.QueryString["ReturnUrl"];
                            if (!String.IsNullOrEmpty(returnUrl))
                                return Redirect(returnUrl);

                            return RedirectToAction("Index", "Reimburse");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Login Failed, Please try Again or Contact Your Administrator.");
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
