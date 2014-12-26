using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ExpenseMeMVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Logon()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logon(string userId, string password)
        {

            if (String.IsNullOrWhiteSpace(userId)) {
                ModelState.AddModelError("UserName", "The user name or password provided is incorrect.");
                return Logon();
            } 

            FormsAuthentication.SetAuthCookie(userId.ToUpper(), true);

            return RedirectToAction("index","home");
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("index", "home");
        }
    }
}