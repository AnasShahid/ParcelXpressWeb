using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using PagedList;
using System.Data;


namespace ParcelXpress.Controllers
{
    [Authorize]
    public class FAQController : Controller
    {
        //
        // GET: /FAQ/
        ParcelXpressConnection _db = new ParcelXpressConnection();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowFAQS(int page = 1)
        {
            var model = _db.FAQS.OrderByDescending(f => f.Date).ToPagedList(page, 15);
            return View(model);
        }
        public ActionResult AddFaq()
        {
            return View();
        }
        public ActionResult EditFaq(int faqId)
        {
            var model = _db.FAQS.Find(faqId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveNewFaq(FAQ faq)
        {
            try
            {
                faq.Date = DateTime.Now.ToUniversalTime();
                if (ModelState.IsValid)
                {
                    
                    _db.FAQS.Add(faq);
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('FAQ has been successfully saved into the system.');</script>";
                }
                else 
                {
                    TempData["toastMessage"] = "<script>toastr.error('FAQ could not be saved.');</script>"; 
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('FAQ could not be saved.');</script>"; 
            }
            return RedirectToAction("ShowFAQS");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveEditFaq(FAQ faq)
        {
            try
            {
                faq.Date = DateTime.Now.ToUniversalTime();
                if (ModelState.IsValid)
                {
                    _db.Entry(faq).State = EntityState.Modified;
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('FAQ has been successfully saved.');</script>";
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('FAQ could not be saved.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('FAQ could not be saved.');</script>";
            }
            return RedirectToAction("ShowFAQS");
        }

        public ActionResult DeleteFaq(int FaqId)
        {
            try
            {
                var data = _db.FAQS.Find(FaqId);
                if (data != null)
                {
                    _db.Entry(data).State = EntityState.Deleted;
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('FAQ has been successfully deleted.');</script>";
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('FAQ could not be deleted.');</script>"; 
            }
            return RedirectToAction("ShowFAQS");
        }
    }
}
