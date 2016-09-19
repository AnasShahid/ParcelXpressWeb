using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Enums;
using ParcelXpress.Models;
using PagedList;
using ParcelXpress.Helpers;
using System.Data;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Runtime.Remoting;

namespace ParcelXpress.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        //
        // GET: /Jobs/

        #region Controller Variables
        private List<String> TypesOfParcel = new List<string>() { "Small item", "Big item", "Delicate item", "Document", "Wrapped Food", "Dry Food", "Small box", "Big box", "Clothing", "Wig", "Electronics" };
        ParcelXpressConnection _db = new ParcelXpressConnection();

        #endregion

        #region Get and View loading methods
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CustomerSearch(string searchTerm = null, string searchCategory = null)
        {
            var model = _db.CUST_DATA
                .Where(c => (searchTerm == null || c.CustomerName.Contains(searchTerm) || c.Address.Contains(searchTerm) || c.ContactNo.Contains(searchTerm)) && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName);

            if (searchCategory != null && !searchCategory.Trim().Equals(""))
            {
                if (searchCategory == ((int)PaymentModes.Account).ToString())
                    return PartialView("_SearchResultCustomer", model.Where(m => m.HasAccount == true && m.HasContract != true));
                else if (searchCategory == ((int)PaymentModes.Contract).ToString())
                    return PartialView("_SearchResultCustomer", model.Where(m => m.HasContract == true));

            }
            return PartialView("_SearchResultCustomer", model);
        }

        public ActionResult CreateNewJob(string customerId = null)
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
            var typeOfParcelList = new List<SelectListItem>();
            foreach (var item in TypesOfParcel)
            {
                typeOfParcelList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }
            ViewBag.TypesOfParcel = typeOfParcelList;

            //var customerAccountInd = false;
            //var customerContractInd = false;
            if (customerId != null)
            {
                try
                {

                    int id = int.Parse(customerId);
                    var customerDetails = _db.CUST_DATA.Find(id);
                    if (customerDetails != null)
                    {
                        JOB model = new JOB() { CustomerName = customerDetails.CustomerName, CustomerId = customerDetails.CustomerId, CustomerPhone = customerDetails.ContactNo, PickupAddress = customerDetails.Address };

                        if (customerDetails.HasAccount != null)
                            ViewBag.CustomerAccountInd = (bool)customerDetails.HasAccount;
                        if (customerDetails.HasContract != null)
                            ViewBag.CustomerContractInd = (bool)customerDetails.HasContract;

                        return View(model);
                    }
                }
                catch (Exception ex)
                { }
            }
            ViewBag.CustomerAccountInd = null;
            ViewBag.CustomerContractInd = null;
            return View();
        }

        public ActionResult ActiveJobs(int page = 1)
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
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            var model = _db.JOBS
                .Where(j => j.JobStatus != closed)
                .OrderByDescending(j => j.LastUpdated)
                .ToPagedList(page, 10);
            return View(model);
        }

        public ActionResult _ActiveJobsDashboard(int page = 1)
        {
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            string open = StringEnum.GetStringValue(StatusCode.Open);
            ViewBag.OpenJobs = _db.JOBS.Where(j => (j.JobStatus == open) && (j.LongDistanceInd != true || j.LongDistanceInd == null)).Count();
            ViewBag.AssignedJobs = _db.JOBS.Where(j => (j.JobStatus != open && j.JobStatus != closed) && (j.LongDistanceInd != true || j.LongDistanceInd == null)).Count();
            var model = _db.JOBS
                .Where(j => (j.JobStatus != closed) && (j.LongDistanceInd != true || j.LongDistanceInd == null))
                .OrderByDescending(j => j.LastUpdated)
                .ToPagedList(page, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ActiveJobsList", model);
            }
            return PartialView(model);
        }

        public ActionResult _LongDistanceJobsDashboard(int page = 1)
        {
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            string open = StringEnum.GetStringValue(StatusCode.Open);
            ViewBag.JobCount = _db.JOBS.Where(j => (j.JobStatus != closed) && (j.LongDistanceInd == true)).Count();
            //   ViewBag.AssignedJobs = _db.JOBS.Where(j => (j.JobStatus != open && j.JobStatus != closed) && (j.LongDistanceInd != true)).Count();
            var model = _db.JOBS
                .Where(j => (j.JobStatus != closed) && (j.LongDistanceInd == true))
                .OrderByDescending(j => j.LastUpdated)
                .ToPagedList(page, 10);
            return PartialView(model);
        }
        public ActionResult _NewJobDashboard(string customerId = null)
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

            var typeOfParcelList = new List<SelectListItem>();
            foreach (var item in TypesOfParcel)
            {
                typeOfParcelList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }
            ViewBag.TypesOfParcel = typeOfParcelList;
            if (customerId != null)
            {
                try
                {

                    int id = int.Parse(customerId);
                    var customerDetails = _db.CUST_DATA.Find(id);
                    if (customerDetails != null)
                    {
                        JOB model = new JOB() { CustomerName = customerDetails.CustomerName, CustomerId = customerDetails.CustomerId, CustomerPhone = customerDetails.ContactNo, PickupAddress = customerDetails.Address };


                        if (customerDetails.HasAccount != null)
                            ViewBag.CustomerAccountInd = (bool)customerDetails.HasAccount;
                        if (customerDetails.HasContract != null)
                            ViewBag.CustomerContractInd = (bool)customerDetails.HasContract;

                        return PartialView(model);
                    }
                }
                catch (Exception ex)
                { }
            }
            ViewBag.CustomerAccountInd = null;
            ViewBag.CustomerContractInd = null;
            return PartialView();
        }

        public ActionResult CustomerSearchDashboard(string searchTerm = null, string searchCategory = null)
        {
            var model = _db.CUST_DATA
                .Where(c => (searchTerm == null || c.CustomerName.Contains(searchTerm) || c.Address.Contains(searchTerm) || c.ContactNo.Contains(searchTerm)) && c.IsDeleted != true)
                .OrderBy(c => c.CustomerName);
            if (searchCategory != null && !searchCategory.Trim().Equals(""))
            {
                if (searchCategory == ((int)PaymentModes.Account).ToString())
                    return PartialView("_SearchResultCustomerDashboard", model.Where(m => m.HasAccount == true && m.HasContract != true));
                else if (searchCategory == ((int)PaymentModes.Contract).ToString())
                    return PartialView("_SearchResultCustomerDashboard", model.Where(m => m.HasContract == true));
            }
            return PartialView("_SearchResultCustomerDashboard", model);
        }

        public ActionResult SingleJobDetails(int jobId)
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
            var model = _db.JOBS.Where(j => j.JobId == jobId).FirstOrDefault();
            return View("_JobListItem", model);
        }

        public ActionResult AllJobs(string searchTerm = null, int page = 1, string searchDate = null)
        {
            IPagedList<JOB> model = null;
            if (searchDate == null || searchDate.Trim() == "")
            {
                model = _db.JOBS
               .Where(r => searchTerm == null || r.CustomerName.StartsWith(searchTerm) || r.DRVR_DATA.DriverName.StartsWith(searchTerm) || r.PickupAddress.Contains(searchTerm) || r.DropAddress.Contains(searchTerm) || r.CustomerPhone.Contains(searchTerm))
               .OrderByDescending(r => r.JobDate)
               .ThenBy(r => r.CustomerName)
               .ToPagedList(page, 20);
            }
            else
            {
                DateTime date = Convert.ToDateTime(searchDate);
                model = _db.JOBS
                    .Where(r => r.JobDate.Year == date.Year && r.JobDate.Month == date.Month && r.JobDate.Day == date.Day)
                    .OrderByDescending(r => r.JobDate)
                    .ThenBy(r => r.CustomerName)
                    .ToPagedList(page, 20);
            }
            ViewBag.searchTerm = searchTerm;
            ViewBag.searchDate = searchDate;


            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_JobsTable", model);
            //}
            return View(model);
        }

        public ActionResult DriverActiveJobs(int driverId)
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
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            string dropped = StringEnum.GetStringValue(StatusCode.DroppedOff);

            var jobs = _db.JOBS.Where(j => j.DriverId == driverId && !(j.JobStatus.Equals(closed) || j.JobStatus.Equals(dropped))).ToList();
            return View(jobs);
        }

        public ActionResult DailyJobs(string searchTerm = null, int page = 1)
        {
            var model = _db.CUST_DATA
                .Where(r => (searchTerm == null || r.CustomerName.Contains(searchTerm)) && r.IsDeleted != true && r.HasAccount == true)
                .OrderBy(r => r.CustomerName)
                .ToPagedList(page, 15);

            ViewBag.searchTerm = searchTerm;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CustomersTable", model);
            }
            return View("DailyCustomers", model);
        }

        public ActionResult PendingJobs(int page = 1)
        {
            var timeNow = DateTime.Now.ToUniversalTime();
            var pending = StringEnum.GetStringValue(StatusCode.Pending);
            var model = _db.PNDG_JOBS
                .Where(j => j.ScheduledTime > timeNow && j.JobStatus==pending )
                .OrderByDescending(j => j.ScheduledTime)
                .ToPagedList(page, 10);
            return View(model);
        }


        //public ActionResult CreateDailyJob(string customerId=null) {
        //    DailyParcelEntity model = new DailyParcelEntity() { ParcelDetails = new DALY_PRCL_DETL(), DailyParcels = new List<DALY_PRCL>() };
        //    var drivers = _db.DRVR_DATA.Where(d => d.IsDeleted != true).OrderBy(d => d.DriverName);
        //    var selectList = new List<SelectListItem>();
        //    foreach (var item in drivers)
        //    {
        //        selectList.Add(new SelectListItem
        //        {
        //            Value = item.DriverId.ToString(),
        //            Text = item.DriverName
        //        });
        //    }
        //    ViewBag.DriversList = selectList;
        //    var typeOfParcelList = new List<SelectListItem>();
        //    foreach (var item in TypesOfParcel)
        //    {
        //        typeOfParcelList.Add(new SelectListItem
        //        {
        //            Value = item,
        //            Text = item
        //        });
        //    }
        //    ViewBag.TypesOfParcel = typeOfParcelList;

        //    //var customerAccountInd = false;
        //    //var customerContractInd = false;
        //    if (customerId != null)
        //    {
        //        try
        //        {

        //            int id = int.Parse(customerId);
        //            var customerDetails = _db.CUST_DATA.Find(id);
        //            if (customerDetails != null)
        //            {
        //                model.ParcelDetails= new DALY_PRCL_DETL() { CustomerName = customerDetails.CustomerName, CustomerId = customerDetails.CustomerId, CustomerPhone = customerDetails.ContactNo, PickupAddress = customerDetails.Address };

        //                if (customerDetails.HasAccount != null)
        //                    ViewBag.CustomerAccountInd = (bool)customerDetails.HasAccount;
        //                if (customerDetails.HasContract != null)
        //                    ViewBag.CustomerContractInd = (bool)customerDetails.HasContract;

        //                return View(model);
        //            }
        //        }
        //        catch (Exception ex)
        //        { }
        //    }
        //    ViewBag.CustomerAccountInd = null;
        //    ViewBag.CustomerContractInd = null;
        //    return View(model);
        //}
        #endregion

        #region Form Actions and Post methods
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendJob(JOB job)
        {
            try
            {
                job.Price = job.Price == null ? decimal.Parse("0.00") : job.Price;
                if (ModelState.ContainsKey("Price"))
                    ModelState["Price"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    SendJobToDrivers(job);
                    return RedirectToAction("Dashboard", "Index");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }
            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "Job Posting");
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCustomerAndSendNewJob(JOB job)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SaveNewCustomer(job);
                    SendJobToDrivers(job);
                    return RedirectToAction("Dashboard", "Index");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }

            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "Job Posting");
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendJobDashboard(JOB job)
        {
            try
            {
                job.Price = job.Price == null ? decimal.Parse("0.00") : job.Price;
                if (ModelState.ContainsKey("Price"))
                    ModelState["Price"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    SendJobToDrivers(job);
                    return RedirectToAction("Dashboard", "Index");
                }
                else
                {
                    var errors = ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .Select(x => new { x.Key, x.Value.Errors })
                                .ToArray();
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }
            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "Job Posting");
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCustomerAndSend(JOB job)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SaveNewCustomer(job);
                    SendJobToDrivers(job);
                    return RedirectToAction("Dashboard", "Index");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }
            catch (Exception ex)
            {
                new CustomException().SaveExceptionToDB(ex, "Job Posting");
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return null;
        }


        public ActionResult CancelJob(int JobId)
        {
            var job = _db.JOBS.Find(JobId);
            try
            {
                var jobsResponse = _db.JOBS_RESP.Where(j => j.JobId == JobId);
                foreach (var response in jobsResponse)
                    _db.Entry(response).State = EntityState.Deleted;


                job.JobStatus = StringEnum.GetStringValue(StatusCode.Closed);
                _db.Entry(job).State = EntityState.Deleted;

                if (job.DriverId != null)
                {
                    var driver = _db.DRVR_DATA.Find(job.DriverId);
                    GcmSender.SendToSingle(driver, "Job from : " + job.CustomerName + " has been cancelled.", "job_changed");
                }
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully cancelled.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Job could not be cancelled.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        public ActionResult EditJob(int JobId)
        {
            var customerAccountInd = false;
            var model = _db.JOBS.Find(JobId);
            if (model.CUST_DATA != null)
            {
                if (model.CUST_DATA.HasAccount != null)
                    customerAccountInd = (bool)model.CUST_DATA.HasAccount;
            }
            model.longDistanceCheckboxValue = model.LongDistanceInd.GetValueOrDefault(false);
            ViewBag.CustomerAccountInd = customerAccountInd;
            ViewBag.fromEdit = true;
            var typeOfParcelList = new List<SelectListItem>();
            foreach (var item in TypesOfParcel)
            {
                typeOfParcelList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }
            ViewBag.TypesOfParcel = typeOfParcelList;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveEditJob(JOB job,string HangfireId=null)
        {
            if (HangfireId != null)
            {
                return UpdateScheduledJob(job, HangfireId);
            }
            job.LastUpdated = DateTime.Now.ToUniversalTime();
            job.LongDistanceInd = job.longDistanceCheckboxValue == true ? true : false;
            job.AccountPaymentInd = job.PaymentMode == (int)PaymentModes.Account ? true : job.PaymentMode == (int)PaymentModes.Contract ? true : false;

            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            string dropped = StringEnum.GetStringValue(StatusCode.DroppedOff);
            try
            {
                if (ModelState.IsValid)
                {

                    var dbJob = _db.JOBS.Find(job.JobId);
                    if (dbJob.JobStatus == dropped || dbJob.JobStatus == closed)
                    {
                        if (dbJob.Price != job.Price)       //If aamount is entered into the system and after finishing the job the amount is changed
                        {
                            var previousJobPrice = dbJob.Price;
                            var updatedJobPrice = job.Price;

                            var trans = _db.CUST_TRAN.Where(t => t.JobId == job.JobId);
                            foreach (var item in trans)
                            {
                                item.PayableAmount = Math.Round(((updatedJobPrice / previousJobPrice) * item.PayableAmount).GetValueOrDefault(0), 2);
                                item.RemainingAmount = Math.Round(((updatedJobPrice / previousJobPrice) * item.RemainingAmount).GetValueOrDefault(0), 2);
                                item.SettledInd = false;
                                _db.Entry(item).State = EntityState.Modified;

                            }

                            var drverTrans = _db.DRVR_TRAN.Where(t => t.JobId == job.JobId);
                            foreach (var drvrTransItem in drverTrans)
                            {
                                var oldAmount = drvrTransItem.Amount;
                                drvrTransItem.Amount = Math.Round(((updatedJobPrice / previousJobPrice) * drvrTransItem.Amount).GetValueOrDefault(0), 2);
                                drvrTransItem.Balance = Math.Round(((drvrTransItem.Amount - oldAmount) + drvrTransItem.Balance).GetValueOrDefault(0), 2);
                                drvrTransItem.SettledInd = false;
                                _db.Entry(drvrTransItem).State = EntityState.Modified;
                            }
                        }
                    }
                    ((IObjectContextAdapter)_db).ObjectContext.Detach(dbJob);
                    _db.Entry(job).State = EntityState.Modified;

                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('Job has been updated in the system.');</script>";
                    if (job.DriverId != null)
                    {
                        var driver = _db.DRVR_DATA.Find(job.DriverId);
                        GcmSender.SendToSingle(driver, "Job from : " + job.CustomerName + " has been updated.", "job_changed");
                    }
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('There were some invalid inputs, job could not be updated.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Job could not be update due to system error.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }



        [HttpPost]
        public ActionResult AddCharges(AddChagesViewModel model)
        {
            try
            {
                var job = _db.JOBS.Find(model.JobId);
                var previousJobPrice = job.Price;
                var updatedJobPrice = job.Price + model.Price;
                job.Price = job.Price + model.Price;

                var trans = _db.CUST_TRAN.Where(t => t.JobId == model.JobId);
                foreach (var item in trans)
                {
                    item.PayableAmount += model.Price;
                    item.RemainingAmount += model.Price;
                    item.SettledInd = false;
                    _db.Entry(item).State = EntityState.Modified;

                }

                var drverTrans = _db.DRVR_TRAN.Where(t => t.JobId == model.JobId);
                foreach (var drvrTransItem in drverTrans)
                {
                    var oldAmount = drvrTransItem.Amount;
                    drvrTransItem.Amount = Math.Round(((updatedJobPrice / previousJobPrice) * drvrTransItem.Amount).GetValueOrDefault(0), 2);
                    drvrTransItem.Balance = Math.Round(((drvrTransItem.Amount - oldAmount) + drvrTransItem.Balance).GetValueOrDefault(0), 2);
                    drvrTransItem.SettledInd = false;
                    _db.Entry(drvrTransItem).State = EntityState.Modified;
                }
                if ((job.Notes == null ? "" : job.Notes).Trim() == "")
                {
                    job.Notes = model.ChargesDescription;
                }
                else
                {
                    job.Notes = (job.Notes == null ? "" : job.Notes) + "\n" + model.ChargesDescription;
                }
                if ((job.ChargesDescription == null ? "" : job.ChargesDescription).Trim() == "")
                {
                    job.ChargesDescription = model.ChargesDescription;
                }
                else
                {
                    job.ChargesDescription = (job.ChargesDescription == null ? "" : job.ChargesDescription) + "\n" + model.ChargesDescription;
                }
                _db.Entry(job).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Job has been updated in the system.');</script>";

            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Job could not be update due to system error.');</script>";
            }
            return RedirectToAction("SingleJobDetails", new { jobId = model.JobId });
        }

        [HttpPost]
        public ActionResult ReassignDriver(JOB job)
        {
            var dbJob = _db.JOBS.Find(job.JobId);
            try
            {

                if (dbJob.DriverId != null)
                {
                    var driver = _db.DRVR_DATA.Find(dbJob.DriverId);
                    GcmSender.SendToSingle(driver, "Job from : " + dbJob.CustomerName + " has been cancelled.", "job_changed");
                }
                dbJob.JobStatus = StringEnum.GetStringValue(StatusCode.Assigned);
                dbJob.AssignedTime = DateTime.Now.ToUniversalTime();
                dbJob.DriverId = job.DriverId;//assign new driver to the job
                var newDriver = _db.DRVR_DATA.Find(job.DriverId);   //get information of newly assigned driver
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("New Job has been assigned to you.");
                sb.AppendLine(" ");
                sb.Append("Pickup from: " + dbJob.PickupAddress);
                string gcmHeader = dbJob.LongDistanceInd.Value == true ? "long_distance_job_assigned" : "specific_job_assigned";
                GcmSender.SendToSingle(newDriver, sb.ToString(), gcmHeader);
                _db.Entry(dbJob).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully reassigned to new driver.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Job could not be modified.');</script>";
            }
            return RedirectToAction("SingleJobDetails", new { jobId = job.JobId });
        }

        [HttpGet]
        public ActionResult SendInvoice(int JobId)
        {
            try
            {
                var job = _db.JOBS.Find(JobId);
                if (job.CustomerId == null || job.CUST_DATA == null || job.CUST_DATA.EmailAddress == null || job.CUST_DATA.EmailAddress.Trim().Equals(""))
                    throw new Exception("Customer does not have an email address configured in the system.");
                var lastEmailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
                if (lastEmailAccount == null || lastEmailAccount.EmailAddress == null || lastEmailAccount.Password == null || lastEmailAccount.EmailClient == null)
                {
                    throw new Exception("Please configure a valid email address from settings to send an email.");
                }
                List<CustomerJobDriver> reportParameters = new List<CustomerJobDriver>();
                reportParameters.Add(new CustomerJobDriver()
                {
                    JobId = job.JobId,
                    JobDate = job.JobDate,
                    PickupAddress = job.PickupAddress,
                    DropAddress = job.DropAddress,
                    RemainingAmount = job.Price.GetValueOrDefault(0),
                    ChargeDescription = job.ChargesDescription,
                    DropAddress1 = job.DropAddress1,
                    DropAddress2 = job.DropAddress2,
                    DropAddress3 = job.DropAddress3,
                    DropAddress4 = job.DropAddress4,
                    Reference = job.Reference
                });

                //bool isPaid = false;
                //if (job.PaymentReceived == true || job.JobStatus == StringEnum.GetStringValue(StatusCode.Closed) || job.JobStatus == StringEnum.GetStringValue(StatusCode.DroppedOff) || job.JobStatus == StringEnum.GetStringValue(StatusCode.PaymentReceived))
                //{
                //    isPaid = true;
                //}
                string reportMarkup;
                string result = PDFGenerator.createCustomerReportMarkup(out reportMarkup,job.CUST_DATA, reportParameters, 0, false);
                if (result != null)
                {
                    TempData["toastMessage"] = "<script>toastr.success('Invoice has been successfully sent to customer.');</script>";
                    CUST_INVC invoice = new CUST_INVC()
                    {
                        InvoiceNumber = result,
                        InvoiceAmount = (reportParameters.Sum(p => p.RemainingAmount)),
                        InvoiceDate = DateTime.Now.ToUniversalTime(),
                        CustomerId = job.CustomerId,
                        JobId = job.JobId,
                        InvoiceStatus = StringEnum.GetStringValue(InvoiceStatus.Due),
                        EmailAddress = job.CUST_DATA.EmailAddress,
                        ReportMarkup = System.Net.WebUtility.HtmlEncode(reportMarkup),
                        IsPaid = false
                    };
                    _db.CUST_INVC.Add(invoice);
                    _db.SaveChanges();
                }
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
            return RedirectToAction("SingleJobDetails", new { jobId = JobId });

        }
        [HttpGet]
        public ActionResult SendInvoiceToEmail(CustomerJobDriver model)
        {
            try
            {
                var job = _db.JOBS.Find(model.JobId);
                if (model.EmailAddress == null || model.EmailAddress.Trim().Equals(""))
                    throw new Exception("Please provide a valid email address.");
                var lastEmailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
                if (lastEmailAccount == null || lastEmailAccount.EmailAddress == null || lastEmailAccount.Password == null || lastEmailAccount.EmailClient == null)
                {
                    throw new Exception("Please configure a valid email address from settings to send an email.");
                }
                List<CustomerJobDriver> reportParameters = new List<CustomerJobDriver>();
                reportParameters.Add(new CustomerJobDriver()
                {
                    JobId = job.JobId,
                    JobDate = job.JobDate,
                    PickupAddress = job.PickupAddress,
                    DropAddress = job.DropAddress,
                    RemainingAmount = job.Price.GetValueOrDefault(0),
                    ChargeDescription = job.ChargesDescription,
                    DropAddress1 = job.DropAddress1,
                    DropAddress2 = job.DropAddress2,
                    DropAddress3 = job.DropAddress3,
                    DropAddress4 = job.DropAddress4,
                    Reference = job.Reference
                });
                CUST_DATA customerInformation = new CUST_DATA() { EmailAddress = model.EmailAddress, Address = job.PickupAddress, ContactNo = job.CustomerPhone, CustomerName = job.CustomerName, HasAccount = false, IsDeleted = false };
                //Check for paid (closed job is always paid)
                //bool isPaid = false;
                //if (job.PaymentReceived == true || job.JobStatus == StringEnum.GetStringValue(StatusCode.Closed) || job.JobStatus == StringEnum.GetStringValue(StatusCode.DroppedOff) || job.JobStatus == StringEnum.GetStringValue(StatusCode.PaymentReceived))
                //{
                //    isPaid = true;
                //}
                string reportMarkup;
                string result = PDFGenerator.createCustomerReportMarkup(out reportMarkup, customerInformation, reportParameters, 0, false);
                if (result != null)
                {
                    TempData["toastMessage"] = "<script>toastr.success('Invoice has been successfully sent to customer.');</script>";
                    CUST_INVC invoice = new CUST_INVC()
                    {
                        InvoiceNumber = result,
                        InvoiceAmount = (reportParameters.Sum(p => p.RemainingAmount)),
                        InvoiceDate = DateTime.Now.ToUniversalTime(),
                        CustomerId = job.CustomerId,
                        JobId = job.JobId,
                        InvoiceStatus = StringEnum.GetStringValue(InvoiceStatus.Due),
                        EmailAddress=customerInformation.EmailAddress,
                        ReportMarkup = System.Net.WebUtility.HtmlEncode(reportMarkup),
                        IsPaid = false
                    };
                    _db.CUST_INVC.Add(invoice);
                    _db.SaveChanges();
                }
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
            return RedirectToAction("SingleJobDetails", new { jobId = model.JobId });

        }

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult _EnterEmail()
        {
            return PartialView("_EnterEmail");
        }

        [HttpGet]
        public ActionResult _SelectDriver()
        {
            return PartialView("_SelectDriver");
        }

        public ActionResult CancelPendingJob(int JobId)
        {
            var job = _db.PNDG_JOBS.Find(JobId);
            try
            {
                job.JobStatus = StringEnum.GetStringValue(StatusCode.Closed);
                _db.Entry(job).State = EntityState.Deleted;
                _db.SaveChanges();

                Hangfire.BackgroundJob.Delete(job.HangfireId);

                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully cancelled.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Job could not be cancelled.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }
        public ActionResult EditPendingJob(int JobId)
        {
            var customerAccountInd = false;
            PNDG_JOBS pendingJob = _db.PNDG_JOBS.Find(JobId);
            if (pendingJob.CustomerId != null&& pendingJob.CustomerId>0)
            {
                var customer = _db.CUST_DATA.Find(pendingJob.CustomerId);
                if (customer.HasAccount != null)
                    customerAccountInd = (bool)customer.HasAccount;
            }
            JOB model = new JOB();
            model = CopyJobDetails(pendingJob, model);
            model.longDistanceCheckboxValue = model.LongDistanceInd.GetValueOrDefault(false);
            string timeZone = ConfigurationManager.AppSettings["timeZone"].ToString();
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var jobTimeUtc = TimeZoneInfo.ConvertTimeFromUtc(pendingJob.ScheduledTime.GetValueOrDefault(new DateTime()), timeZoneInfo);
            model.SendDateTime = jobTimeUtc.ToString();
            ViewBag.CustomerAccountInd = customerAccountInd;
            ViewBag.fromEdit = true;
            ViewBag.HangfireId = pendingJob.HangfireId;
            var typeOfParcelList = new List<SelectListItem>();
            foreach (var item in TypesOfParcel)
            {
                typeOfParcelList.Add(new SelectListItem
                {
                    Value = item,
                    Text = item
                });
            }
            ViewBag.TypesOfParcel = typeOfParcelList;

            return View("EditJob",model);
        }


        //public ActionResult AddSchedule(DailyParcelEntity DailyParcel)
        //{
        //  //  DailyParcel.DailyParcels.Add(new DALY_PRCL() { DayOfWeek = (int)DayOfWeek.Monday, TimeOfDay = "00:10" });
        //    return PartialView("_DailyParcelSchedule", DailyParcel);
        //}
        #endregion

        #region Private methods
        private void SendJobToDrivers(JOB job)
        {
            if (job.SendDateTime == null)
                _SendJob(job);
            else
            {
                DateTime sendTime = Convert.ToDateTime(job.SendDateTime);
                string timeZone = ConfigurationManager.AppSettings["timeZone"].ToString();
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var jobTimeUtc = TimeZoneInfo.ConvertTimeToUtc(sendTime, timeZoneInfo);
                var utcTimeNow = DateTime.Now.ToUniversalTime();
                TimeSpan ts = jobTimeUtc - utcTimeNow;
                var id = Hangfire.BackgroundJob.Schedule(() => _SendJob(job), ts);

                PNDG_JOBS pendingJob = new PNDG_JOBS();
                pendingJob = CopyJobDetails(job, pendingJob);
                pendingJob.HangfireId = id;
                pendingJob.JobStatus = StringEnum.GetStringValue(StatusCode.Pending);
                pendingJob.ScheduledTime = jobTimeUtc;
                pendingJob.JobDate = DateTime.Now.ToUniversalTime();
                _db.PNDG_JOBS.Add(pendingJob);
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Job has been scheduled and will be sent at selected time.');</script>";


            }
        }

        public void _SendJob(JOB job)
        {
            job.LongDistanceInd = job.longDistanceCheckboxValue == true ? true : false;
            job.JobDate = (DateTime.Now).ToUniversalTime();
            job.LastUpdated = DateTime.Now.ToUniversalTime();
            job.AccountPaymentInd = job.PaymentMode == (int)PaymentModes.Account ? true : job.PaymentMode == (int)PaymentModes.Contract ? true : false;



            if (job.DriverId == null)
            {
                job.JobStatus = StringEnum.GetStringValue(StatusCode.Open);
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
                    try
                    {
                        Task.Run(() => GcmSender.SendToSingle(driver, sb.ToString(), "new_job"));
                        // GcmSender.SendToSingle(driver, sb.ToString(), "new_job");
                    }
                    catch (Exception ex)
                    {
                    }
                }

                //    GcmSender.SendToAll("New job available. Pickup address: " + job.PickupAddress, "new_job");
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully added and sent out to drivers.');</script>";
            }
            else
            {
                job.JobStatus = StringEnum.GetStringValue(StatusCode.Assigned);
                job.AssignedTime = DateTime.Now.ToUniversalTime();
                var driver = _db.DRVR_DATA.Find(job.DriverId);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("New Job has been assigned to you.");
                sb.AppendLine(" ");
                sb.Append("Pickup from: " + job.PickupAddress);
                string gcmHeader = job.LongDistanceInd.Value == true ? "long_distance_job_assigned" : "specific_job_assigned";
                GcmSender.SendToSingle(driver, sb.ToString(), gcmHeader);
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully sent out to the selected driver.');</script>";

            }
            _db.JOBS.Add(job);
            _db.SaveChanges();
        }

        private void SaveNewCustomer(JOB job)
        {
            try
            {
                if (job.CustomerId != null && job.CustomerId > 0)
                    throw new CustomException() { CustomMessage = "Customer already exists in the system." };

                var existingCustomerCount = _db.CUST_DATA.Count(c => c.CustomerName.ToLower() == job.CustomerName.ToLower() && c.Address.ToLower() == job.PickupAddress.ToLower() && c.ContactNo.Trim() == job.CustomerPhone.Trim());
                if (existingCustomerCount > 0)
                    throw new CustomException() { CustomMessage = "Customer already exists in the system." };
                CUST_DATA customer = new CUST_DATA();
                customer.Address = job.PickupAddress;
                customer.CustomerName = job.CustomerName;
                customer.IsDeleted = false;
                customer.ContactNo = job.CustomerPhone;
                customer.HasAccount = false;
                _db.CUST_DATA.Add(customer);
                _db.SaveChanges();
                TempData["toastMessage1"] = "<script>toastr.success('Customer has been successfully saved into the system.');</script>";


            }
            catch (CustomException cex)
            {
                TempData["toastMessage1"] = "<script>toastr.warning('" + cex.CustomMessage + "');</script>";

            }
            catch (Exception ex)
            {
                TempData["toastMessage1"] = "<script>toastr.error('Customer could not be saved in the system.');</script>";
            }
        }

        private void SendNewJob(JOB job)
        {
            job.LongDistanceInd = job.longDistanceCheckboxValue == true ? true : false;
            job.JobDate = (DateTime.Now).ToUniversalTime();
            job.LastUpdated = DateTime.Now.ToUniversalTime();
            job.AccountPaymentInd = job.PaymentMode == (int)PaymentModes.Account ? true : false;

            if (job.DriverId == null)
            {
                job.JobStatus = StringEnum.GetStringValue(StatusCode.Open);
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

                //    GcmSender.SendToAll("New job available. Pickup address: " + job.PickupAddress, "new_job");
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully added and sent out to drivers.');</script>";
            }
            else
            {
                job.JobStatus = StringEnum.GetStringValue(StatusCode.Assigned);
                var driver = _db.DRVR_DATA.Find(job.DriverId);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("New Job has been assigned to you.");
                sb.AppendLine(" ");
                sb.Append("Pickup from: " + job.PickupAddress);
                GcmSender.SendToSingle(driver, sb.ToString(), "long_distance_job_assigned");
                TempData["toastMessage"] = "<script>toastr.success('Job has been successfully sent out to the selected driver.');</script>";

            }
            _db.JOBS.Add(job);
            _db.SaveChanges();
        }

        private ActionResult UpdateScheduledJob(JOB job, string hangfireId)
        {
            try
            {
                job.Price = job.Price == null ? decimal.Parse("0.00") : job.Price;
                if (ModelState.ContainsKey("Price"))
                    ModelState["Price"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    var pendingJob = _db.PNDG_JOBS.FirstOrDefault(p => p.HangfireId == hangfireId);
                    if (pendingJob != null)
                    {
                        pendingJob.JobStatus = StringEnum.GetStringValue(StatusCode.Sent);
                        _db.Entry(pendingJob).State = EntityState.Modified;
                    }
                    Hangfire.BackgroundJob.Delete(hangfireId);
                    SendJobToDrivers(job);
                    return RedirectToAction("Dashboard", "Index");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Job has been updated.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to update the job.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        private dynamic CopyJobDetails(dynamic source, dynamic target)
        {
            target.AccountPaymentInd = source.AccountPaymentInd;
            target.ChargesDescription = source.ChargesDescription;
            target.CustomerId = source.CustomerId;
            target.CustomerName = source.CustomerName;
            target.CustomerPhone = source.CustomerPhone;
            target.DropAddress = source.DropAddress;
            target.DropAddress1 = source.DropAddress1;
            target.DropAddress2 = source.DropAddress2;
            target.DropAddress3 = source.DropAddress3;
            target.DropAddress4 = source.DropAddress4;
            target.DropoffContact = source.DropoffContact;
            target.IsPaid = source.IsPaid;
            target.JobDate = source.JobDate;
            target.JobStatus = source.JobStatus;
            target.Notes = source.Notes;
            target.PaymentMode = source.PaymentMode;
            target.PickupAddress = source.PickupAddress;
            target.Price = source.Price;
            target.Reference = source.Reference;
            target.TypeOfParcel = source.TypeOfParcel;

            if (target.GetType() == typeof(PNDG_JOBS))
                target.LongDistanceInd = source.longDistanceCheckboxValue == true ? true : false;
            else
                target.LongDistanceInd = source.LongDistanceInd;

            return target;
        }

        #endregion

    }
}
