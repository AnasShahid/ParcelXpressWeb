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
using ClosedXML;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

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

        public ActionResult AllInvoices(int page = 1, string searchTxt=null, string fromDate = null, string toDate = null)
        {
            string invalidInvoice = StringEnum.GetStringValue(InvoiceStatus.Invalid);

            var model = _db.CUST_INVC.Where(i =>  i.InvoiceStatus != invalidInvoice && (searchTxt == null || i.CUST_DATA.CustomerName.Contains(searchTxt) || i.JOB.CustomerName.Contains(searchTxt) ||i.INVC_PYMT_MODE.PaymentModeDsc.Contains(searchTxt) || i.InvoiceNumber.Contains(searchTxt))).OrderByDescending(p => p.InvoiceDate).ToList();
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
                ViewData["view"] = "AllInvoices";
                ViewData["searchTxt"] = searchTxt;
                ViewData["fromDate"] = fromDate;
                ViewData["toDate"] = toDate;
                return PartialView("_InvoiceTable", finalModel);
            }
            ViewBag.TotalJobs = model.Count();
            ViewBag.TotalAmount = model.Sum(p => p.InvoiceAmount.GetValueOrDefault(0));
            ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            ViewBag.SearchText = searchTxt;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(finalModel);
        }

        public ActionResult PaidInvoices(int page = 1, string searchTxt = null, string fromDate = null, string toDate = null)
        {
            int paymentModeCash = (int)InvoicePaymentModes.Cash;
            int paymentModeBacs = (int)InvoicePaymentModes.Bacs;
            string invalidInvoice = StringEnum.GetStringValue(InvoiceStatus.Invalid);
            var model = _db.CUST_INVC.Where(i => i.IsPaid == true && i.InvoiceStatus != invalidInvoice&&(searchTxt == null || i.CUST_DATA.CustomerName.Contains(searchTxt) || i.JOB.CustomerName.Contains(searchTxt) || i.INVC_PYMT_MODE.PaymentModeDsc.Contains(searchTxt) ||  i.InvoiceNumber.Contains(searchTxt))).OrderByDescending(p => p.InvoiceDate).ToList();
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
                ViewData["view"] = "PaidInvoices";
                ViewData["searchTxt"] = searchTxt;
                ViewData["fromDate"] = fromDate;
                ViewData["toDate"] = toDate;
                return PartialView("_InvoiceTable", finalModel);
            }
            ViewBag.TotalJobs = model.Count();
            ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            ViewBag.PaidByCash = model.Where(x =>x.InvoicePaymentMode.GetValueOrDefault(0) == paymentModeCash).Sum(p => p.PaidAmount);
            ViewBag.PaidByBacs = model.Where(x => x.InvoicePaymentMode.GetValueOrDefault(0) == paymentModeBacs).Sum(p => p.PaidAmount);
            ViewBag.SearchText = searchTxt;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View(finalModel);
        }

        public ActionResult PendingInvoices(int page = 1, string searchTxt = null, string fromDate = null, string toDate = null)
        {
            string invalidInvoice = StringEnum.GetStringValue(InvoiceStatus.Invalid);
            var model = _db.CUST_INVC.Where(i => i.IsPaid == false&& i.InvoiceStatus != invalidInvoice&& (searchTxt == null || i.CUST_DATA.CustomerName.Contains(searchTxt) || i.JOB.CustomerName.Contains(searchTxt) ||  i.InvoiceNumber.Contains(searchTxt))).OrderByDescending(p => p.InvoiceDate).ToList();
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
                ViewData["view"] = "PendingInvoices";
                ViewData["searchTxt"] = searchTxt;
                ViewData["fromDate"] = fromDate;
                ViewData["toDate"] = toDate;
                return PartialView("_InvoiceTable", finalModel);
            }
            //ViewBag.TotalJobs = model.Count();
            //ViewBag.PaidAmount = model.Sum(p => p.PaidAmount.GetValueOrDefault(0));
            //ViewBag.PaidByCash = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Cash).Sum(p => p.PaidAmount);
            //ViewBag.PaidByBacs = model.Where(x => x.InvoicePaymentMode.Value == (int)InvoicePaymentModes.Bacs).Sum(p => p.PaidAmount);
            ViewBag.SearchText = searchTxt;
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

                    var previousCreditRecord = _db.CUST_CRDT.Where(cc => cc.CustomerId == thisInvoice.CustomerId && cc.SettledInd != true);
                    var previousBalance = previousCreditRecord.Sum(pcr => pcr.RemainingAmount).GetValueOrDefault(0);
                    if (previousBalance > 0) //Update previous credit to 0
                    {
                        foreach (var item in previousCreditRecord)
                        {
                            item.RemainingAmount = 0;
                            item.SettledAmount = item.CreditAmount;
                            item.SettledInd = true;
                            _db.Entry(item).State = EntityState.Modified;
                        }
                    }
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
                    var customer = _db.CUST_DATA.Find(thisInvoice.CustomerId);
                    if (customer != null && customer.HasContract.GetValueOrDefault(false) == false)
                    {
                        CUST_CRDT newCredit = new CUST_CRDT() { CustomerId = thisInvoice.CustomerId, Date = (DateTime.Now).ToUniversalTime(), CreditAmount = balance, RemainingAmount = balance, SettledAmount = 0, SettledInd = false };
                        _db.CUST_CRDT.Add(newCredit);
                    }
                }
                thisInvoice.InvoicePaymentMode = invoice.InvoicePaymentMode;
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

        [HttpPost]
        public ActionResult DeleteInvoice(int InvoiceId)
        {
            try {
                var invoice = _db.CUST_INVC.Find(InvoiceId);
                invoice.InvoiceStatus = StringEnum.GetStringValue(InvoiceStatus.Invalid);
                _db.Entry(invoice).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Invoice has been deleted successfully.');</script>";
            }
            catch (Exception ex) {
                TempData["toastMessage"] = "<script>toastr.error('Invoice could not be deleted.');</script>";
            }
            return RedirectToAction("PendingInvoices");
        }

        [HttpPost]
        public ActionResult ResendInvoice(int InvoiceId)
        {
            try
            {
                var result = PDFGenerator.resendInvoice(InvoiceId);
                if (result != true)
                    throw new Exception();
                TempData["toastMessage"] = "<script>toastr.success('Invoice has been sent to customer successfully.');</script>";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Authentication"))
                {
                    TempData["toastMessage"] = "<script>toastr.error('There was an error connecting to the mail server, Please check your email connection settings.');</script>";
                }
                TempData["toastMessage"] = "<script>toastr.error('Invoice could not be sent.');</script>";
            }
            return RedirectToAction("PendingInvoices");
        }

        [HttpPost]
        public void ExportToExcel(string FromDate = null, string ToDate = null)
        {
            var gv = new GridView();
            gv.DataSource = this.GetAllInvoiceData(FromDate,ToDate);
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AllInvoices"+DateTime.Now.ToString("ddMMyy")+".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

   
        }

        private object GetAllInvoiceData(string fromDate, string toDate)
        {
            string invalidInvoice = StringEnum.GetStringValue(InvoiceStatus.Invalid);

            var model = _db.CUST_INVC.Where(i => i.InvoiceStatus != invalidInvoice).OrderByDescending(p => p.InvoiceDate).ToList();
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
            var Data = (from m in model select new InvoiceInfo {
                CustomerName = m.CUST_DATA == null ? m.JOB.CustomerName : m.CUST_DATA.CustomerName,
                InvoiceNumber = m.InvoiceNumber,
                InvoiceDate = m.InvoiceDate.ToString("dd/MM/yyyy"),
                Amount = m.InvoiceAmount.GetValueOrDefault(0).ToString(),
                Paid = m.IsPaid == true ? "Yes" : "No",
                PaidDate = m.PaidDate.HasValue ? m.PaidDate.Value.ToString("dd/MM/yyyy") : "",
                PaymentMode=m.INVC_PYMT_MODE==null?"": m.INVC_PYMT_MODE.PaymentModeDsc
            }).ToList();

            return Data;
        }
        #endregion


    }
}