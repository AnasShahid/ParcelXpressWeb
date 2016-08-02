using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Models;
using ParcelXpress.Enums;
using ParcelXpress.Helpers;

namespace ParcelXpress.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();

        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configuration()
        {
            var lastEmailAccount = _db.EMAL_ACNT.OrderByDescending(e => e.EmailAccountId).FirstOrDefault();
            EmailAccount model = new EmailAccount();
            if (lastEmailAccount != null)
            {
                model.EmailAddress = lastEmailAccount.EmailAddress;
                model.Password = lastEmailAccount.Password;
            }
            var items = from enumValue in Enum.GetValues(typeof(ParcelXpress.Enums.MailClients)).Cast<object>()
                        select new SelectListItem
                        {
                            Text = enumValue.ToString(),
                            Value = ((int)enumValue).ToString()
                        };
            ViewBag.MailClients = items;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveConfiguration(EmailAccount account)
        {
            try
            {
                EMAL_ACNT emailAccount = new EMAL_ACNT();
                emailAccount.EmailAddress = account.EmailAddress;
                emailAccount.Password = account.Password;
                emailAccount.EmailClient = account.MailProvider;
                emailAccount.ExecutionDate = DateTime.Now.ToUniversalTime();
                _db.EMAL_ACNT.Add(emailAccount);
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Configuration saved successfully.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Configuration could not be saved.');</script>";
            }
            return RedirectToAction("Dashboard", "Index");
        }

        public ActionResult ContractPackages()
        {
            var model = _db.CONT_PKGS.Where(p => p.IsActive == true).ToList();
            return View(model);
        }
        public ActionResult _PackageDetails()
        {
            CONT_PKGS model = new CONT_PKGS();
            ViewBag.FromEdit = false;
            ViewBag.FromAdd = false;
            return PartialView(model);
        }

        public ActionResult _AddPackage()
        {
            CONT_PKGS model = new CONT_PKGS();
            ViewBag.FromEdit = false;
            ViewBag.FromAdd = true;
            return PartialView("_PackageDetails", model);
        }
        public ActionResult _EditPackage(int PackageId = 0)
        {
            CONT_PKGS model = new CONT_PKGS();
            if (PackageId > 0)
            {
                model = _db.CONT_PKGS.Find(PackageId);
            }
            ViewBag.FromEdit = true;
            ViewBag.FromAdd = false;
            return PartialView("_PackageDetails", model);
        }
        [HttpPost]
        public ActionResult SavePackage(CONT_PKGS package)
        {
            try
            {
                if (package.PackageName == null || package.PackageName.Trim().Equals(""))
                    throw new Exception("Valid Package name is required");
                else if (package.Price <= 0)
                    throw new Exception("Price must be greater than 0");

                package.LastUpdated = DateTime.Now.ToUniversalTime();
                package.IsActive = true;
                package.IsDeleted = false;
                if (package.PackageId > 0)
                    _db.Entry(package).State = System.Data.EntityState.Modified;
                else
                    _db.CONT_PKGS.Add(package);

                _db.SaveChanges();

                TempData["toastMessage"] = "<script>toastr.success('Package has been saved successfully.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('" + ex.Message + "');</script>";
            }
            return RedirectToAction("ContractPackages");
        }

        [HttpPost]
        public ActionResult _DeletePackage(CONT_PKGS package)
        {
            try
            {
                if (package.PackageId > 0)
                {
                    var dbPackage = _db.CONT_PKGS.Find(package.PackageId);
                    dbPackage.IsActive = false;
                    dbPackage.IsDeleted = true;
                    _db.Entry(dbPackage).State = System.Data.EntityState.Modified;
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('Package has been deleted successfully.');</script>";
                }
                else {
                    TempData["toastMessage"] = "<script>toastr.warning('Please select a package to delete.');</script>";

                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('Package could not be deleted');</script>";
            }
            return RedirectToAction("ContractPackages");

        }
    }
}