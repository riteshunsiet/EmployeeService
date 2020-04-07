using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeService.Models;
using EmployeeDataAccess;
using System.Net.Http;
using System.Threading;

namespace EmployeeService.Controllers
{
    public class EmployeMVCController : Controller
    {
        // GET: EmployeMVC
        public ActionResult Index()
        {
            // return View();
            IEnumerable<EmpModel> employee = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62997/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Employees");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<EmpModel>>();
                    readTask.Wait();

                    employee = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    employee = Enumerable.Empty<EmpModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(employee);

        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(EmpModel employee)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62997/api/Employees");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<EmpModel>("Employees", employee);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(employee);
        }

        public ActionResult Edit(int id)
        {
            EmpModel employee = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62997/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Employees?id=" + id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<EmpModel>();
                    readTask.Wait();

                    employee = readTask.Result;
                }
            }

            return View(employee);
        }

        [HttpPost]
        public ActionResult Edit(EmpModel employee)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62997/api/");
                
                //HTTP PUT
                var putTask =  client.PutAsJsonAsync<EmpModel>("Employees/" + employee.ID, employee);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View(employee);
        }

        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62997/api/");

                //HTTP DELETE
                    var deleteTask = client.DeleteAsync("Employees/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
    }
}