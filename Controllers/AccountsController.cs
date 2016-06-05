using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using ParcelXpress.Enums;
using ParcelXpress.Helpers;
using System.Data;
using System.Data.Objects;

namespace ParcelXpress.Controllers
{

    [Authorize]
    public class AccountsController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();
        //
        // GET: /Accounts/

        public ActionResult DriverAccounts(int driverId = 0)
        {
            decimal totalDriverCommission = 0;
            decimal totalCashPayment = 0;
            decimal totalPxpBalance = 0;
            decimal totalJobBalance = 0;
            decimal totalAmountReceivable = 0;
            string driverName = "";
            IEnumerable<JOB> jobs = null;

            if (driverId != 0)
            {
                driverName = _db.DRVR_DATA.Find(driverId).DriverName;
                var transactions = _db.DRVR_TRAN.Where(t => t.DriverId == driverId && t.SettledInd != true);
                if (transactions != null && transactions.Count() > 0)
                {
                    var transactionIn = StringEnum.GetStringValue(TransactionTypeCode.In);
                    var transactionOut = StringEnum.GetStringValue(TransactionTypeCode.Out);

                    jobs = _db.JOBS.Where(j => transactions.Any(t => j.JobId == t.JobId));
                    totalJobBalance = jobs.Sum(j => j.Price).GetValueOrDefault(0);      //total sum of jobs

                    var paymentFromAccount = jobs.Where(j => j.AccountPaymentInd == true).Sum(j => j.Price).GetValueOrDefault(0);
                    totalCashPayment = totalJobBalance - paymentFromAccount;        //total cash that driver had already received

                    totalDriverCommission = transactions.Where(t => t.TransactionType == transactionIn).Sum(t => t.Amount).GetValueOrDefault(0);
                    totalPxpBalance = transactions.Where(t => t.TransactionType == transactionOut).Sum(t => t.Amount).GetValueOrDefault(0);

                    totalAmountReceivable = totalCashPayment - totalDriverCommission;   //Driver has to give the amount by cutting his commission
                }
            }
            ViewBag.DriverName = driverName;
            ViewBag.AmountReceivable = totalAmountReceivable;
            ViewBag.DriverCommission = totalDriverCommission;
            ViewBag.CashPayment = totalCashPayment;
            ViewBag.JobBalance = totalJobBalance;
            ViewBag.DriverId = driverId;
            return View(jobs);
        }

        public ActionResult SettleDriverCommission(int driverId)
        {
            try
            {
                if (driverId != 0)
                {
                    var transactions = _db.DRVR_TRAN.Where(t => t.DriverId == driverId && t.SettledInd != true);
                    foreach (var trans in transactions)
                    {
                        trans.Balance = 0;
                        trans.SettledInd = true;
                        _db.Entry(trans).State = EntityState.Modified;
                    }
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('Driver Account has been successfully settled.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured while posting driver account.');</script>";
            }
            return RedirectToAction("DriverAccounts");

        }

        public ActionResult DriverSearch(string searchTerm = null)
        {
            var model = _db.DRVR_DATA
                .Where(c => (searchTerm == null || c.DriverName.Contains(searchTerm)) && c.IsDeleted != true)
                .OrderBy(c => c.DriverName);

            foreach (var item in model)
            {
                var transactions = _db.DRVR_TRAN.Where(t => t.DriverId == item.DriverId && t.SettledInd != true);
                if (transactions != null && transactions.Count() > 0)
                {
                    var transactionIn = StringEnum.GetStringValue(TransactionTypeCode.In);
                    var transactionOut = StringEnum.GetStringValue(TransactionTypeCode.Out);

                    var jobs = _db.JOBS.Where(j => transactions.Any(t => j.JobId == t.JobId));
                    var totalJobBalance = jobs.Sum(j => j.Price).GetValueOrDefault(0);      //total sum of jobs

                    var paymentFromAccount = jobs.Where(j => j.AccountPaymentInd == true).Sum(j => j.Price).GetValueOrDefault(0);
                    var totalCashPayment = totalJobBalance - paymentFromAccount;        //total cash that driver had already received

                    var totalDriverCommission = transactions.Where(t => t.TransactionType == transactionIn).Sum(t => t.Amount).GetValueOrDefault(0);
                    //totalPxpBalance = transactions.Where(t => t.TransactionType == transactionOut).Sum(t => t.Amount).GetValueOrDefault(0);

                    item.DriverCommissionAmount = totalCashPayment - totalDriverCommission;   //Driver has to give the amount by cutting his commission
                }
                else {
                    item.DriverCommissionAmount = 0;
                }
            }
            return PartialView("_SearchResultDriver", model);
        }

        public ActionResult CustomerSearch(string searchTerm = null)
        {
            var model = _db.CUST_DATA
                .Where(c => (searchTerm == null || c.CustomerName.Contains(searchTerm) || c.Address.Contains(searchTerm) || c.ContactNo.Contains(searchTerm)) && c.HasAccount == true && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName);

            //var abc = from cust in _db.CUST_DATA
            //            join trans in _db.CUST_TRAN on cust.CustomerId equals trans.CustomerId
            //          where (searchTerm == null || cust.CustomerName.Contains(searchTerm) || cust.Address.Contains(searchTerm) || cust.ContactNo.Contains(searchTerm)) && cust.HasAccount == true && trans.SettledInd != true                        
            //          group cust by new { cust.CustomerId, cust.CustomerName, cust.AccountRefNumber, cust.ContactNo, cust.Address } into custGroup
            //          let totalAmount=custGroup.Sum(x=>x.)
            //            orderby custGroup.Key.CustomerName
            //          select new { CustomerId = custGroup.Key.CustomerId, CustomerName = custGroup.Key.CustomerName, AccountRefNumber = custGroup.Key.AccountRefNumber, ContactNo = custGroup.Key.ContactNo, Address = custGroup.Key.Address, TotalPayable = trans};

            //List<dynamic> tempList = new List<dynamic>();
            foreach (var item in model)
            {
                item.CustomerPayable = ((_db.CUST_TRAN.Where(trans => trans.CustomerId == item.CustomerId && trans.SettledInd != true)).Sum(c => c.RemainingAmount)).GetValueOrDefault(0);
            }

            return PartialView("_SearchResultCustomer", model);
        }

        public ActionResult CustomerAccounts(int customerId = 0)
        {
            decimal totalCustomerPayable = 0;
            decimal totalPaidByCustomer = 0;
            string customerName = "";
            List<CUST_BILL> BillHistory = null;
            if (customerId != 0)
            {
                customerName = _db.CUST_DATA.Find(customerId).CustomerName;
                var customerAccountList = (from trans in _db.CUST_TRAN
                                           where trans.CustomerId == customerId && trans.SettledInd != true
                                           join jobs in _db.JOBS on trans.JobId equals jobs.JobId
                                           join driver in _db.DRVR_DATA on jobs.DriverId equals driver.DriverId
                                           select new { JobId = jobs.JobId, JobDate = jobs.JobDate, DriverName = driver.DriverName, Price = trans.PayableAmount, ReceivedAmount = trans.ReceivedAmount, RemainingAmount = trans.RemainingAmount }).ToList();

                List<CustomerJobDriver> CustomerDriverJobList = new List<CustomerJobDriver>();
                foreach (var item in customerAccountList)
                {
                    CustomerDriverJobList.Add(new CustomerJobDriver()
                    {
                        JobId = item.JobId,
                        JobDate = item.JobDate,
                        DriverName = item.DriverName,
                        Price = item.Price.GetValueOrDefault(0),
                        ReceivedAmount = item.ReceivedAmount.GetValueOrDefault(0),
                        RemainingAmount = item.RemainingAmount.GetValueOrDefault(0)
                    });
                }
                totalPaidByCustomer = _db.CUST_CRDT.Where(cc => cc.CustomerId == customerId && cc.SettledInd != true).Sum(cc => cc.RemainingAmount).GetValueOrDefault(0);
                totalCustomerPayable = CustomerDriverJobList.Sum(p => p.RemainingAmount);
                ViewBag.CustomerAccounts = CustomerDriverJobList;

                BillHistory = _db.CUST_BILL.Where(b => b.CustomerId == customerId).OrderByDescending(b => b.PaymentDate).Take(10).ToList();
            }
            ViewBag.PreviousPaidAmount = totalPaidByCustomer;
            ViewBag.CustomerPayable = totalCustomerPayable;
            ViewBag.CustomerName = customerName;
            ViewBag.CustomerId = customerId;
            ViewBag.BillHistory = BillHistory;
            CUST_BILL model = new CUST_BILL() { CustomerId = customerId, DueAmount = totalCustomerPayable - totalPaidByCustomer };
            return View(model);
        }

        [HttpPost]
        public ActionResult SettleCustomerAccount(CUST_BILL bill)
        {
            var previousCreditRecord = _db.CUST_CRDT.Where(cc => cc.CustomerId == bill.CustomerId && cc.SettledInd != true);
            var previousBalance = previousCreditRecord.Sum(pcr => pcr.RemainingAmount).GetValueOrDefault(0);
            if (bill.PaidAmount == 0 && previousBalance == 0)
            {
                TempData["toastMessage"] = "<script>toastr.warning('Customer does not have paid amount or previous balance to settle the bill.');</script>";
                return RedirectToAction("CustomerAccounts", new { customerId = bill.CustomerId });
            }
            bill.RemainingAmount = bill.DueAmount - (bill.PaidAmount);
            bill.PreviousCredit = previousBalance;
            bill.PaymentDate = (DateTime.Now).ToUniversalTime();
            try
            {
                var balance = bill.PaidAmount + previousBalance;
                var settlingtransactions = _db.CUST_TRAN.Where(t => t.CustomerId == bill.CustomerId && t.SettledInd != true);

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
                if (balance > 0) // Save remaining amount to customer credit.
                {
                    CUST_CRDT newCredit = new CUST_CRDT() { CustomerId = bill.CustomerId, Date = (DateTime.Now).ToUniversalTime(), CreditAmount = balance, RemainingAmount = balance, SettledAmount = 0, SettledInd = false };
                    _db.CUST_CRDT.Add(newCredit);
                }
                _db.CUST_BILL.Add(bill);
                _db.SaveChanges();
                ResetAlertHistory(bill.CustomerId);
                TempData["toastMessage"] = "<script>toastr.success('Customer Account has been successfully posted.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('There was an error posting the account.');</script>";
            }
            return RedirectToAction("CustomerAccounts", new { customerId = bill.CustomerId });
        }

        [HttpPost]
        public ActionResult CreateCustomerStatement()
        {
            int customerId = 0;
            decimal previousPaidAmount = 0;
            try
            {
                customerId = Convert.ToInt16(TempData["CustomerId"].ToString());
                var customerJobs = TempData["CustomerJobs"] as List<ParcelXpress.Helpers.CustomerJobDriver>;
                CUST_DATA customerInformation = _db.CUST_DATA.Find(customerId);
                //Check if customer has an email Address
                if (customerInformation == null || customerInformation.EmailAddress == null || customerInformation.EmailAddress.Trim().Equals(""))
                {
                    throw new Exception("Customer does not have an email address configured with the account.");
                }
                //Read Configurations for configured Email Address
                var lastEmailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
                previousPaidAmount = _db.CUST_CRDT.Where(cc => cc.CustomerId == customerId && cc.SettledInd != true).Sum(cc => cc.RemainingAmount).GetValueOrDefault(0);


                if (lastEmailAccount == null || lastEmailAccount.EmailAddress == null || lastEmailAccount.Password == null || lastEmailAccount.EmailClient == null)
                {
                    throw new Exception("Please configure a valid email address from settings to send an email.");
                }

                var reportDetails = (from cj in customerJobs
                                     join job in _db.JOBS on cj.JobId equals job.JobId
                                     select new { JobId = job.JobId, JobDate = job.JobDate, PaidAmount = cj.ReceivedAmount, PickupAddress = job.PickupAddress, DropAddress = job.DropAddress, RemainingAmount = cj.RemainingAmount, ChargeDescription = job.ChargesDescription }).ToList();
                List<CustomerJobDriver> reportParameters = new List<CustomerJobDriver>();
                foreach (var item in reportDetails)
                {
                    reportParameters.Add(new CustomerJobDriver()
                    {
                        JobId = item.JobId,
                        JobDate = item.JobDate,
                        PickupAddress = item.PickupAddress,
                        DropAddress = item.DropAddress,
                        RemainingAmount = item.RemainingAmount,
                        ChargeDescription = item.ChargeDescription
                    });
                }

                bool result = PDFGenerator.createCustomerReportMarkup(customerInformation, reportParameters, previousPaidAmount);
                if (result == true)
                    TempData["toastMessage"] = "<script>toastr.success('Invoice has been successfully sent to customer.');</script>";
                else
                    TempData["toastMessage"] = "<script>toastr.error('There was an error connecting to the mail server, Please check your email connection settings.');</script>";

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Authentication"))
                {
                    TempData["toastMessage"] = "<script>toastr.error('There was an error connecting to the mail server, Please check your email connection settings.');</script>";
                }
                else
                    TempData["toastMessage"] = "<script>toastr.error('" + ex.Message + "');</script>";
            }
            return RedirectToAction("CustomerAccounts", new { customerId = customerId });
        }

        [HttpGet]
        public ActionResult AccountOverview(string fromDate = null, string toDate = null)
        {
            List<JOB> jobs = new List<JOB>();
            bool model = false;
            try
            {
                if (fromDate == null || fromDate == "" || toDate == null || toDate == "")
                    throw new Exception("Please fill both dates to see the account overview.");

                DateTime startDate = Convert.ToDateTime(fromDate);
                DateTime endDate = Convert.ToDateTime(toDate);
                if (endDate < startDate)
                    throw new Exception("[To Date] cannot be earlier than [From date].");

                jobs = _db.JOBS.Where(j => (EntityFunctions.TruncateTime(j.JobDate) >= EntityFunctions.TruncateTime(startDate)) && (EntityFunctions.TruncateTime(j.JobDate) <= EntityFunctions.TruncateTime(endDate))).ToList();
                if (jobs.Count > 0)
                    model = true;
                if (model)
                {
                    decimal totalJobsPrice= jobs.Sum(j => j.Price).GetValueOrDefault(0);
                    decimal totalDriverCommission= ((from j in jobs
                                                     join drvrTran in _db.DRVR_TRAN on j.JobId equals drvrTran.JobId
                                                     where drvrTran.TransactionType==StringEnum.GetStringValue(TransactionTypeCode.In)
                                                     select drvrTran).Sum(x => x.Amount)).GetValueOrDefault(0);
                    decimal netTotalEarned=totalJobsPrice-totalDriverCommission;
                    decimal totalEarnedOnZeroJobs = ((from j in jobs
                                                      join drvrTran in _db.DRVR_TRAN on j.JobId equals drvrTran.JobId
                                                      where (drvrTran.TransactionType == StringEnum.GetStringValue(TransactionTypeCode.In) & drvrTran.Amount == 0)
                                                      select j).Sum(x => x.Price)).GetValueOrDefault(0);
                    
                    List<Dictionary<string, dynamic>> paymentTypesSummary = new List<Dictionary<string, dynamic>>();
                    var paymentModes = Enum.GetValues(typeof(PaymentModes));

                    foreach (var item in paymentModes)
                    {
                        Dictionary<string, dynamic> paymentTypeSummary = new Dictionary<string, dynamic>();
                        paymentTypeSummary.Add("Type", item.ToString());
                        paymentTypeSummary.Add("Count", jobs.Count(j => j.PaymentMode ==(int) item));
                        paymentTypeSummary.Add("Amount", jobs.Where(j => j.PaymentMode == (int)item).Sum(x => x.Price));
                        paymentTypesSummary.Add(paymentTypeSummary);
                    }

                    ViewBag.paymentTypeSummary = paymentTypesSummary;
                    ViewBag.TotalJobs = jobs.Count;
                    ViewBag.SubIncomeRaw = totalJobsPrice;
                    ViewBag.TotalDriverCommission =totalDriverCommission;
                    ViewBag.TotalEarnedOnZeroJobs = totalEarnedOnZeroJobs;
                    ViewBag.NetTotalEarned = netTotalEarned;
                    ViewBag.TotalEarnedOnCommission = netTotalEarned - totalEarnedOnZeroJobs;
                   
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.info('" + ex.Message + "');</script>";
            }
            return View(model);
        }

        private void ResetAlertHistory(int customerId)
        {
            if (customerId <= 0)
                return;
            var dueAlerts = _db.DUES_ALRT.Where(due => due.CustomerId == customerId && due.IsPaid != true);
            foreach (var item in dueAlerts)
            {
                item.IsPaid = true;
                _db.Entry(item).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }

        [HttpGet]
        public ActionResult ShowAccountHistory(int CustomerId, string fromDate = null, string toDate = null)
        {
            try
            {
                if (fromDate == null || fromDate == "" || toDate == null || toDate == "")
                    throw new Exception("Please fill both dates to see the account overview.");

                DateTime startDate = Convert.ToDateTime(fromDate);
                DateTime endDate = Convert.ToDateTime(toDate);
                if (endDate < startDate)
                    throw new Exception("[To Date] cannot be earlier than [From date].");

                string customerName = "";
                if (CustomerId != 0)
                {
                    customerName = _db.CUST_DATA.Find(CustomerId).CustomerName;
                    List<JOB> jobs = _db.JOBS.Where(j =>j.CustomerId==CustomerId && (EntityFunctions.TruncateTime(j.JobDate) >= EntityFunctions.TruncateTime(startDate)) && (EntityFunctions.TruncateTime(j.JobDate) <= EntityFunctions.TruncateTime(endDate))).ToList();

                    var enumValues=Enum.GetValues(typeof(PaymentModes)).Cast<PaymentModes>().ToList();
                    jobs.ForEach(j=>j.paymentModeDescription= StringEnum.GetDisplayName(enumValues.FirstOrDefault(e=>j.PaymentMode==(int)e)));
                    
                    ViewBag.CustomerName = customerName;
                    ViewBag.ToDate = endDate.ToString("dd-MMM-yyyy"); ;
                    ViewBag.FromDate = startDate.ToString("dd-MMM-yyyy");
                    return View(jobs);
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('" + ex.Message + "');</script>";
            }
            return RedirectToAction("CustomerAccounts", new { customerId = CustomerId }); ;
        }
    }
}
