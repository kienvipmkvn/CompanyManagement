using CompanyManagement.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace CompanyManagement.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account account)
        {
            if (ModelState.IsValid)
            {
                var db = new SqlConnection(ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);
                var res = db.ExecuteScalar<int>("spLogin", new 
                {
                    user = account.Username,
                    pass = account.Password
                }, commandType: System.Data.CommandType.StoredProcedure) ;
                if (res != 0)
                {
                    account.UserId = res;
                    Session["admin"] = account;
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            return View(account);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return Redirect(Url.Action("Login"));
        }
        //public ActionResult ViewPage1()
        //{
        //    return View();
        //}
        //public ActionResult View2()
        //{
        //    return View();
        //}
    }
}