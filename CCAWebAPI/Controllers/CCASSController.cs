using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CCAWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CCAContollerSS : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CCAContollerSS(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Table")]
        public JsonResult Get()
        {
            string query = @"SELECT DISTINCT dbo.Details.Supplier_Name, dbo.Details.Plate_#, dbo.Details.Face_Label_Plate_#, dbo.Details.Back_Label_Plate_#, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, Sample_Name, Shared_Card, dbo.Details.Change, dbo.Details.Change_FL, dbo.Details.Output, dbo.Details.Output_FL 
FROM dbo.Sample 
INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("CCASS");
            SqlDataReader myReader;
            using (SqlConnection myCon = new(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("Job")]
        public JsonResult GetJob(string id)
        {
            string[] realId = id.Split(',');
            string query = $"SELECT * from dbo.Details where (Sample_ID='{realId[0]}' and (Art_Type_BL='{realId[1]}' OR Art_Type_FL='{realId[1]}'))";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("CCASS");
            SqlDataReader myReader;
            using (SqlConnection myCon = new(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("Full")]
        public JsonResult GetFull()
        {
            string query = @"SELECT * from dbo.Details";

            DataTable table = new();
            string sqlDataSource = _configuration.GetConnectionString("CCASS");
            SqlDataReader myReader;
            using (SqlConnection myCon = new(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }
    }
}
