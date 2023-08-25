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
            public string Program { get; set; }
            public string Change { get; set; }
        }
        public class Status
        {
            public string Status_Type { get; set; }
            public string Sample_ID { get; set; }
            public string Program { get; set; }
            public string New_Status { get; set; }
        }

        public class Update
        {
            public string Program { get; set; }
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
            string query = $"UPDATE dbo.Details set Change = '{cng.Change}' where (Sample_ID = '{cng.Sample_ID}' AND Program = '{cng.Program}')";

            SqlPut(query);

            string historySql = $"INSERT INTO dbo.History(Sample_ID, Program, Text, Type) VALUES('{cng.Sample_ID}', '{cng.Program}', '{cng.Change}', 'Automatic')";
            SqlPut(historySql);


            return new JsonResult("Updated Successfully");
        }

        [HttpPut("JobHS/Status")]
        public JsonResult PutStatus(Status stat)
        {
            string type = "Manual";
            string query = "";
            string historySql = "";
            if (stat.Status_Type.Equals("fl"))
            {
                query = $"UPDATE dbo.Details set Status_FL = '{stat.New_Status}' where (Sample_ID = '{stat.Sample_ID}' AND Program = '{stat.Program}')";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Type) VALUES('{stat.Sample_ID}', '{stat.Program}', 'FL: {stat.New_Status}', '{type}')";
            }
            if (stat.Status_Type.Equals("bl"))
            {
                query = $"UPDATE dbo.Details set Status = '{stat.New_Status}' where (Sample_ID = '{stat.Sample_ID}' AND Program = '{stat.Program}')";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Type) VALUES('{stat.Sample_ID}', '{stat.Program}', 'BL: {stat.New_Status}', '{type}')";
            }
            SqlPut(query);
            SqlPut(historySql);

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
            foreach (DataRow dr in currentInfo.Rows)
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
            string updateText = "Updating with new LAR Data";
            foreach (LarModels.Sample s in lARXlsSheet.SampleList)
            {
                string deleteSql = $"DELETE FROM dbo.Details WHERE (Sample_ID='{s.Sample_ID}')";
                SqlPut(deleteSql);
                deleteSql = $"DELETE FROM dbo.Sample WHERE (Sample_ID='{s.Sample_ID}')";
                SqlPut(deleteSql);
                deleteSql = $"DELETE FROM dbo.Labels WHERE (Sample_ID='{s.Sample_ID}')";
                SqlPut(deleteSql);
                //deleteSql = $"DELETE FROM dbo.Warranties WHERE (Sample_ID='{s.Sample_ID}' AND Program='{upd.Program}')";
                deleteSql = $"DELETE FROM dbo.Warranties WHERE (Sample_ID='{s.Sample_ID}')";
                SqlPut(deleteSql);
            }

            foreach (LarModels.Details d in lARXlsSheet.DetailsList)
            {
                string sql = "INSERT INTO dbo.Details (Sample_ID, Primary_Display, Division_List, Supplier_Name, " +
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
                        "" + d.ArtType_FL.Replace("'", "''") + "', '" + "Not Done" + "', '" +
                        "" + "Not Done" + "', '" + d.Change.Replace("'", "''") + "', '" +
                        "" + d.Change_FL.Replace("'", "''") + "', '" + upd.Program + "', '" +
                        "" + d.Output + "', '" + d.Output_FL + "', '" + d.Job_Number_BL + "', '" + d.Job_Number_FL + "')";
                SqlPut(sql);

                string historySql = $"INSERT INTO dbo.History(Sample_ID, Program, Text, Type) VALUES('{d.Sample_ID}', '{upd.Program}', '{updateText}', 'Automatic')";
                SqlPut(historySql);
            }

            foreach (LarModels.Sample s in lARXlsSheet.SampleList)
            {
                string sql = "INSERT INTO dbo.Sample " +
                           "(Sample_ID, Sample_Name, Sample_Size, " + "Sample_Type, " +
                           "Sampled_Color_SKU, Shared_Card, Sampled_With_Merch_Product_ID, Quick_Ship, Binder, " +
                           "Border, Character_Rating_by_Color, Feeler, MSRP, MSRP_Canada, " +
                           "Our_Price, Our_Price_Canada, RRP_US, Sampling_Color_Description, Split_Board, " +
                           "Trade_Up, Wood_Imaging, Sample_Note) " +
                           //"Trade_Up, Wood_Imaging, Sample_Note, Program) " +
                           "VALUES('" + s.Sample_ID + "', '" + s.Sample_Name.Replace("'", "''") + "', '" + s.Sample_Size + "', '" + s.Sample_Type + "', " +
                           "'" + s.Sampled_Color_SKU.Replace("'", "''") + "', '" + s.Shared_Card + "', '" + s.Sampled_With_Merch_Product_ID + "', '" + s.Quick_Ship + "', '" + s.Binder + "', " +
                           "'" + s.Border + "', '" + s.Character_Rating_by_Color + "', '" + s.Feeler.Replace("'", "''") + "', '" + s.MSRP + "', '" + s.MSRP_Canada + "', " +
                           "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board.Replace("'", "''") + "', " +
                           "'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note + "')"; 
                           //"'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note + "', '" + upd.Program + "')"; 
                SqlPut(sql);
            }

            foreach (LarModels.Labels l in lARXlsSheet.LabelList)
            {
                string sql = "INSERT INTO dbo.Labels (Merchandised_Product_ID, Sample_ID, Division_Label_Type, Division_Label_Name) VALUES ('" + l.Merchandised_Product_ID + "', '" + l.Sample_ID + "', '" + l.Division_Label_Type + "', '" + l.Division_Label_Name + "')";
                //string sql = "INSERT INTO dbo.Labels (Merchandised_Product_ID, Sample_ID, Division_Label_Type, Division_Label_Name, Program) VALUES ('" + l.Merchandised_Product_ID + "', '" + l.Sample_ID + "', '" + l.Division_Label_Type + "', '" + l.Division_Label_Name + "', '" + upd.Program + "')";
                SqlPut(sql);
            }

            foreach (LarModels.Warranties w in lARXlsSheet.WarrantiesList)
            {
                string sql = "INSERT INTO dbo.Warranties " +
                        "(Merchandised_Product_ID,Sample_ID,Provider,Duration,Warranty_Period,Product_Warranty_Type_Code) " +
                        //"(Merchandised_Product_ID,Sample_ID,Provider,Duration,Warranty_Period,Product_Warranty_Type_Code, Program) " +
                        "VALUES ('" + w.Merchandised_Product_ID + "', '" + w.Sample_ID + "', '" + w.Provider + "', " +
                        "'" + w.Duration + "', '" + w.Warranty_Period + "', '" + w.Product_Warranty_Type_Code + "'); ";
                        //"'" + w.Duration + "', '" + w.Warranty_Period + "', '" + w.Product_Warranty_Type_Code + "', '" + upd.Program + "'); ";

                SqlPut(sql);
            }

            foreach (LarModels.MktSpreadsheetItem m in mktSpreadsheetItemList)
            {
                string sql = $"UPDATE dbo.Details SET Change='{m.Change}', Change_FL='{m.Change_FL}', Status='{m.Status}', Status_FL='{m.Status_FL}' WHERE (Sample_ID='{m.Sample_ID}' AND Program='{m.Program}')";
                SqlPut(sql);
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpGet("TableHS")]
        public JsonResult GetTable()
        {
            //string query = @"SELECT DISTINCT dbo.Details.Face_Label_Plate, dbo.Details.Back_Label_Plate, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, dbo.Details.Program, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Details.Change, dbo.Details.Change_FL 
            string query = @"SELECT DISTINCT dbo.Details.Face_Label_Plate, dbo.Details.Back_Label_Plate, dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type_BL, dbo.Details.Art_Type_FL, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Details.Program, dbo.Details.Merchandised_Product_ID
FROM dbo.Sample 
INNER JOIN dbo.Details ON (dbo.Details.Sample_ID=dbo.Sample.Sample_ID)";
//INNER JOIN dbo.Details ON (dbo.Details.Sample_ID=dbo.Sample.Sample_ID AND dbo.Details.Program=dbo.Sample.Program)";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("JobHS/{id}")]
        public JsonResult GetJob(string id)
        {            
            string roomscenePath = @"\\MAG1PVSF4\Resources\Approved Roomscenes\CCA Automation 2.0";
            string[] realId = id.Split(',');
            List<string> styleList = new();
            List<string> mIds = new();
            bool doRoomsceneStuff = true;

            if (doRoomsceneStuff)
            {
                string supplierSQL = $"SELECT DISTINCT Supplier_Product_Name FROM dbo.Details WHERE (Sample_ID='{realId[0]}' AND Program='{realId[1]}')";
                DataTable supplierDT = GetDataTable(supplierSQL);
                foreach (DataRow dr in supplierDT.Rows)
                {
                    styleList.Add(dr["Supplier_Product_Name"].ToString());
                }

                int count = 0;
                string mIdSql = $"SELECT DISTINCT Merchandised_Product_Color_Id FROM dbo.Details WHERE (";
                foreach (string style in styleList)
                {
                    if (count.Equals(0))
                    {
                        mIdSql += $"(Supplier_Product_Name = '{style}'";
                        count++;
                    }
                    else
                    {
                        mIdSql += $" OR Supplier_Product_Name = '{style}'";
                    }
                }
                mIdSql += $") AND Program='{realId[1]}')";

                DataTable mIdDT = GetDataTable(mIdSql);
                foreach (DataRow dr in mIdDT.Rows)
                {
                    mIds.Add(dr["Merchandised_Product_Color_Id"].ToString());
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

                string insertRoomSql = $"UPDATE dbo.Details SET dbo.Details.Roomscene='{roomsceneName}' WHERE (dbo.Details.Sample_ID = '{realId[0]}' AND dbo.Details.Program='{realId[1]}')";
                
                SqlPut(insertRoomSql);
            }

            string query = $"SELECT dbo.Details.*, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, dbo.Sample.Sample_Note, dbo.Labels.Division_Label_Name from dbo.Details inner join dbo.Sample ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID inner join dbo.Labels ON dbo.Details.Sample_ID=dbo.Labels.Sample_ID where (dbo.Details.Sample_ID='{realId[0]}' and dbo.Details.Program='{realId[1]}' and dbo.Labels.Merchandised_Product_ID='{realId[2]}')";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("HistoryHS/{id}")]
        public JsonResult GetHistory(string id)
        {
            string[] realId = id.Split(',');
            
            string query = $"SELECT FORMAT (DateTime, 'yyyy-MM-dd HH:mm:ss') as DateTime, Text, Type FROM dbo.History WHERE (Sample_ID='{realId[0]}' and Program='{realId[1]}') ORDER BY DateTime ASC";

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
