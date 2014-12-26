using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ExpenseMeMVC.Infrastructure
{
    public interface ISessionState
    {
        string UserName { get; }
    }

    public class SessionState : ISessionState
    {

        public string UserName
        {
            get { return GetUserName(); }
        }

        private string GetUserName()
        {
            var userName = "";

            if (HttpContext.Current.Request.IsAuthenticated)
            {
                userName = HttpContext.Current.User.Identity.Name;
            }

            return userName;
        }
    }
}