using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using ParcelXpress.Helpers;

namespace ParcelXpress.Controllers
{
    public class IndexController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();   
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }

       
        [Authorize]
        public ActionResult Home(bool refreshRequired=false)
        {
            ViewBag.MessageCount = _db.DRVR_MSGS.Where(m => m.MessageReadInd == false).Count();
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
        [Authorize]
        public ActionResult Configuration()
        {
            var lastEmailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
            EmailAccount model = new EmailAccount();
            if(lastEmailAccount!=null ){
                model.EmailAddress = lastEmailAccount.EmailAddress;
                model.Password = lastEmailAccount.Password;
            }
            var items = from enumValue in Enum.GetValues(typeof(ParcelXpress.Enums.MailClients)).Cast<object>()
                        select new SelectListItem
                        {
                            Text = enumValue.ToString(),
                            Value = ((int)enumValue).ToString()
                        };
            ViewBag.MailClients = items;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveConfiguration(EmailAccount account)
        {
            try {
                EMAL_ACNT emailAccount = new EMAL_ACNT();
                emailAccount.EmailAddress = account.EmailAddress;
                emailAccount.Password = account.Password;
                emailAccount.EmailClient = account.MailProvider;
                emailAccount.ExecutionDate = DateTime.Now.ToUniversalTime();
                _db.EMAL_ACNT.Add(emailAccount);
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Configuration saved successfully.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Configuration could not be saved.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }


    }
}
