using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using System.Linq;

namespace SampleApplication.Models
{
    public class AccountModel
    {
        public const int UserTypeEmployee = 1;
        public const int UserTypeCashier = 2;

        public static int GetUserId()
        {
            int userId = 0;
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get Forms Identity From Current User
                        FormsIdentity id = (FormsIdentity)
                        HttpContext.Current.User.Identity;

                        // Get Forms Ticket From Identity object
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Retrieve stored user-data (role information is assigned 
                        // when the ticket is created, separate multiple roles 
                        // with commas) 
                        string strUserId = ticket.Name;
                        userId = int.Parse(strUserId);
                    }
                }
            }
            return userId;
        }//end function GetUserCode

        public static int GetUserTypeId()
        {
            int userTypeId = 0;
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get Forms Identity From Current User
                        FormsIdentity id = (FormsIdentity)
                        HttpContext.Current.User.Identity;

                        // Get Forms Ticket From Identity object
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Retrieve stored user-data (role information is assigned 
                        // when the ticket is created, separate multiple roles 
                        // with commas) 
                        string[] userDetail = ticket.UserData.Split('#');
                        if (!String.IsNullOrEmpty(userDetail[0]))
                        {
                            string strUserTypeId = userDetail[0];
                            userTypeId = int.Parse(strUserTypeId);
                        }
                    }
                }
            }
            return userTypeId;
        }//end function GetUserTypeId

        public static string GetUserName()
        {
            string userName = "";
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get Forms Identity From Current User
                        FormsIdentity id = (FormsIdentity)
                        HttpContext.Current.User.Identity;

                        // Get Forms Ticket From Identity object
                        FormsAuthenticationTicket ticket = id.Ticket;

                        // Retrieve stored user-data (role information is assigned 
                        // when the ticket is created, separate multiple roles 
                        // with commas) 
                        string[] userDetail = ticket.UserData.Split('#');
                        if (!String.IsNullOrEmpty(userDetail[1]))
                            userName = userDetail[1];
                    }
                }
            }
            return userName;
        }//end function GetUserName
        public class LoginModel
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            public int UserId { get; set; }
            public int UserTypeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}