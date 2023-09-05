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
    public class CCAContollerSS : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public CCAContollerSS(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPut("JobSS/Change")]
        public JsonResult PutChange(ControllerModels.Changes cng)
        {
            string query = $"UPDATE dbo.Details set Change = '{cng.Change}' where (Sample_ID = '{cng.Sample_ID}' AND Program = '{cng.Program}')";

            SqlPut(query);

            string historySql = $"INSERT INTO dbo.History(Sample_ID, Program, Text, Submitter) VALUES('{cng.Sample_ID}', '{cng.Program}', '{cng.Change}', 'Webpage')";
            SqlPut(historySql);


            return new JsonResult("Updated Successfully");
        }

        [HttpPut("JobSS/Status")]
        public JsonResult PutStatus(ControllerModels.Status stat)
        {
            string type = "Webpage";
            string query = "";
            string historySql = "";
            if (stat.Status_Type.Equals("fl"))
            {
                query = $"UPDATE dbo.Details set Status_FL = '{stat.New_Status}' where (Sample_ID = '{stat.Sample_ID}' AND Program = '{stat.Program}')";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Submitter) VALUES('{stat.Sample_ID}', '{stat.Program}', 'FL: {stat.New_Status}', '{type}')";
            }
            if (stat.Status_Type.Equals("bl"))
            {
                query = $"UPDATE dbo.Details set Status = '{stat.New_Status}' where (Sample_ID = '{stat.Sample_ID}' AND Program = '{stat.Program}')";
                historySql = $"INSERT INTO dbo.History (Sample_ID, Program, Text, Submitter) VALUES('{stat.Sample_ID}', '{stat.Program}', 'BL: {stat.New_Status}', '{type}')";
            }
            SqlPut(query);
            SqlPut(historySql);

            return new JsonResult("Updated Successfully");
        }

        [HttpPut("UpdateSS")]
        public JsonResult PutUpdate(ControllerModels.Update upd)
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
                deleteSql = $"DELETE FROM dbo.Warranties WHERE (Sample_ID='{s.Sample_ID}')";
                //deleteSql = $"DELETE FROM dbo.Warranties WHERE (Sample_ID='{s.Sample_ID}' AND Program='{upd.Program}')";
                SqlPut(deleteSql);
            }
            foreach (LarModels.Details d in lARXlsSheet.DetailsList)
            {
                string sql = "INSERT INTO dbo.Details (" +
                        "Sample_ID, Primary_Display, Division_List, " +
                        "Supplier_Name, Child_Supplier, Taxonomy, " +
                        "Supplier_Product_Name, Merchandised_Product_ID, Merch_Prod_Start_Date, " +
                        "Division_Product_Name, Division_Collection, Division_Rating, " +
                        "Product_Type, Product_Class, Is_Web_Product, Sample_Box_Enabled, " +
                        "Number_of_Colors, Made_In, Fiber_Company, Fiber_Brand, " +
                        "Merchandise_Brand, Primary_Fiber, Primary_Fiber_Percentage, " +
                        "Second_Fiber, Second_Fiber_Percentage, Third_Fiber, Third_Fiber_Percentage, " +
                        "Percent_BCF, Percent_Spun, Pile_Line, Stain_Treatment, " +
                        "Soil_Treatment, Dye_Method, Face_Weight, Yarn_Twist, " +
                        "Match, Match_Length, Match_Width, Total_Weight, " +
                        "Density, Gauge, Pile_Height, Stitches, Backing, " +
                        "IAQ_Number, Is_FHA_Certified, FHA_Type, FHA_Class, " +
                        "FHA_Lab, Durability_Rating, Flammability, " +
                        "Static_AATCC134, NBS_Smoke_Density_ASTME662, Radiant_Panel_ASTME648, " +
                        "Installation_Pattern, Commercial_Rating, Is_Green_Rated, " +
                        "Green_Natural_Sustained, Green_Recyclable_Content, Green_Recycled_Content, " +
                        "Size_Name, Manufacturer_Product_Color_ID, Mfg_Color_Name, " +
                        "Manufacturer_Feeler, Mfg_Color_Number, Sample_Box, Sample_Box_Availability, " +
                        "Manufacturer_SKU_Number, Merchandised_Product_Color_ID, Merch_Color_Start_Date, " +
                        "Merch_Color_Name, Merch_Color_Number, Merchandised_SKU_Number, " +
                        "CCASKUID, Art_Type_BL, Art_Type_FL, Back_Label_Plate, Face_Label_Plate, " +
                        "ADDNumber, Color_Sequence, Status, Status_FL, Change, Change_FL, Program, Output, Output_FL, Layout, Job_Number_BL, Job_Number_FL" +
                        ") VALUES (" +
                        "'" + d.Sample_ID + "', '" + d.Primary_Display.Replace("'", "''") + "', '" + d.Division_List.Replace("'", "''") + "', " +
                        "'" + d.Supplier_Name.Replace("'", "''") + "', '" + d.Child_Supplier.Replace("'", "''") + "', '" + d.Taxonomy.Replace("'", "''") + "', " +
                        "'" + d.Supplier_Product_Name.Replace("'", "''") + "', '" + d.Merchandised_Product_ID.Replace("'", "''") + "', '" + d.Merch_Prod_Start_Date.Replace("'", "''") + "', " +
                        "'" + d.Division_Product_Name.Replace("'", "''") + "', '" + d.Division_Collection.Replace("'", "''") + "', '" + d.Division_Rating.Replace("'", "''") + "', " +
                        "'" + d.Product_Type.Replace("'", "''") + "', '" + d.Product_Class.Replace("'", "''") + "', '" + d.Is_Web_Product.Replace("'", "''") + "', '" + d.Sample_Box_Enabled.Replace("'", "''") + "', " +
                        "'" + d.Number_of_Colors.Replace("'", "''") + "', '" + d.Made_In.Replace("'", "''") + "', '" + d.Fiber_Company.Replace("'", "''") + "', '" + d.Fiber_Brand.Replace("'", "''") + "', " +
                        "'" + d.Merchandise_Brand.Replace("'", "''") + "', '" + d.Primary_Fiber.Replace("'", "''") + "', '" + d.Primary_Fiber_Percentage.Replace("'", "''") + "', " +
                        "'" + d.Second_Fiber.Replace("'", "''") + "', '" + d.Second_Fiber_Percentage.Replace("'", "''") + "', '" + d.Third_Fiber.Replace("'", "''") + "', '" + d.Third_Fiber_Percentage.Replace("'", "''") + "', " +
                        "'" + d.Percent_BCF.Replace("'", "''") + "', '" + d.Percent_Spun.Replace("'", "''") + "', '" + d.Pile_Line.Replace("'", "''") + "', '" + d.Stain_Treatment.Replace("'", "''") + "', " +
                        "'" + d.Soil_Treatment.Replace("'", "''") + "', '" + d.Dye_Method.Replace("'", "''") + "', '" + d.Face_Weight.Replace("'", "''") + "', '" + d.Yarn_Twist.Replace("'", "''") + "', " +
                        "'" + d.Match.Replace("'", "''") + "', '" + d.Match_Length.Replace("'", "''") + "', '" + d.Match_Width.Replace("'", "''") + "', '" + d.Total_Weight.Replace("'", "''") + "', " +
                        "'" + d.Density.Replace("'", "''") + "', '" + d.Gauge.Replace("'", "''") + "', '" + d.Pile_Height.Replace("'", "''") + "', '" + d.Stitches.Replace("'", "''") + "', '" + d.Backing.Replace("'", "''") + "', " +
                        "'" + d.IAQ_Number.Replace("'", "''") + "', '" + d.Is_FHA_Certified.Replace("'", "''") + "', '" + d.FHA_Type.Replace("'", "''") + "', '" + d.FHA_Class.Replace("'", "''") + "', " +
                        "'" + d.FHA_Lab.Replace("'", "''") + "', '" + d.Durability_Rating.Replace("'", "''") + "', '" + d.Flammability.Replace("'", "''") + "', " +
                        "'" + d.Static_AATCC134.Replace("'", "''") + "', '" + d.NBS_Smoke_Density_ASTME662.Replace("'", "''") + "', '" + d.Radiant_Panel_ASTME648.Replace("'", "''") + "', " +
                        "'" + d.Installation_Pattern.Replace("'", "''") + "', '" + d.Commercial_Rating.Replace("'", "''") + "', '" + d.Is_Green_Rated.Replace("'", "''") + "', " +
                        "'" + d.Green_Natural_Sustained.Replace("'", "''") + "', '" + d.Green_Recyclable_Content.Replace("'", "''") + "', '" + d.Green_Recycled_Content.Replace("'", "''") + "', " +
                        "'" + d.Size_Name.Replace("'", "''") + "', '" + d.Manufacturer_Product_Color_ID.Replace("'", "''") + "', '" + d.Mfg_Color_Name.Replace("'", "''") + "', " +
                        "'" + d.Manufacturer_Feeler.Replace("'", "''") + "', '" + d.Mfg_Color_Number.Replace("'", "''") + "', '" + d.Sample_Box.Replace("'", "''") + "', '" + d.Sample_Box_Availability.Replace("'", "''") + "', " +
                        "'" + d.Manufacturer_SKU_Number.Replace("'", "''") + "', '" + d.Merchandised_Product_Color_ID.Replace("'", "''") + "', '" + d.Merch_Color_Start_Date.Replace("'", "''") + "', " +
                        "'" + d.Merch_Color_Name.Replace("'", "''") + "', '" + d.Merch_Color_Number.Replace("'", "''") + "', '" + d.Merchandised_SKU_Number.Replace("'", "''") + "', " +
                        "'" + d.CcaSkuId.Replace("'", "''") + "', '" + d.ArtType_BL.Replace("'", "''") + "', '" + d.ArtType_FL.Replace("'", "''") + "', '" + d.Plate_ID_BL.Replace("'", "''") + "', '" + d.Plate_ID_FL.Replace("'", "''") + "', " +
                        "'" + d.ADDNumber.Replace("'", "''") + "', '" + d.Color_Sequence.Replace("'", "''") + "', 'Not Done', 'Not Done', '" + d.Change.Replace("'", "''") + "', '" + d.Change_FL.Replace("'", "''") + "', '" + upd.Program + "', '" + d.Output + "', '" + d.Output_FL + "', '" + d.Layout.Replace("'", "''") + "', '" + d.Job_Number_BL.Replace("'", "''") + "', '" + d.Job_Number_FL.Replace("'", "''") + "')";
                SqlPut(sql);
            }

            foreach (LarModels.Sample s in lARXlsSheet.SampleList)
            {
                string sql = "INSERT INTO dbo.Sample " +
                            "(Sample_ID, Sample_Name, Sample_Size, " + "Sample_Type, " +
                            "Sampled_Color_SKU, Shared_Card, Multiple_Color_Lines, Sampled_With_Merch_Product_ID, Quick_Ship, Binder, " +
                            "Border, Character_Rating_by_Color, Feeler, MSRP, MSRP_Canada, " +
                            "Our_Price, Our_Price_Canada, RRP_US, Sampling_Color_Description, Split_Board, " +
                            "Trade_Up, Wood_Imaging, Sample_Note) " +
                            //"Trade_Up, Wood_Imaging, Sample_Note, Program) " +
                            "VALUES('" + s.Sample_ID + "', '" + s.Sample_Name.Replace("'", "''") + "', '" + s.Sample_Size + "', '" + s.Sample_Type + "', " +
                            "'" + s.Sampled_Color_SKU.Replace("'", "''") + "', '" + s.Shared_Card + "', '" + s.Multiple_Color_Lines + "', '" + s.Sampled_With_Merch_Product_ID + "', '" + s.Quick_Ship + "', '" + s.Binder + "', " +
                            "'" + s.Border + "', '" + s.Character_Rating_by_Color + "', '" + s.Feeler.Replace("'", "''") + "', '" + s.MSRP + "', '" + s.MSRP_Canada + "', " +
                            "'" + s.Our_Price + "', '" + s.Our_Price_Canada + "', '" + s.RRP_US + "', '" + s.Sampling_Color_Description + "', '" + s.Split_Board.Replace("'", "''") + "', " +
                            "'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note.Replace("'", "''") + "')";
                //"'" + s.Trade_Up + "', '" + s.Wood_Imaging + "', '" + s.Sample_Note.Replace("'", "''") + "', '" + upd.Program + "')";
                SqlPut(sql);

                string historySql = $"INSERT INTO dbo.History(Sample_ID, Program, Text, Submitter) VALUES('{s.Sample_ID}', '{upd.Program}', '{updateText}', 'Webpage')";
                SqlPut(historySql);
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

        [HttpGet("TableSS")]
        public JsonResult GetTable()
        {
            string query = @"SELECT DISTINCT dbo.Details.Supplier_Name, dbo.Details.Face_Label_Plate, dbo.Details.Back_Label_Plate, 
                            dbo.Details.Sample_ID, dbo.Details.Status, dbo.Details.Status_FL, dbo.Details.Art_Type_BL, 
                            dbo.Details.Art_Type_FL, dbo.Details.Program, dbo.Sample.Sample_Name, dbo.Sample.Feeler, 
                            dbo.Sample.Shared_Card, dbo.Details.Change, dbo.Details.Change_FL FROM dbo.Sample 
                            INNER JOIN dbo.Details ON (dbo.Details.Sample_ID=dbo.Sample.Sample_ID)";
            //INNER JOIN dbo.Details ON (dbo.Details.Sample_ID='dbo.Sample.Sample_ID' AND dbo.Details.Program='dbo.Sample.Program')";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("JobSS/{program}/{id}/{mId}")]
        public JsonResult GetJob(string program, string id, string mId)
        {
            string roomscenePath = @"\\MAG1PVSF4\Resources\Approved Roomscenes\CCA Automation 2.0";
            List<string> styleList = new();
            List<string> mIds = new();
            bool doRoomsceneStuff = true;

            if (doRoomsceneStuff)
            {
                string supplierSQL = $"SELECT DISTINCT Supplier_Product_Name FROM dbo.Details " +
                                        $"WHERE (Sample_ID='{id}' AND Program='{program}')";
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
                mIdSql += $") AND Program='{program}')";

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

                string insertRoomSql = $"UPDATE dbo.Details SET dbo.Details.Roomscene='{roomsceneName}' " +
                                        $"WHERE (dbo.Details.Sample_ID = '{id}' AND dbo.Details.Program='{program}')";

                SqlPut(insertRoomSql);
            }

            string query = $"SELECT dbo.Details.*, dbo.Sample.Sample_Name, dbo.Sample.Feeler, dbo.Sample.Shared_Card, " +
                            $"dbo.Sample.Sample_Note, dbo.Sample.Split_Board, dbo.Labels.Division_Label_Name " +
                            $"from dbo.Details inner join dbo.Sample ON dbo.Details.Sample_ID=dbo.Sample.Sample_ID " +
                            $"inner join dbo.Labels ON dbo.Details.Sample_ID=dbo.Labels.Sample_ID " +
                            $"where (dbo.Details.Sample_ID='{id}' and dbo.Details.Program='{program}')";

            DataTable table = GetDataTable(query);

            return new JsonResult(table);
        }

        [HttpGet("HistorySS/{program}/{id}/{mId}")]
        public JsonResult GetHistory(string program, string id, string mId)
        {
            string query = $"SELECT FORMAT (DateTime, 'yyyy-MM-dd HH:mm:ss') as DateTime, Text, Submitter " +
                            $"FROM dbo.History WHERE (Sample_ID='{id}' and Program='{program}') ORDER BY DateTime ASC";

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
