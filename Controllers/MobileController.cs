﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using ParcelXpress.Enums;
using System.Data;
using ParcelXpress.Helpers;
using System.Text;

namespace ParcelXpress.Controllers
{
    public class MobileController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();
        //
        // GET: /Mobile/
        [HttpPost]
        public JsonResult Register(REQT_DRVR driverRequest)
        {
            String result;
            try
            {
                if (ModelState.IsValid)
                {
                    int countExisting = 0;
                    countExisting = _db.DRVR_DATA.Where(d => d.LoginName == driverRequest.LoginName).Count() + _db.REQT_DRVR.Where(d => d.LoginName == driverRequest.LoginName).Count();
                    if (countExisting > 0)
                    {
                        result = "user exist";
                    }
                    else
                    {
                        var req = new REQT_DRVR()
                        {
                            DriverName = driverRequest.DriverName,
                            LoginName = driverRequest.LoginName,
                            Password = driverRequest.Password,
                            ContactNo = driverRequest.ContactNo,
                            IsConverted = false,
                            GcmId = driverRequest.GcmId
                        };

                        result = "DriverName: " + req.DriverName + "\nLoginName:" + req.LoginName + "\nPassword:" + req.Password + "\nContactNo=" + req.ContactNo + "\nGCMId:" + req.GcmId;
                        _db.REQT_DRVR.Add((req));
                        _db.Entry(req).State = EntityState.Added;
                        _db.SaveChanges();
                        result = "true";
                    }
                }
                else
                {
                    result = "Data was invalid, request not submitted.";
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DriverLogin(string username, string password)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            var driver = _db.DRVR_DATA.Where(d => d.LoginName == username && d.Password == password && d.IsDeleted == false).FirstOrDefault();
            if (driver != null)
            {
                driver.IsActive = true;
                _db.Entry(driver).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                driver = new DRVR_DATA() { DriverId = 0, DriverName = "Invalid" };
            }
            return Json(driver, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DriverLoginPost(DRVR_DATA driverLogin)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            var driver = _db.DRVR_DATA.Where(d => d.LoginName == driverLogin.LoginName && d.Password == driverLogin.Password && d.IsDeleted == false).FirstOrDefault();
            if (driver != null)
            {
                driver.IsActive = true;
                _db.Entry(driver).State = EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                driver = new DRVR_DATA() { DriverId = 0, DriverName = "Invalid" };
            }
            return Json(driver, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 4, Location = System.Web.UI.OutputCacheLocation.Server)]
        [HttpGet]
        public JsonResult AvailableJobs()
        {
            string open = StringEnum.GetStringValue(StatusCode.Open);
            try
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var availablejobs = _db.JOBS.Where(j => j.JobStatus == open)
                                    .OrderByDescending(j => j.JobDate);
                return Json(availablejobs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<JOB> availablejobs = null;
                return Json(availablejobs, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult MyJobs(int driverId)
        {
            string assigned = StringEnum.GetStringValue(StatusCode.Assigned);
            string pickedUp = StringEnum.GetStringValue(StatusCode.PickedUp);
            string droppedOff = StringEnum.GetStringValue(StatusCode.DroppedOff);

            try
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var myJobs = _db.JOBS.Where(j => j.DriverId == driverId && (j.JobStatus == assigned || j.JobStatus == pickedUp || j.JobStatus == droppedOff))
                                        .OrderByDescending(j => j.JobDate);
                return Json(myJobs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<JOB> myJobs = null;
                return Json(myJobs, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetBalance(int driverId)
        {
            string inTransaction = StringEnum.GetStringValue(TransactionTypeCode.In);
            string outTransaction = StringEnum.GetStringValue(TransactionTypeCode.Out);
            _db.Configuration.ProxyCreationEnabled = false;
            var transactions = _db.DRVR_TRAN.Where(t => t.DriverId == driverId && t.SettledInd != true);
            Balance bal = new Balance();
            foreach (var tran in transactions)
            {
                if (tran.TransactionType == inTransaction)
                    bal.DriverBalance += tran.Balance.GetValueOrDefault(0);
                else if (tran.TransactionType == outTransaction)
                    bal.PxpBalance += tran.Balance.GetValueOrDefault(0);
            }

            return Json(bal, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult MessageServer(DRVR_MSGS driverMessage)
        {
            string result = "";
            try
            {
                driverMessage.MessageDate = DateTime.Now.ToUniversalTime();
                driverMessage.MessageReadInd = false;
                driverMessage.MessageReceivedInd = true;
                if (ModelState.IsValid)
                {
                    _db.DRVR_MSGS.Add(driverMessage);
                    _db.SaveChanges();
                    result = "Your message has been received.";
                }
                else
                {
                    result = "There were some invalid inputs";
                }
            }
            catch (Exception ex)
            {
                result = "An exception occurred on the server";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getAllActiveJobs()
        {
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            try
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var jobs = _db.JOBS.Where(j => j.JobStatus != closed)
                .OrderByDescending(j => j.JobDate);

                return Json(jobs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult VerifyAccount(int customerId)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            CUST_DATA customer = _db.CUST_DATA.Find(customerId);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeJobStatus(JOB job)
        {
            try
            {
                var thisjob = _db.JOBS.Find(job.JobId);
                DateTime transDate = DateTime.Now.ToUniversalTime();
                string dropped = StringEnum.GetStringValue(StatusCode.DroppedOff);
                if (job.JobStatus == dropped)           //Accounting when dropped off
                {
                    var driver = _db.DRVR_DATA.Find(job.DriverId);
                    decimal commission = (job.Price * (driver.CommissionRate / 100)).GetValueOrDefault(0);
                    var driverTransactionCommission = new DRVR_TRAN()
                    {
                        DriverId = driver.DriverId,
                        JobId = job.JobId,
                        TransactionDate = transDate,
                        TransactionType = StringEnum.GetStringValue(TransactionTypeCode.In),
                        Amount = commission,
                        Balance = commission,
                        SettledInd = false
                    };
                    _db.DRVR_TRAN.Add(driverTransactionCommission);
                    _db.SaveChanges();

                    if (!job.AccountPaymentInd.GetValueOrDefault(false))    //Customer paid the cash to driver
                    {
                        var diverTransactionPXP = new DRVR_TRAN()
                        {
                            DriverId = driver.DriverId,
                            JobId = job.JobId,
                            TransactionDate = transDate,
                            TransactionType = StringEnum.GetStringValue(TransactionTypeCode.Out),
                            Amount = (job.Price - commission),
                            Balance = (job.Price - commission),
                            SettledInd = false
                        };
                        _db.DRVR_TRAN.Add(diverTransactionPXP);
                        _db.SaveChanges();
                    }
                    else
                    {
                        var diverTransactionPXP = new DRVR_TRAN()
                        {
                            DriverId = driver.DriverId,
                            JobId = job.JobId,
                            TransactionDate = transDate,
                            TransactionType = StringEnum.GetStringValue(TransactionTypeCode.Out),
                            Amount = (0 - commission),
                            Balance = (0 - commission),
                            SettledInd = false
                        };
                        _db.DRVR_TRAN.Add(diverTransactionPXP);     //Driver does not have to pay PXP instead commission is dropped from pxp account
                        _db.SaveChanges();

                        var customerTransAccount = new CUST_TRAN()
                        {
                            CustomerId = job.CustomerId,
                            JobId = job.JobId,
                            TransactionDate = transDate,
                            PayableAmount = job.Price, //whole amount is receivable from customer
                            ReceivedAmount = 0,
                            SettledInd = false
                        };
                        _db.CUST_TRAN.Add(customerTransAccount);
                        _db.SaveChanges();
                    }
                    job.JobStatus = StringEnum.GetStringValue(StatusCode.Closed);
                }
                thisjob.AccountPaymentInd = job.AccountPaymentInd;
                thisjob.JobStatus = job.JobStatus;
                thisjob.JobDate = transDate;
                _db.Entry(thisjob).State = EntityState.Modified;
                _db.SaveChanges();

                return Json("Job Status has been changed.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("An Exception occured on the server " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChangeDriverStatus(DRVR_DATA driver)
        {
            string result = "";
            try
            {
                var dbDriver = _db.DRVR_DATA.Find(driver.DriverId);
                dbDriver.IsActive = driver.IsActive;
                _db.Entry(dbDriver).State = EntityState.Modified;
                _db.SaveChanges();
                if (dbDriver.IsActive == true)
                    result = "You are now active in the system, you will receive job updates";
                else
                    result = "You are now marked as busy, you wont be able to receive new job updates";

            }
            catch (Exception ex)
            {
                result = "An exception occured on the system " + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddCharges(JOB currentJob)
        {
            try
            {
                var job = _db.JOBS.Find(currentJob.JobId);
                job.Price = currentJob.Price;
                if ((job.Notes == null ? "" : job.Notes).Trim() == "")
                {
                    job.Notes = "Driver Comments: " + currentJob.Notes;
                }
                else
                {
                    job.Notes = (job.Notes == null ? "" : job.Notes) + "\nDriver Comments: " + currentJob.Notes;
                }
                if ((job.ChargesDescription == null ? "" : job.ChargesDescription).Trim() == "")
                {
                    job.ChargesDescription = currentJob.Notes;
                }
                else {
                    job.ChargesDescription = (job.ChargesDescription == null ? "" : job.ChargesDescription) + "\n" + currentJob.Notes;
                }
                _db.Entry(job).State = EntityState.Modified;
                _db.SaveChanges();
                return Json("Charges have been successfully added", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json("An exception occurred on the server " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TakeJob(JOBS_RESP jobResponse)
        {
            string result = "";
            try
            {
                jobResponse.ResponseTime = DateTime.Now.ToUniversalTime();
                var job = _db.JOBS.Find(jobResponse.JobId);
                if (job.DriverId == null || job.DriverId == 0)
                {
                    string responseTime = "";
                    responseTime = getResponseTimeFromCode(jobResponse.ResponseCode);
                    job.DriverId = jobResponse.DriverId;
                    job.JobStatus = StringEnum.GetStringValue(StatusCode.Assigned);
                    jobResponse.IsAssigned = true;
                    _db.Entry(job).State = EntityState.Modified;
                    _db.SaveChanges();
                    var driver = _db.DRVR_DATA.Find(job.DriverId);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Job has been assinged to you.");
                    sb.AppendLine(" ");
                    sb.Append("Pickup from: " + job.PickupAddress + "||" + responseTime + "||" + job.CustomerPhone);
                    GcmSender.SendToSingle(driver, sb.ToString(), "job_assigned");
                    result = "assigned";
                }
                else
                {
                    jobResponse.IsAssigned = false;
                    result = "not_assigned";
                }
                _db.JOBS_RESP.Add(jobResponse);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = "error";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string getResponseTimeFromCode(string responseCode)
        {
            if (responseCode == StringEnum.GetStringValue(ResponseCode.LessThanFive))
                return StringEnum.GetDisplayName(ResponseCode.LessThanFive);
            else if (responseCode == StringEnum.GetStringValue(ResponseCode.FiveToTen))
                return StringEnum.GetDisplayName(ResponseCode.FiveToTen);
            else if (responseCode == StringEnum.GetStringValue(ResponseCode.TenToFifteen))
                return StringEnum.GetDisplayName(ResponseCode.TenToFifteen);
            else if (responseCode == StringEnum.GetStringValue(ResponseCode.FifteenToTwenty))
                return StringEnum.GetDisplayName(ResponseCode.FifteenToTwenty);
            else if (responseCode == StringEnum.GetStringValue(ResponseCode.MoreThanTwenty))
                return StringEnum.GetDisplayName(ResponseCode.MoreThanTwenty);
            else
                return "";
        }

        [HttpGet]
        public JsonResult LoadDriverInbox(int driverId)
        {
            var driverCreatedOn = _db.DRVR_DATA.Find(driverId).CreatedOn;
            _db.Configuration.ProxyCreationEnabled = false;
            var messages = _db.DRVR_MSGS
                .Where(d => d.MessageReceivedInd == false && (d.DriverId == driverId || (d.DriverId == null && d.MessageDate >= driverCreatedOn)))
                .OrderByDescending(m => m.MessageDate)
                .Select(m => new { m.MessageId, m.MessageDate, m.Message, m.MessageReadInd, m.MessageReceivedInd, m.DriverId })
                .Take(20);
            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SetAccountPayment(int jobId, bool accountPayment)
        {
            try
            {
                var job = _db.JOBS.Find(jobId);
                job.AccountPaymentInd = accountPayment;
                _db.Entry(job).State = EntityState.Modified;
                _db.SaveChanges();
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("true", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult MyJobHistory(int driverId)
        {
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            DateTime today = (DateTime.Now).ToUniversalTime();
            _db.Configuration.ProxyCreationEnabled = false;
            var driverCommission = _db.DRVR_DATA.Find(driverId).CommissionRate;

            var Jobs = _db.JOBS.Where(j => j.JobDate.Year == today.Year && j.JobDate.Month == today.Month && j.JobDate.Day == today.Day && j.JobStatus == closed);
            //.Select(j=>new{j.JobId,j.JobDate,j.JobStatus,j.DriverId,j.DriverCommission,j.CustomerPhone,j.CustomerName,j.CustomerId,j.AccountPaymentInd,j.DropAddress,j.PickupAddress,j.Price,j.Notes,j});
            List<JOB> myJobs = new List<JOB>();
            foreach (var job in Jobs)
            {
                if (job.DriverId == driverId)
                {
                    job.DriverCommission = job.Price * (driverCommission / 100);
                    job.DRVR_DATA = null;
                    job.CUST_DATA = null;
                    myJobs.Add(job);
                }
            }
            return Json(myJobs, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCustomerNames()
        {
            List<string> customersList = new List<string>();
            foreach (var item in _db.CUST_DATA)
            {
                customersList.Add(item.CustomerId + "--" + item.CustomerName);
            }
            return Json(customersList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCustomerDetails(List<int> searchedCustomersId)
        {
            try
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var customers = _db.CUST_DATA.Where(c => searchedCustomersId.Contains(c.CustomerId));
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SendJob(JOB receivedJob)
        {
            string result = "";
            JOB job = new JOB()
            {
                AccountPaymentInd = receivedJob.AccountPaymentInd,
                CustomerId = receivedJob.CustomerId,
                CustomerName = receivedJob.CustomerName,
                CustomerPhone = receivedJob.CustomerPhone,
                DropAddress = receivedJob.DropAddress,
                Notes = receivedJob.Notes,
                PickupAddress = receivedJob.PickupAddress,
                Price = receivedJob.Price
            };
            try
            {
                job.DriverId = job.DriverId == 0 ? null : job.DriverId;
                job.CustomerId = job.CustomerId == 0 ? null : job.CustomerId;

                job.JobId = 0;
                if (ModelState.IsValid)
                {
                    job.JobStatus = StringEnum.GetStringValue(StatusCode.Open);
                    job.JobDate = (DateTime.Now).ToUniversalTime();
                    _db.JOBS.Add(job);
                    _db.SaveChanges();
                    var activeDrivers = _db.DRVR_DATA.Where(d => d.IsActive == true && d.IsDeleted != true);
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("New Job Available.");
                    sb.AppendLine(" ");
                    sb.AppendLine("Pickup Address: " + job.PickupAddress);
                    sb.AppendLine(" ");
                    sb.AppendLine("Drop Address: " + job.DropAddress);
                    sb.AppendLine(" ");
                    if (job.Notes != null && job.Notes.Trim() != "")
                        sb.AppendLine("Notes: " + job.Notes);

                    foreach (var driver in activeDrivers)
                    {
                        GcmSender.SendToSingle(driver, sb.ToString(), "new_job");
                    }
                    result = "Job has been posted successfully";
                }
                else
                {
                    result = "Job could not be posted due to invalid inputs";
                }
            }
            catch (Exception ex)
            {
              
                result = "An exception occured on server.";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SendJobTest(string customerName,string contactNumber,int customerId,string pickupAddress,string dropAddress,decimal price,bool accountPayment,string notes)
        {
            string result = "";
            JOB job = new JOB()
            {
                AccountPaymentInd = accountPayment,
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerPhone = contactNumber,
                DropAddress = dropAddress,
                Notes = notes,
                PickupAddress = pickupAddress,
                Price = price
            };
            try
            {
                job.DriverId = job.DriverId == 0 ? null : job.DriverId;
                job.JobId = 0;
                if (ModelState.IsValid)
                {
                    job.JobStatus = StringEnum.GetStringValue(StatusCode.Open);
                    job.JobDate = (DateTime.Now).ToUniversalTime();
                    _db.JOBS.Add(job);
                    _db.SaveChanges();
                    var activeDrivers = _db.DRVR_DATA.Where(d => d.IsActive == true && d.IsDeleted != true);
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("New Job Available.");
                    sb.AppendLine(" ");
                    sb.AppendLine("Pickup Address: " + job.PickupAddress);
                    sb.AppendLine(" ");
                    sb.AppendLine("Drop Address: " + job.DropAddress);
                    sb.AppendLine(" ");
                    if (job.Notes != null && job.Notes.Trim() != "")
                        sb.AppendLine("Notes: " + job.Notes);

                    foreach (var driver in activeDrivers)
                    {
                        GcmSender.SendToSingle(driver, sb.ToString(), "new_job");
                    }
                    result = "Job has been posted successfully";
                }
                else
                {
                    result = "Job could not be posted due to invalid inputs";
                }
            }
            catch (Exception ex)
            {
                result = "An exception occured on server: \n" + ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoginAdmin(string username, string password)
        {
            bool result;
            try {
                var validUser = _db.SYS_USER.Where(a => a.UserName == username  && a.Password == password).FirstOrDefault();
                if (validUser != null)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReRegisterDevice(DRVR_DATA driver)
        {
            var existingDriver = _db.DRVR_DATA.Where(d => d.LoginName == driver.LoginName && d.Password == driver.Password).FirstOrDefault();
            try
            {
                if (existingDriver == null)
                {
                    throw new Exception("Username and/or Password is incorrect.");
                }
                existingDriver.GcmId = driver.GcmId;
                _db.Entry(existingDriver).State = EntityState.Modified;
                _db.SaveChanges();
                GcmSender.SendToSingle(driver, "Your device has been successfully registered.", "register_success");

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("Request has been successfully submitted to server.", JsonRequestBehavior.AllowGet);

        }
    }
}