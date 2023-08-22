using Microsoft.AspNetCore.Http;
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

    public class CCAContoller : ControllerBase
    {
        public class Changes
        {
            public string Sample_ID { get; set; }
            public string Change { get; set; }
        }
        public class Status
        {
            public string Status_Type { get; set; }
            public string Sample_ID { get; set; }
            public string New_Status { get; set; }
        }

        public class Update
        {
            public string XlsFileName { get; set; }
        }


        private readonly IConfiguration _configuration;

        public CCAContoller(IConfiguration configuration)
        {
            _configuration = configuration;
        }        

        [HttpPut("JobHS/Change")]
        public JsonResult PutChange(Changes cng)
        {
            string query = $"UPDATE dbo.Details set Change = '{cng.Change}' where Sample_ID = '{cng.Sample_ID}'";
            SqlPut(query);

            return new JsonResult("Updated Successfully");
        }        

        [HttpPut("JobHS/Status")]
        public JsonResult PutStatus(Status stat)
        {
            string query = "";
            if (stat.Status_Type.Equals("fl"))
            {
                query = $"UPDATE dbo.Details set Status_FL = '{stat.New_Status}' where Sample_ID = '{stat.Sample_ID}'";
            }
            if (stat.Status_Type.Equals("bl"))
            {
                query = $"UPDATE dbo.Details set Status = '{stat.New_Status}' where Sample_ID = '{stat.Sample_ID}'";
            }

            SqlPut(query);

            return new JsonResult("Updated Successfully");
        }

        [HttpPut("UpdateHS")]
        public JsonResult PutUpdate(Update upd)
        {
            List<LarModels.MktSpreadsheetItem> mktSpreadsheetItemList = new();
            string fileName = upd.XlsFileName.Replace("\"", "");
            LarModels.LARXlsSheet lARXlsSheet = Lar.GetLar(fileName);
            string currentInfoQuery = "SELECT Change, Change_FL, Program, Sample_ID, Status, Status_FL FROM dbo.Details";
            DataTable currentInfo = GetDataTable(currentInfoQuery);
            foreach(DataRow dr in currentInfo.Rows)
            {
                LarModels.MktSpreadsheetItem mktSpreadsheetItem = new();
                mktSpreadsheetItem.Change = dr["Change"].ToString();
                mktSpreadsheetItem.Change_FL = dr["Change_FL"].ToString();
                mktSpreadsheetItem.Program = dr["Program"].ToString();
                mktSpreadsheetItem.Sample_ID = dr["Sample_ID"].ToString();
                mktSpreadsheetItem.Status = dr["Status"].ToString();
                mktSpreadsheetItem.Status_FL = dr["Status_FL"].ToString();
                mktSpreadsheetItemList.Add(mktSpreadsheetItem);
            }

            foreach (LarModels.Details d in lARXlsSheet.DetailsList)
            {
                string query = "INSERT INTO dbo.Details (Sample_ID, Primary_Display, Division_List, Supplier_Name, " +
                    "Child_Supplier, Taxonomy, Supplier_Product_Name, Merchandised_Product_ID, " +
                    "Merch_Prod_Start_Date, Division_Product_Name, Web_Product_Name, Division_Collection, " +
                    "Division_Rating, Product_Type, Product_Class, Is_Web_Product, Sample_Box_Enabled, " +
                    "Number_of_Colors, Made_In, Appearance, Backing, Edge_Profile, End_Profile, FHA_Class, " +
                    "FHA_Lab, FHA_Type, Finish, Glazed_Hardness, Grade, Is_FHA_Certified, Is_Recommended_Outdoors, " +
                    "Is_Wall_Tile, Locking_Type, Radiant_Heat, Shade_Variation, Stain_Treatment, Wear_Layer, " +
                    "Wear_Layer_Type, Construction, Gloss_Level, Hardness_Rating, Installation_Method, Match, " +
                    "Match_Length, Match_Width, Species, Merchandise_Brand, Commercial_Rating, Is_Green_Rated, " +
                    "Green_Natural_Sustained, Green_Recyclable_Content, Green_Recycled_Content, Size_Name, Length, " +
                    "Length_Measurement, Width, Width_Measurement, Thickness, Thickness_Measurement, " +
                    "Thickness_Fraction, Manufacturer_Product_Color_ID, Mfg_Color_Name, Mfg_Color_Number, " +
                    "Sample_Box, Sample_Box_Availability, Manufacturer_SKU_Number, Merchandised_Product_Color_ID, " +
                    "Merch_Color_Start_Date, Merch_Color_Name, Merch_Color_Number, Merchandised_SKU_Number, Barcode, " +
                    "CCASKUID, Size_UC, Roomscene, Back_Label_Plate, Face_Label_Plate, " +
                    "Art_Type_BL, Art_Type_FL, Status, Status_FL, Change, Change_FL, Program, Output, Output_FL, Job_Number_BL, Job_Number_FL) " +
                    "VALUES " +
                    "('" + d.Sample_ID.Replace("'", "''") + "', '" + d.Primary_Display.Replace("'", "''") + "', '" +
                    "" + d.Division_List.Replace("'", "''") + "', '" + d.Supplier_Name.Replace("'", "''") + "', '" +
                    "" + d.Child_Supplier.Replace("'", "''") + "', '" + d.Taxonomy.Replace("'", "''") + "', '" +
                    "" + d.Supplier_Product_Name.Replace("'", "''") + "', '" +
                    "" + d.Merchandised_Product_ID.Replace("'", "''") + "', '" +
                    "" + d.Merch_Prod_Start_Date.Replace("'", "''") + "', '" +
                    "" + d.Division_Product_Name.Replace("'", "''") + "', '" +
                    "" + d.Web_Product_Name.Replace("'", "''") + "', '" + d.Division_Collection.Replace("'", "''") + "', '" +
                    "" + d.Division_Rating.Replace("'", "''") + "', '" + d.Product_Type.Replace("'", "''") + "', '" +
                    "" + d.Product_Class.Replace("'", "''") + "', '" + d.Is_Web_Product.Replace("'", "''") + "', '" +
                    "" + d.Sample_Box_Enabled.Replace("'", "''") + "', '" + d.Number_of_Colors.Replace("'", "''") + "', '" +
                    "" + d.Made_In.Replace("'", "''") + "', '" + d.Appearance.Replace("'", "''") + "', '" +
                    "" + d.Backing.Replace("'", "''") + "', '" + d.Edge_Profile.Replace("'", "''") + "', '" +
                    "" + d.End_Profile.Replace("'", "''") + "', '" + d.FHA_Class.Replace("'", "''") + "', '" +
                    "" + d.FHA_Lab.Replace("'", "''") + "', '" + d.FHA_Type.Replace("'", "''") + "', '" +
                    "" + d.Finish.Replace("'", "''") + "', '" + d.Glazed_Hardness.Replace("'", "''") + "', '" +
                    "" + d.Grade.Replace("'", "''") + "', '" + d.Is_FHA_Certified.Replace("'", "''") + "', '" +
                    "" + d.Is_Recommended_Outdoors.Replace("'", "''") + "', '" + d.Is_Wall_Tile.Replace("'", "''") + "', '" +
                    "" + d.Locking_Type.Replace("'", "''") + "', '" + d.Radiant_Heat.Replace("'", "''") + "', '" +
                    "" + d.Shade_Variation.Replace("'", "''") + "', '" + d.Stain_Treatment.Replace("'", "''") + "', '" +
                    "" + d.Wear_Layer.Replace("'", "''") + "', '" + d.Wear_Layer_Type.Replace("'", "''") + "', '" +
                    "" + d.Construction.Replace("'", "''") + "', '" + d.Gloss_Level.Replace("'", "''") + "', '" +
                    "" + d.Hardness_Rating.Replace("'", "''") + "', '" + d.Installation_Method.Replace("'", "''") + "', '" +
                    "" + d.Match.Replace("'", "''") + "', '" + d.Match_Length.Replace("'", "''") + "', '" +
                    "" + d.Match_Width.Replace("'", "''") + "', '" + d.Species.Replace("'", "''") + "', '" +
                    "" + d.Merchandise_Brand.Replace("'", "''") + "', '" + d.Commercial_Rating.Replace("'", "''") + "', '" +
                    "" + d.Is_Green_Rated.Replace("'", "''") + "', '" +
                    "" + d.Green_Natural_Sustained.Replace("'", "''") + "', '" +
                    "" + d.Green_Recyclable_Content.Replace("'", "''") + "', '" +
                    "" + d.Green_Recycled_Content.Replace("'", "''") + "', '" + d.Size_Name.Replace("'", "''") + "', '" +
                    "" + d.Length.Replace("'", "''") + "', '" + d.Length_Measurement.Replace("'", "''") + "', '" +
                    "" + d.Width.Replace("'", "''") + "', '" + d.Width_Measurement.Replace("'", "''") + "', '" +
                    "" + d.Thickness.Replace("'", "''") + "', '" + d.Thickness_Measurement.Replace("'", "''") + "', '" +
                    "" + d.Thickness_Fraction.Replace("'", "''") + "', '" +
                    "" + d.Manufacturer_Product_Color_ID.Replace("'", "''") + "', '" +
                    "" + d.Mfg_Color_Name.Replace("'", "''") + "', '" + d.Mfg_Color_Number.Replace("'", "''") + "', '" +
                    "" + d.Sample_Box.Replace("'", "''") + "', '" + d.Sample_Box_Availability.Replace("'", "''") + "', '" +
                    "" + d.Manufacturer_SKU_Number.Replace("'", "''") + "', '" +
                    "" + d.Merchandised_Product_Color_ID.Replace("'", "''") + "', '" +
                    "" + d.Merch_Color_Start_Date.Replace("'", "''") + "', '" +
                    "" + d.Merch_Color_Name.Replace("'", "''") + "', '" + d.Merch_Color_Number.Replace("'", "''") + "', '" +
                    "" + d.Merchandised_SKU_Number.Replace("'", "''") + "', '" + d.Barcode.Replace("'", "''") + "', '" +
                    "" + d.CcaSkuId.Replace("'", "''") + "', '" + d.Size_UC.Replace("'", "''") + "', '" +
                    "" + d.Roomscene.Replace("'", "''") + "', '" +
                    "" + d.Plate_ID_BL.Replace("'", "''") + "', '" + d.Plate_ID_FL.Replace("'", "''") + "', '" +
                    "" + d.ArtType_BL.Replace("'", "''") + "', '" +
                    "" + d.ArtType_FL.Replace("'", "''") + "', '" + d.Status.Replace("'", "''") + "', '" +
                    "" + d.Status_FL.Replace("'", "''") + "', '" + d.Change.Replace("'", "''") + "', '" +
                    "" + d.Change_FL.Replace("'", "''") + "', '" + d.Program.Replace("'", "''") + "', '" +
                    "" + d.Output + "', '" + d.Output_FL + "', '" + d.Job_Number_BL + "', '" + d.Job_Number_FL + "')";
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpGet("TableHS")]
        public JsonResult GetTable()
        {
            string query = @"SELECT DISTINCT dbo.Details.Face_Label_Plate, dbo.Details.Back_Label_Plate, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, dbo.Details.Program, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Details.Change, dbo.Details.Change_FL, dbo.Details.Output, dbo.Details.Output_FL 
FROM dbo.Sample 
INNER JOIN dbo.Details ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("JobHS/{id}")]
        public JsonResult GetJob(string id)
        {
            string roomscenePath = @"\\MAG1PVSF4\Resources\Approved Roomscenes\CCA Automation 2.0";
            string[] realId = id.Split(',');
            List<string> styleList = new();
            string sqlDataSource = _configuration.GetConnectionString("CCA");
            List<string> mIds = new();
            bool doRoomsceneStuff = true;
            SqlCommand command;
            SqlDataReader dataReader;

            if (doRoomsceneStuff)
            {

                string sql1 = $"SELECT DISTINCT Supplier_Product_Name FROM dbo.Details WHERE (Sample_ID='{realId[0]}' AND Program='{realId[1]}')";
                using (SqlConnection myCon = new(sqlDataSource))
                {
                    myCon.Open();
                    command = new SqlCommand(sql1, myCon);
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
                sql += $") AND Program='{realId[1]}')";
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
                string insertRoomSql = $"UPDATE dbo.Details SET dbo.Details.Roomscene='{roomsceneName}' WHERE (dbo.Details.Sample_ID = '{realId[0]}' AND Program='{realId[1]}')";
                using (SqlConnection myCon = new(sqlDataSource))
                {
                    myCon.Open();
                    command = new SqlCommand(insertRoomSql, myCon);
                    dataReader = command.ExecuteReader();
                    dataReader.Close();
                    command.Dispose();
                    myCon.Close();
                }
            }

            string query = $"SELECT dbo.Details.*, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Sample.Sample_Note, dbo.Labels.Division_Label_Name from dbo.Details inner join dbo.Sample ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID inner join dbo.Labels ON dbo.Details.Sample_ID=dbo.Labels.Sample_ID where (dbo.Details.Sample_ID='{realId[0]}' and Program='{realId[1]}')";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("ProgramsHS")]
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
            string sqlDataSource = _configuration.GetConnectionString("CCA");
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
            string sqlDataSource = _configuration.GetConnectionString("CCA");
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
