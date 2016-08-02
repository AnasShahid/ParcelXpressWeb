using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using ParcelXpress.Enums;
using ParcelXpress.Helpers;
using PagedList;
using System.Data.Objects;
using System.Data;

namespace ParcelXpress.Controllers
{
    public class InvoiceController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        #region Views 

        public ActionResult AllInvoices(int page = 1, string fromDate = null, string toDate = null)
        {
            var model = _db.CUST_INVC.Where(i =>  i.InvoiceStatus != StringEnum.GetStringValue(InvoiceStatus.Invalid)).OrderByDescending(p => p.InvoiceDate).ToList();
            if (fromDate != null && !fromDate.Trim().Equals(""))
            {
                DateTime startDate = Convert.ToDateTime(fromDate);
                model = model.Where(p => p.InvoiceDate.Date >= startDate.Date).ToList();
            }
            if (toDate != null && !toDate.Trim().Equals(""))
            {
                DateTime endDate = Convert.ToDateTime(toDate);
                model = model.Where(p => p.InvoiceDate.Date <= endDate.Date).ToList();
            }
            var finalModel = model.ToPagedList(page, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_InvoiceTable", finalModel);
            }
            ViewBag.TotalJobs = model.Count();
            ViewBag.TotalAmount = model.Sum(p => p.InvoiceAmount.GetValueOrDefault(0));
            ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(finalModel);
        }

        public ActionResult PaidInvoices(int page = 1, string fromDate = null, string toDate = null)
        {
            var model = _db.CUST_INVC.Where(i => i.IsPaid == true && i.InvoiceStatus != StringEnum.GetStringValue(InvoiceStatus.Invalid)).OrderByDescending(p => p.InvoiceDate).ToList();
            if (fromDate != null && !fromDate.Trim().Equals(""))
            {
                DateTime startDate = Convert.ToDateTime(fromDate);
                model = model.Where(p => p.InvoiceDate.Date >= startDate.Date).ToList();
            }
            if (toDate != null && !toDate.Trim().Equals(""))
            {
                DateTime endDate = Convert.ToDateTime(toDate);
                model = model.Where(p => p.InvoiceDate.Date <= endDate.Date).ToList();
            }
            var finalModel = model.ToPagedList(page, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_InvoiceTable", finalModel);
            }
            ViewBag.TotalJobs = model.Count();
            ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            ViewBag.PaidByCash = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Cash).Sum(p => p.PaidAmount);
            ViewBag.PaidByBacs = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Bacs).Sum(p => p.PaidAmount);
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(finalModel);
        }

        public ActionResult PendingInvoices(int page = 1, string fromDate = null, string toDate = null)
        {
            var model = _db.CUST_INVC.Where(i => i.IsPaid == false&& i.InvoiceStatus != StringEnum.GetStringValue(InvoiceStatus.Invalid)).OrderByDescending(p => p.InvoiceDate).ToList();
            if (fromDate != null && !fromDate.Trim().Equals(""))
            {
                DateTime startDate = Convert.ToDateTime(fromDate);
                model = model.Where(p => p.InvoiceDate.Date >= startDate.Date).ToList();
            }
            if (toDate != null && !toDate.Trim().Equals(""))
            {
                DateTime endDate = Convert.ToDateTime(toDate);
                model = model.Where(p => p.InvoiceDate.Date <= endDate.Date).ToList();
            }
            var finalModel = model.ToPagedList(page, 20);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_InvoiceTable", finalModel);
            }
            //ViewBag.TotalJobs = model.Count();
            //ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            //ViewBag.PaidByCash = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Cash).Sum(p => p.PaidAmount);
            //ViewBag.PaidByBacs = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Bacs).Sum(p => p.PaidAmount);
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(finalModel);
        }
        public ActionResult _InvoicePayableSummary(int InvoiceId = 0)
        {
            var invoicePaymentModes = _db.INVC_PYMT_MODE.Where(pm => pm.IsActive == true).OrderBy(pm => pm.PaymentModeDsc);
            var selectList = new List<SelectListItem>();
            foreach (var item in invoicePaymentModes)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.PaymentModeId.ToString(),
                    Text = item.PaymentModeDsc
                });
            }

            ViewBag.PaymentModeList = selectList;
            CUST_INVC model = new CUST_INVC();
            if (InvoiceId != 0)
            {
                model = _db.CUST_INVC.Find(InvoiceId);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_InvoiceSettlement", model);
            }
            return PartialView(model);
        }
        #endregion

        #region PostEvents
        public ActionResult SettleInvoice(CUST_INVC invoice)
        {
            try
            {
                if (invoice.InvoicePaymentMode == null || invoice.InvoicePaymentMode.Value < 0)
                {
                    throw new Exception("Please select a payment mode to settle invoice");
                }
                var thisInvoice = _db.CUST_INVC.Find(invoice.InvoiceId);
                thisInvoice.PaidAmount = thisInvoice.InvoiceAmount;
                
                if (thisInvoice.CustomerId.GetValueOrDefault(0) > 0) {
                    CUST_BILL bill = new CUST_BILL() { CustomerId = thisInvoice.CustomerId.Value, DueAmount = thisInvoice.InvoiceAmount.GetValueOrDefault(0), PaidAmount = thisInvoice.PaidAmount.GetValueOrDefault(0), PaymentDate = DateTime.Now.ToUniversalTime() };
                    bill.RemainingAmount = bill.DueAmount - (bill.PaidAmount);
                    bill.PreviousCredit = 0;
                    _db.CUST_BILL.Add(bill);
                }
                var balance = thisInvoice.PaidAmount.Value ;
                var settlingtransactions = _db.CUST_TRAN.Where(t => t.InvoiceId == thisInvoice.InvoiceId);

                foreach (var transaction in settlingtransactions)
                {
                    if (balance == 0)
                    { break; }
                    if (transaction.RemainingAmount.GetValueOrDefault(0) <= balance)
                    {
                        balance = balance - transaction.RemainingAmount.GetValueOrDefault(0);
                        transaction.ReceivedAmount = transaction.ReceivedAmount.GetValueOrDefault(0) + transaction.RemainingAmount.GetValueOrDefault(0);
                        transaction.SettledInd = true;
                    }
                    else
                    {
                        transaction.ReceivedAmount = transaction.ReceivedAmount.GetValueOrDefault(0) + balance;
                        balance = 0;
                    }
                    _db.Entry(transaction).State = EntityState.Modified;
                }
                
                if (balance > 0) // Save remaining amount to customer credit.
                {
                    CUST_CRDT newCredit = new CUST_CRDT() { CustomerId = thisInvoice.CustomerId, Date = (DateTime.Now).ToUniversalTime(), CreditAmount = balance, RemainingAmount = balance, SettledAmount = 0, SettledInd = false };
                    _db.CUST_CRDT.Add(newCredit);
                }
                thisInvoice.PaidDate = DateTime.Now.ToUniversalTime();
                thisInvoice.IsPaid = true;
                thisInvoice.InvoiceStatus = StringEnum.GetStringValue(InvoiceStatus.Paid);
                _db.Entry(thisInvoice).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Invoice has been marked as paid successfully.');</script>";
            }
            catch (Exception ex) {
                TempData["toastMessage"] = "<script>toastr.error('"+ex.Message+"');</script>";
            }
            return RedirectToAction("PendingInvoices");
        }
        #endregion


    }
}