using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using CompanyManagement.Models;
using System.Configuration;
using Dapper;
using System.Data;
using PagedList;
using CompanyManagement.DAL.Repository;
using CompanyManagement.Areas.Admin.Models;

namespace CompanyManagement.Controllers
{
    public class ManagementController : Controller
    {
        private CompanyRepository _companyRepository;
        public ActionResult Index(string sortOrder, string str, int? page)
        {
            _companyRepository = new CompanyRepository();
            List<Company> companies = _companyRepository.Get();
            var coms = companies.Where(c => true);
            if (str != null && str != "") coms = companies.Where(c => c.Name.ToLowerInvariant().Contains(str.ToLowerInvariant()));
            ViewBag.SearchStr = str;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SortName = sortOrder == "name" ? "desc_name" : "name";
            ViewBag.SortNoE = sortOrder == "desc_noe" ? "noe" : "desc_noe";
            ViewBag.SortAddress = sortOrder == "add" ? "desc_add" : "add";
            ViewBag.SortPhone = sortOrder == "Phone" ? "desc_Phone" : "Phone";
            ViewBag.SortEd = sortOrder == "Ed" ? "desc_Ed" : "Ed";

            switch (sortOrder)
            {
                case "name":
                    coms = coms.OrderBy(c => c.Name);
                    break;
                case "desc_name":
                    coms = coms.OrderByDescending(c => c.Name);
                    break;
                case "desc_noe":
                    coms = coms.OrderByDescending(c => c.NumberofEmployee);
                    break;
                case "noe":
                    coms = coms.OrderBy(c => c.NumberofEmployee);
                    break;
                case "desc_add":
                    coms = coms.OrderByDescending(c => c.Address);
                    break;
                case "add":
                    coms = coms.OrderBy(c => c.Address);
                    break;
                case "Phone":
                    coms = coms.OrderBy(c => c.Telephone);
                    break;
                case "desc_Phone":
                    coms = coms.OrderByDescending(c => c.Telephone);
                    break;
                case "desc_Ed":
                    coms = coms.OrderByDescending(c => c.EstablishmentDay);
                    break;
                case "Ed":
                    coms = coms.OrderBy(c => c.EstablishmentDay);
                    break;
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;

            return View(coms.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Index1(int? page) 
        {
            int pageNum = page ?? 1;
            int numRow = 5;
            var db = new SqlConnection(ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);
            var noOfRecord = int.Parse(db.ExecuteScalar("select count(*) from company").ToString());
            ViewBag.NoOfRecord = noOfRecord;
            ViewBag.CurrentPage = pageNum;
            ViewBag.NumRow = numRow;

            List<Company> coms = db.Query<Company>("spPageList", new { offset = (pageNum - 1) * numRow, numRow }, 
                commandType: CommandType.StoredProcedure).ToList();
            return View(coms);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (Session["admin"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (company != null)
                    {
                        var ad = Session["admin"] as Account;
                        _companyRepository = new CompanyRepository();
                        var re = _companyRepository.Create(company, ad.UserId);
                        return Redirect(Url.Action("Details", new { id = int.Parse(re.ToString()) }));
                    }
                }
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            _companyRepository = new CompanyRepository();
            Company company = _companyRepository.GetById(id);
            return View(company);
        }

        public ActionResult Edit(int id)
        {
            if (Session["admin"] != null)
            {
                _companyRepository = new CompanyRepository();
                Company company = _companyRepository.GetById(id);
                return View(company);
            }
            else
            {
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company != null)
                {
                    var ad = Session["admin"] as Account;
                    _companyRepository = new CompanyRepository();
                    _companyRepository.Edit(company, ad.UserId);
                    return Redirect(Url.Action("Details", new { id = company.CompanyID }));
                }
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            if (Session["admin"] != null)
            {
                _companyRepository = new CompanyRepository();
                Company company = _companyRepository.GetById(id);
                return View(company);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var ad = Session["admin"] as Account;
            _companyRepository = new CompanyRepository();
            _companyRepository.Delete(id, ad.UserId);
            return Redirect(Url.Action("Index"));
        }

        
    }
}