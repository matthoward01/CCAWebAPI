﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CCAWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CCAContollerSS : ControllerBase
    {
        public class ChangesSS
        {
            public string Sample_ID { get; set; }
            public string Program { get; set; }
            public string Change { get; set; }
        }
        public class StatusSS
        {
            public string Status_Type { get; set; }
            public string Sample_ID { get; set; }
            public string Program { get; set; }
            public string New_Status { get; set; }
        }

        private readonly IConfiguration _configuration;

        public CCAContollerSS(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        

        [HttpPut("JobSS/Change")]
        public JsonResult PutChange(ChangesSS cng)
        {
            string query = $"UPDATE dbo.Details set Change = '{cng.Change}' where Sample_ID = '{cng.Sample_ID}'";

            SqlPut(query);

            string historySql = $"INSERT INTO dbo.History(Sample_ID, Program, Text, Type) VALUES('{cng.Sample_ID}', '{cng.Program}', '{cng.Change}', 'Automatic')";
            SqlPut(historySql);


            return new JsonResult("Updated Successfully");
        }

        [HttpPut("JobSS/Status")]
        public JsonResult PutStatus(StatusSS stat)
        {
            string type = "Manual";
            string query = "";
            string historySql = "";
            if (stat.Status_Type.Equals("fl"))
            {
                query = $"UPDATE dbo.Details set Status_FL = '{stat.New_Status}' where Sample_ID = '{stat.Sample_ID}'";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Type) VALUES('{stat.Sample_ID}', '{stat.Program}', 'FL: {stat.New_Status}', '{type}')";
            }
            if (stat.Status_Type.Equals("bl"))
            {
                query = $"UPDATE dbo.Details set Status = '{stat.New_Status}' where Sample_ID = '{stat.Sample_ID}'";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Type) VALUES('{stat.Sample_ID}', '{stat.Program}', 'BL: {stat.New_Status}', '{type}')";
            }
            SqlPut(query);
            SqlPut(historySql);

            return new JsonResult("Updated Successfully");
        }

        [HttpGet("TableSS")]
        public JsonResult GetTable()
        {
            string query = @"SELECT DISTINCT dbo.Details.Supplier_Name, dbo.Details.Face_Label_Plate, dbo.Details.Back_Label_Plate, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, dbo.Details.Program, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Details.Change, dbo.Details.Change_FL, dbo.Details.Output, dbo.Details.Output_FL 
FROM dbo.Sample 
INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("JobSS/{id}")]
        public JsonResult GetJob(string id)
        {
            string roomscenePath = @"\\MAG1PVSF4\Resources\Approved Roomscenes\CCA Automation 2.0";
            string[] realId = id.Split(',');
            List<string> styleList = new();
            string sqlDataSource = _configuration.GetConnectionString("CCASS");
            List<string> mIds = new();
            bool doRoomsceneStuff = true;
            SqlCommand command;
            SqlDataReader dataReader;

            if (doRoomsceneStuff)
            {
                //TODO: Redo to use DataTable
                string sqlSuppliers = $"SELECT DISTINCT Supplier_Product_Name FROM dbo.Details WHERE (Sample_ID='{realId[0]}' AND Program='{realId[1]}')";
                using (SqlConnection myCon = new(sqlDataSource))
                {
                    myCon.Open();
                    command = new SqlCommand(sqlSuppliers, myCon);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        styleList.Add(dataReader.GetString(dataReader.GetOrdinal("Supplier_Product_Name")));
                    }

                    dataReader.Close();
                    command.Dispose();
                    myCon.Close();
                }
                int count = 0;
                string sql = $"SELECT DISTINCT Merchandised_Product_Color_Id FROM dbo.Details WHERE (";
                foreach (string style in styleList)
                {
                    if (count.Equals(0))
                    {
                        sql += $"(Supplier_Product_Name = '{style}'";
                        count++;
                    }
                    else
                    {
                        sql += $" OR Supplier_Product_Name = '{style}'";
                    }
                } 
                sql += $") and Manufacturer_Feeler='yes' AND Program='{realId[1]}')";
                //TODO: Redo to use DataTable
                using (SqlConnection myCon = new(sqlDataSource))
                {
                    myCon.Open();
                    command = new SqlCommand(sql, myCon);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        mIds.Add(dataReader.GetString(dataReader.GetOrdinal("Merchandised_Product_Color_Id")));
                    }

                    dataReader.Close();
                    command.Dispose();
                    myCon.Close();
                }
                string roomsceneName = "";
                List<string> roomsceneNames = Directory.GetFiles(roomscenePath, "*.tif", SearchOption.AllDirectories).ToList();
                int index = -1;
                foreach (string s in mIds)
                {
                    index = roomsceneNames.FindIndex(x => Path.GetFileName(x).StartsWith(s));
                    if (index != -1)
                        break;
                }
                if (index != -1)
                {
                    roomsceneName = Path.GetFileName(roomsceneNames[index]);
                }

                string insertRoomSql = $"UPDATE dbo.Details SET dbo.Details.Roomscene='{roomsceneName}' WHERE (dbo.Details.Sample_ID = '{realId[0]}' AND dno.Details.Program='{realId[1]}')";
                
                SqlPut(insertRoomSql);
            }

            string query = $"SELECT dbo.Details.*, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Sample.Sample_Note, dbo.Sample.Split_Board, dbo.Labels.Division_Label_Name from dbo.Details inner join dbo.Sample ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID inner join dbo.Labels ON dbo.Details.Sample_ID=dbo.Labels.Sample_ID where (dbo.Details.Sample_ID='{realId[0]}' and dbo.Details.Program='{realId[1]}')";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("HistorySS/{id}")]
        public JsonResult GetHistory(string id)
        {
            string[] realId = id.Split(',');

            string query = $"SELECT FORMAT (DateTime, 'yyyy-MM-dd HH:mm:ss') as DateTime, Text, Type FROM dbo.History WHERE (Sample_ID='{realId[0]}' and Program='{realId[1]}') ORDER BY DateTime ASC";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("ProgramsSS")]
        public JsonResult GetPrograms()
        {
            //string query = @"SELECT DISTINCT Program from dbo.Details";
            string query = @"SELECT DISTINCT Sample_ID, Program, Status, Status_FL from dbo.Details ORDER BY Program ASC";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        private DataTable GetDataTable(string query)
        {
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

            return table;
        }

        private void SqlPut(string query)
        {
            string sqlDataSource = _configuration.GetConnectionString("CCASS");
            SqlDataReader myReader;
            using (SqlConnection myCon = new(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                    myCon.Close();
                }
            }
        }
    }
}
