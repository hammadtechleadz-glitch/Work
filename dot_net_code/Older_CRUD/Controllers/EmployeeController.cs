using Older_CRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Older_CRUD.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            List<Employee> list = Employee.GetAllEmployees();
            return View(list);
        }
        //Get
        public ActionResult Create()
        {
            return View();
        }
        //Post
        [HttpPost]
        public ActionResult Create(Employee emp)
        {
            if (!ModelState.IsValid) return View(emp);

            try
            {
                if (Employee.Create(emp))
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Unable to save employee data.");
                return View(emp);
            }
            catch (Exception ex)
            {
                // Pulls the exact custom text we threw above and displays it in @Html.ValidationSummary
                ModelState.AddModelError("", ex.Message);
                return View(emp);
            }
        }
    }
}