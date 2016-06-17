using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParcelXpress.Enums;
using ParcelXpress.Models;
using PagedList;
using System.Text;
using System.Data;

namespace ParcelXpress.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        ParcelXpressConnection _db = new ParcelXpressConnection();

        public ActionResult CreateCustomerAccount(CUST_DATA customer = null,string accountRefNumber=null)
        {
            if (accountRefNumber != null)
            {
                ViewBag.HasAccount = true;
                ViewBag.AccountRefNumber = accountRefNumber;
            }
            return View(customer);
        }

        public ActionResult AllCustomers(string searchTerm = null,int page=1)
        {
            var model = _db.CUST_DATA
                .Where(r => (searchTerm == null || r.CustomerName.Contains(searchTerm)) && r.IsDeleted!=true)
                .OrderBy(r => r.CustomerName)
                .ToPagedList(page,15);

            ViewBag.searchTerm = searchTerm;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CustomersTable", model);
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveNewCustomer(CUST_DATA customer) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customer.AccountRefNumber != null)
                    {
                        var code = customer.AccountRefNumber.Split('C');
                        customer.AccountId = Convert.ToInt32(code[1]);
                    }
                    customer.IsDeleted = false;
                    _db.CUST_DATA.Add(customer);
                    _db.SaveChanges();

                    TempData["toastMessage"] = "<script>toastr.success('Customer has been successfully saved into the system.');</script>";
                    return RedirectToAction("AllCustomers");
                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Customer not saved due to invalid inputs.');</script>";
                }
            }

            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured, customer not saved.');</script>";
            }
            return RedirectToAction("CreateCustomerAccount", customer);
        }

        public ActionResult EditCustomer(int id, string accountRefNumber=null)
        {
            CUST_DATA customer=new CUST_DATA();
            if (id > 0)
            {
                 customer = _db.CUST_DATA.Find(id);
            }
            if (accountRefNumber != null)
            {
                ViewBag.HasAccount = true;
                ViewBag.AccountRefNumber = accountRefNumber;
            }
            else 
            {
                ViewBag.HasAccount = customer.HasAccount;
                ViewBag.AccountRefNumber = customer.AccountRefNumber;
            }
            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult saveEditCustomer(CUST_DATA customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customer.AccountRefNumber != null)
                    {
                        var code = customer.AccountRefNumber.Split('C');
                        customer.AccountId = Convert.ToInt32(code[1]);
                    }
                    customer.IsDeleted = false;
                    _db.Entry(customer).State = EntityState.Modified;
                    _db.SaveChanges();
                    TempData["toastMessage"] = "<script>toastr.success('Customer has been successfully updated in the system.');</script>";

                }
                else
                {
                    TempData["toastMessage"] = "<script>toastr.error('Customer not saved due to invalid inputs.');</script>";
                }
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('An error occured, customer not saved.');</script>";
            }
            return RedirectToAction("AllCustomers");
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int CustomerId)
        {
            try
            {
                var customer = _db.CUST_DATA.Find(CustomerId);
                customer.IsDeleted = true;
                _db.Entry(customer).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["toastMessage"] = "<script>toastr.success('Customer has been successfully deleted from the system.');</script>";
            }
            catch (Exception ex)
            {
                TempData["toastMessage"] = "<script>toastr.error('There was an error while processing your request.');</script>";
            }
            return RedirectToAction("AllCustomers");
        }

        public ActionResult generateAccountRef(string fromAction = "CreateCustomerAccount",int custId=0)
        {
            string accountCode = accountCodeGenerator();
            if (fromAction == "EditCustomer")
            {
                return RedirectToAction(fromAction, new { id=custId,accountRefNumber = accountCode });
            }
            return RedirectToAction(fromAction, new { accountRefNumber=accountCode });
        }


        private string accountCodeGenerator()
        {
            int? maxId = _db.CUST_DATA.Max(c => (int?)c.AccountId);
            if (maxId == null || maxId.Value > 99999)
                maxId = 1;
            else
                maxId += 1;
            StringBuilder builder = new StringBuilder();
            builder.Append("PXP-");
            builder.Append(DateTime.Now.ToString("yy-C"));

            if (maxId.Value <= 9)
                builder.Append("0000" + maxId.Value);
            else if (maxId.Value <= 99)
                builder.Append("000" + maxId.Value);
            else if (maxId.Value <= 999)
                builder.Append("00" + maxId.Value);
            else if (maxId.Value <= 9999)
                builder.Append("0" + maxId.Value);
            else if (maxId.Value <= 99999)
                builder.Append("" + maxId.Value);
            return builder.ToString();

        }


    }
}
