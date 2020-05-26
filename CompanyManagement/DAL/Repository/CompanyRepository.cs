using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Configuration;
using CompanyManagement.Models;
using Dapper;

namespace CompanyManagement.DAL.Repository
{
    public class CompanyRepository
    {
        private readonly IDbConnection _connection;

        public CompanyRepository()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            _connection = factory.CreateConnection();
            _connection.ConnectionString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            _connection.Open();
        }

        public List<Company> Get()
        {
            return _connection.Query<Company>("spCompanyList", 
                commandType: CommandType.StoredProcedure).ToList();
        }

        public Company GetById(int id)
        {
            return _connection.Query<Company>("spSelectCompany", new { companyid = id }, 
                commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public int Create(Company company, int id)
        {
            var result = _connection.ExecuteScalar("spCreateCompany", new
            {
                company.Name,
                NoE = company.NumberofEmployee,
                company.Address,
                company.Telephone,
                company.EstablishmentDay,
                createbyuser = id,
                updatebyuser = id
            }, commandType: CommandType.StoredProcedure);

            return int.Parse(result.ToString());
        }

        public void Edit(Company company, int id)
        {
            _connection.Execute("spEditCompany", new { 
                company.CompanyID,
                company.Name,
                company.NumberofEmployee,
                company.Address, 
                company.Telephone,
                company.EstablishmentDay,
                updatebyuser = id}, 
                commandType: CommandType.StoredProcedure);
        }
        
        public void Delete(int comid, int adid)
        {
            _connection.Execute("spDeleteCompany", new { companyid = comid, userid = adid}, 
                commandType: CommandType.StoredProcedure);
        }
    }
}