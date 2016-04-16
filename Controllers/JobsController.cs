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
using System.Data.Entity.Validation;

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

        public ActionResult CustomerSearch(string searchTerm = null)
        {
            var model = _db.CUST_DATA
                .Where(c => searchTerm == null || c.CustomerName.Contains(searchTerm) || c.Address.Contains(searchTerm) || c.ContactNo.Contains(searchTerm))
                .OrderBy(c => c.CustomerName);

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

            var customerAccountInd = false;
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
                            customerAccountInd = (bool)customerDetails.HasAccount;
                        ViewBag.CustomerAccountInd = customerAccountInd;

                        return View(model);
                    }
                }
                catch (Exception ex)
                { }
            }
            ViewBag.CustomerAccountInd = customerAccountInd;
            return View();
        }

        public ActionResult ActiveJobs(int page = 1)
        {
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            var model = _db.JOBS
                .Where(j => j.JobStatus != closed)
                .OrderByDescending(j => j.JobDate)
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
                .OrderByDescending(j => j.JobDate)
                .ToPagedList(page, 10);
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
                .OrderByDescending(j => j.JobDate)
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

            var customerAccountInd = false;
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
                            customerAccountInd = (bool)customerDetails.HasAccount;
                        ViewBag.CustomerAccountInd = customerAccountInd;

                        return PartialView(model);
                    }
                }
                catch (Exception ex)
                { }
            }
            ViewBag.CustomerAccountInd = customerAccountInd;
            return PartialView();
        }

        public ActionResult CustomerSearchDashboard(string searchTerm = null)
        {
            var model = _db.CUST_DATA
                .Where(c => searchTerm == null || c.CustomerName.Contains(searchTerm) || c.Address.Contains(searchTerm) || c.ContactNo.Contains(searchTerm))
                .OrderBy(c => c.CustomerName);

            return PartialView("_SearchResultCustomerDashboard", model);
        }

        public ActionResult SingleJobDetails(int jobId)
        {
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
            string closed = StringEnum.GetStringValue(StatusCode.Closed);
            string dropped = StringEnum.GetStringValue(StatusCode.DroppedOff);

            var jobs = _db.JOBS.Where(j => j.DriverId == driverId && !(j.JobStatus.Equals(closed) || j.JobStatus.Equals(dropped))).ToList();
            return View(jobs);
        }

        #endregion

        #region Form Actions and Post methods
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendJob(JOB job)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SendJobToDrivers(job);
                    return RedirectToAction("Home", "Index", new { refreshRequired = true });
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return null;
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
                    return RedirectToAction("Home", "Index", new { refreshRequired = true });
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Sorry, the job could not be posted.');</script>";
                }
            }

            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendJobDashboard(JOB job)
        {
            try
            {
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
                TempData["toastMessage"] = "<script>toastr.error('An error occured while trying to post the job.');</script>";
            }
            return null;
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
        public ActionResult saveEditJob(JOB job)
        {
            job.LongDistanceInd = job.longDistanceCheckboxValue == true ? true : false;
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
                                item.PayableAmount = (updatedJobPrice / previousJobPrice) * item.PayableAmount;
                                item.RemainingAmount = (updatedJobPrice / previousJobPrice) * item.RemainingAmount;
                                item.SettledInd = false;
                                _db.Entry(item).State = EntityState.Modified;

                            }

                            var drverTrans = _db.DRVR_TRAN.Where(t => t.JobId == job.JobId);
                            foreach (var drvrTransItem in drverTrans)
                            {
                                var oldAmount = drvrTransItem.Amount;
                                drvrTransItem.Amount = (updatedJobPrice / previousJobPrice) * drvrTransItem.Amount;
                                drvrTransItem.Balance = (drvrTransItem.Amount - oldAmount) + drvrTransItem.Balance;
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
                    drvrTransItem.Amount = (updatedJobPrice / previousJobPrice) * drvrTransItem.Amount;
                    drvrTransItem.Balance = (drvrTransItem.Amount - oldAmount) + drvrTransItem.Balance;
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

        #endregion

        #region Private methods
        private void SendJobToDrivers(JOB job)
        {
            job.LongDistanceInd = job.longDistanceCheckboxValue == true ? true : false;
            job.JobDate = (DateTime.Now).ToUniversalTime();


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

        #endregion

    }
}
