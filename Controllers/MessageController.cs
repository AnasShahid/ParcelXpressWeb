using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using ParcelXpress.Models;
using System.Data;
using ParcelXpress.Helpers;


namespace ParcelXpress.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();
        //
        // GET: /Message/

        public ActionResult MainPage(string partialActionName = null)
        {
            ViewBag.MessageCount = _db.DRVR_MSGS.Where(m => m.MessageReadInd == false).Count();
            return View();
        }


        public ActionResult Inbox(int page = 1)
        {
            var messages = loadInbox(page);
            return PartialView("_Inbox", messages);
        }

        public void clearRead()
        {
            var messageupdate = _db.DRVR_MSGS.Where(m => m.MessageReadInd == false);

            foreach (var item in messageupdate.ToList())
            {
                _db.Entry(item).State = EntityState.Modified;
                item.MessageReadInd = true;
                _db.SaveChanges();
            }
        }

        private object loadInbox(int pageNumber = 1)
        {
            var msgs = _db.DRVR_MSGS
                .Where(m => m.MessageReceivedInd == true)
                .OrderBy(m => m.MessageReadInd)
                .ThenByDescending(m => m.MessageDate)
                .ToPagedList(pageNumber, 10);
            return msgs;
        }

        public ActionResult Sent(int page = 1)
        {
            var msgs = _db.DRVR_MSGS
                .Where(m => m.MessageReceivedInd == false)
                .OrderByDescending(m => m.MessageDate)
                .ToPagedList(page, 10);
            return PartialView("_SentItems", msgs);
        }

        public ActionResult WriteNew()
        {
            var drivers = _db.DRVR_DATA.Where(d => d.IsDeleted != true).OrderBy(d => d.DriverName);
            var selectList = new List<SelectListItem>();
            foreach (var item in drivers)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.DriverId.ToString(),
                    Text = item.DriverName
                });
            }

            ViewBag.DriversList = selectList;
            return PartialView("_WriteNew");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sendToSingle(DRVR_MSGS drvrMsg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    drvrMsg.MessageDate = DateTime.Now.ToUniversalTime();
                    drvrMsg.MessageReceivedInd = false;
                    _db.DRVR_MSGS.Add(drvrMsg);
                    _db.SaveChanges();
                    SendMessagetoDriver(drvrMsg);
                    TempData["toastMessage"] = "<script>toastr.success('Message has been successfully sent to driver.');</script>";
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Message not sent.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An Error occured, message not sent.');</script>";
            }

            return RedirectToAction("MainPage");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sendToAll(DRVR_MSGS drvrMsg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    drvrMsg.MessageDate = DateTime.Now.ToUniversalTime();
                    drvrMsg.MessageReceivedInd = false;
                    _db.DRVR_MSGS.Add(drvrMsg);
                    _db.SaveChanges();
                    SendMessagetoDriver(drvrMsg);
                    TempData["toastMessage"] = "<script>toastr.success('Message has been successfully sent to driver.');</script>";
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Message not sent.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An Error occured, message not sent.');</script>";
            }

            return RedirectToAction("MainPage");
        }

        private void SendMessagetoDriver(DRVR_MSGS drvrMsg)
        {
            if (drvrMsg.DriverId != null && drvrMsg.DriverId > 0)
            {
                var driver = _db.DRVR_DATA.Find(drvrMsg.DriverId);
                GcmSender.SendToSingle(driver, drvrMsg.Message, "new_message");
            }
            else if (drvrMsg.DriverId == null)
            {
                var activeDrivers = _db.DRVR_DATA.Where(d => d.IsActive == true && d.IsDeleted != true);
                foreach (var driver in activeDrivers)
                {
                    GcmSender.SendToSingle(driver, drvrMsg.Message, "new_message");
                }
            }
        }

        public ActionResult _NewMessageDashboard()
        {
            var drivers = _db.DRVR_DATA.Where(d => d.IsDeleted != true).OrderBy(d => d.DriverName);
            var selectList = new List<SelectListItem>();
            foreach (var item in drivers)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.DriverId.ToString(),
                    Text = item.DriverName
                });
            }

            ViewBag.DriversList = selectList;
            ViewBag.MessageCount = _db.DRVR_MSGS.Where(m => m.MessageReadInd == false).Count();
            if ((int)ViewBag.MessageCount > 0)
            {
                TempData["toastMessageForInbox"] = "<script>toastr.info('You have unread messages in inbox.');</script>";
            }
            if (Request.IsAjaxRequest()) {
                return PartialView("_InboxCountDashboard");
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sendToSingleDashboard(DRVR_MSGS drvrMsg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    drvrMsg.MessageDate = DateTime.Now.ToUniversalTime();
                    drvrMsg.MessageReceivedInd = false;
                    _db.DRVR_MSGS.Add(drvrMsg);
                    _db.SaveChanges();
                    SendMessagetoDriver(drvrMsg);
                    TempData["toastMessage"] = "<script>toastr.success('Message has been successfully sent to driver.');</script>";
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Message not sent.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An Error occured, message not sent.');</script>";
            }

            return RedirectToAction("Dashboard", "Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sendToAllDashboard(DRVR_MSGS drvrMsg)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    drvrMsg.MessageDate = DateTime.Now.ToUniversalTime();
                    drvrMsg.MessageReceivedInd = false;
                    _db.DRVR_MSGS.Add(drvrMsg);
                    _db.SaveChanges();
                    SendMessagetoDriver(drvrMsg);
                    TempData["toastMessage"] = "<script>toastr.success('Message has been successfully sent to driver.');</script>";
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Message not sent.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An Error occured, message not sent.');</script>";
            }

            return RedirectToAction("Dashboard", "Index");
        }
    }
}
