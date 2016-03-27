using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using System.Web.Security;

namespace ParcelXpress.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Index/
       
        [HttpGet]
        public ActionResult MainScreen()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MainScreen(SYS_USER user)
        { 
            if(ModelState.IsValid)
            {
                using (Models.ParcelXpressConnection pxpEntities = new ParcelXpressConnection())
                {
                    var validUser = pxpEntities.SYS_USER.Where(a => a.UserName == user.UserName && a.Password == user.Password).FirstOrDefault();

                    if (validUser != null)
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, false);
                        return RedirectToAction("Dashboard", "Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login data is incorrect!");
                        
                    }
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            TempData["toastMessage"] = "<script>toastr.success('You have been logged out of the system successfully.');</script>";
            return RedirectToAction("MainScreen", "Login");
        }

    }
}
