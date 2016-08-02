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
            return RedirectToAction("Dashboard", "Index");
            //ViewBag.MessageCount = _db.DRVR_MSGS.Where(m => m.MessageReadInd == false).Count();
            //return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
        


    }
}
