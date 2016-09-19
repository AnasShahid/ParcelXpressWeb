using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using System.Data;
using ParcelXpress.Helpers;
using ParcelXpress.Enums;

namespace ParcelXpress.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        //
        // GET: /Driver/
        ParcelXpressConnection _db = new ParcelXpressConnection();

        public ActionResult DriverRequests()
        {

            var driverRequestList = from driver in _db.REQT_DRVR
                                    where driver.IsConverted == false
                                    orderby driver.DriverReqId
                                    select driver;

            return View(driverRequestList);
        }

        public ActionResult AllDrivers(string searchTerm = null)
        {
            var model = _db.DRVR_DATA
                .Where(r => (searchTerm == null || r.DriverName.StartsWith(searchTerm)) && r.IsDeleted != true)
                .OrderBy(r => r.DriverName);

            int newReqCount = _db.REQT_DRVR.Where(r => r.IsConverted == false).Count();
            ViewData["newRequestCount"] = newReqCount;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DriversTable", model);
            }
            return View(model);
        }

        public ActionResult DeleteDriverRequest(int id)
        {
            REQT_DRVR requestData = new REQT_DRVR();
            requestData = _db.REQT_DRVR.Find(id);
            if (requestData != null)
            {
                _db.Entry(requestData).State = EntityState.Deleted;
                _db.SaveChanges();

                TempData["toastMessage"] = "<script>toastr.success('Request has been successfully deleted.');</script>";
            }
            return RedirectToAction("DriverRequests");
        }

        public ActionResult RegisterDriver(int id)
        {
            REQT_DRVR requestData = new REQT_DRVR();
            requestData = _db.REQT_DRVR.Find(id);

            DRVR_DATA driverData = new DRVR_DATA()
            {
                DriverName = requestData.DriverName,
                LoginName = requestData.LoginName,
                Password = requestData.Password,
                GcmId = requestData.GcmId,
                SecurityQuestionId = requestData.SecurityQuestionId,
                SecurityAnswer = requestData.SecurityAnswer,
                IsActive = false,
                ContactNo = requestData.ContactNo
            };
            ViewData["requestId"] = id;

            return View(driverData);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult saveNewDriver(DRVR_DATA driver, int reqtId)
        {
            REQT_DRVR requestData = new REQT_DRVR();
            requestData = _db.REQT_DRVR.Find(reqtId);
            try
            {
                driver.CommissionRate = driver.CommissionRate == null ? 0.00M : driver.CommissionRate;
                if (ModelState.ContainsKey("CommissionRate"))
                    ModelState["CommissionRate"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    driver.IsDeleted = false;
                    driver.CreatedOn = (DateTime.Now).ToUniversalTime();
                    _db.DRVR_DATA.Add(driver);

                    _db.Entry(requestData).State = EntityState.Modified;
                    requestData.IsConverted = true;
                    _db.SaveChanges();

                    TempData["toastMessage"] = "<script>toastr.success('Driver has been successfully saved into the system.');</script>";
                    GcmSender.SendToSingle(driver, "You are now registered into the system. You can use your Id password to login.", "register_success");
                    return RedirectToAction("AllDrivers");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Driver data could not be saved.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured while saving the driver.');</script>";
            }
            return RedirectToAction("RegisterDriver", new { id = reqtId });
        }

        public ActionResult EditDriver(int id)
        {
            DRVR_DATA driverData = _db.DRVR_DATA.Find(id);
            return View(driverData);
        }

        [HttpPost]
        public ActionResult editDriverInformation(DRVR_DATA driver)
        {
            try
            {
                driver.CommissionRate = driver.CommissionRate == null ? 0.00M : driver.CommissionRate;
                if (ModelState.ContainsKey("CommissionRate"))
                    ModelState["CommissionRate"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    _db.Entry(driver).State = EntityState.Modified;
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('Driver has been successfully updated in the system.');</script>";

                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Driver data could not be updated.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An exception occured while trying to update the driver information.');</script>";
            }
            return RedirectToAction("AllDrivers");
        }

        [HttpPost]
        public ActionResult DeleteDriver(int DriverId)
        {
            try
            {
                var driver = _db.DRVR_DATA.Find(DriverId);
                driver.IsDeleted = true;
                _db.Entry(driver).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Driver has been successfully deleted from the system.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('There was an error while processing your request.');</script>";
            }
            return RedirectToAction("AllDrivers");
        }

        public ActionResult _ActiveDriversDashboard()
        {
            var model = _db.DRVR_DATA.Where(d => d.IsActive == true && d.IsDeleted != true).ToList();
            foreach (var item in model)
            {
                item.ActiveJobsCount = item.JOBS.Where(p => (p.JobStatus == StringEnum.GetStringValue(StatusCode.Assigned)) || (p.JobStatus == StringEnum.GetStringValue(StatusCode.PickedUp)) || (p.JobStatus == StringEnum.GetStringValue(StatusCode.DroppedOff))).Count();
                item.isTimeIn = item.DRVR_TIME_SHET.LastOrDefault(p => p.IsLoggedIn == true) != null;
                item.AdditionalInfo = item.DRVR_TIME_SHET.LastOrDefault(d => d.IsLoggedIn == true) != null ? TimezoneHelper.ConvertUTCtoLocal(item.DRVR_TIME_SHET.LastOrDefault(d => d.IsLoggedIn == true).LoginTime.Value).ToString("hh:mm tt") : "";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ActiveDriversList", model);
            }
            return PartialView(model);
        }

        public ActionResult AcceptHourlyDriver(int driverId)
        {
            try
            {
                var driver = _db.DRVR_DATA.Find(driverId);
                if (driver.IsOnHourlyRate == false || !driver.HourlyRate.HasValue || driver.HourlyRate <= 0)
                {
                    throw new Exception("Hourly rate is not defined for selected driver");
                }
                DRVR_TIME_SHET timeSheet = new DRVR_TIME_SHET()
                {
                    DriverId = driver.DriverId,
                    HourlyRate = driver.HourlyRate,
                    IsLoggedIn = true,
                    LoginTime = DateTime.Now.ToUniversalTime(),
                    SettledInd = false
                };
                _db.DRVR_TIME_SHET.Add(timeSheet);
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Driver has been successfully accepted. Driver time recording has been started');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('" + ex.Message + "');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        public ActionResult LogoutHourlyDriver(int driverId)
        {
            try
            {
                var driverTimeSheet = _db.DRVR_TIME_SHET.Where(d => d.DriverId == driverId && d.IsLoggedIn == true);
                foreach (var item in driverTimeSheet)
                {
                    item.IsLoggedIn = false;
                    item.LogoutTime = DateTime.Now.ToUniversalTime();
                    TimeSpan hoursWorked = (item.LogoutTime - item.LoginTime).GetValueOrDefault(new TimeSpan());
                    item.AmountEarned = (decimal)(hoursWorked.TotalHours) * item.HourlyRate.GetValueOrDefault(0);
                    _db.Entry(item).State = EntityState.Modified;
                }
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Driver time recording has been stopped successfully.');</script>";

            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('" + ex.Message + "');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }
    }
}
