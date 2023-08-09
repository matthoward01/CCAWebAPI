using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CCAWebAPI.Models.CCASS
{
    public partial class CCASSContext : DbContext
    {
        public CCASSContext()
        {
        }

        public CCASSContext(DbContextOptions<CCASSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Detail> Details { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }
        public virtual DbSet<Warranty> Warranties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Detail>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.SampleId, "ClusteredIndex-20230126-130852")
                    .IsClustered();

                entity.Property(e => e.Addnumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("ADDNumber");

                entity.Property(e => e.ArtType)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Art_Type");

                entity.Property(e => e.ArtTypeBl)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Art_Type_BL");

                entity.Property(e => e.ArtTypeFl)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Art_Type_FL");

                entity.Property(e => e.BackLabelPlate)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Back_Label_Plate_#");

                entity.Property(e => e.Backing)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.Ccaskuid)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("CCASKUID");

                entity.Property(e => e.Change)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.ChangeFl)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Change_FL");

                entity.Property(e => e.ChildSupplier)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Child_Supplier");

                entity.Property(e => e.ColorSequence)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Color_Sequence");

                entity.Property(e => e.CommercialRating)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Commercial_Rating");

                entity.Property(e => e.Density)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.DivisionCollection)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Division_Collection");

                entity.Property(e => e.DivisionList)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Division_List");

                entity.Property(e => e.DivisionProductName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Division_Product_Name");

                entity.Property(e => e.DivisionRating)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Division_Rating");

                entity.Property(e => e.DivisonStyleNameAndColor)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Divison_Style_Name_and_Color");

                entity.Property(e => e.DurabilityRating)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Durability_Rating");

                entity.Property(e => e.DyeMethod)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Dye_Method");

                entity.Property(e => e.FaceLabelPlate)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Face_Label_Plate_#");

                entity.Property(e => e.FaceWeight)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Face_Weight");

                entity.Property(e => e.FhaClass)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("FHA_Class");

                entity.Property(e => e.FhaLab)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("FHA_Lab");

                entity.Property(e => e.FhaType)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("FHA_Type");

                entity.Property(e => e.FiberBrand)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Fiber_Brand");

                entity.Property(e => e.FiberCompany)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Fiber_Company");

                entity.Property(e => e.Flammability)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.Gauge)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.GreenNaturalSustained)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Green_Natural_Sustained");

                entity.Property(e => e.GreenRecyclableContent)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Green_Recyclable_Content");

                entity.Property(e => e.GreenRecycledContent)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Green_Recycled_Content");

                entity.Property(e => e.IaqNumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("IAQ_Number");

                entity.Property(e => e.InstallationPattern)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Installation_Pattern");

                entity.Property(e => e.IsFhaCertified)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Is_FHA_Certified");

                entity.Property(e => e.IsGreenRated)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Is_Green_Rated");

                entity.Property(e => e.IsWebProduct)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Is_Web_Product");

                entity.Property(e => e.JobNumberBl)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Job_Number_BL");

                entity.Property(e => e.JobNumberFl)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Job_Number_FL");

                entity.Property(e => e.Layout)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.MadeIn)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Made_In");

                entity.Property(e => e.ManufacturerFeeler)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Manufacturer_Feeler");

                entity.Property(e => e.ManufacturerProductColorId)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Manufacturer_Product_Color_ID");

                entity.Property(e => e.ManufacturerSkuNumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Manufacturer_SKU_Number");

                entity.Property(e => e.Match)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.MatchLength)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Match_Length");

                entity.Property(e => e.MatchWidth)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Match_Width");

                entity.Property(e => e.MerchColorName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merch_Color_Name");

                entity.Property(e => e.MerchColorNumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merch_Color_Number");

                entity.Property(e => e.MerchColorStartDate)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merch_Color_Start_Date");

                entity.Property(e => e.MerchProdStartDate)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merch_Prod_Start_Date");

                entity.Property(e => e.MerchandiseBrand)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merchandise_Brand");

                entity.Property(e => e.MerchandisedProductColorId)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merchandised_Product_Color_ID");

                entity.Property(e => e.MerchandisedProductId)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merchandised_Product_ID");

                entity.Property(e => e.MerchandisedSkuNumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Merchandised_SKU_Number");

                entity.Property(e => e.MfgColorName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Mfg_Color_Name");

                entity.Property(e => e.MfgColorNumber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Mfg_Color_Number");

                entity.Property(e => e.NbsSmokeDensityAstme662)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("NBS_Smoke_Density_ASTME662");

                entity.Property(e => e.NumberOfColors)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Number_of_Colors");

                entity.Property(e => e.OutputFl).HasColumnName("Output_FL");

                entity.Property(e => e.PercentBcf)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Percent_BCF");

                entity.Property(e => e.PercentSpun)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Percent_Spun");

                entity.Property(e => e.PileHeight)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Pile_Height");

                entity.Property(e => e.PileLine)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Pile_Line");

                entity.Property(e => e.Plate)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Plate_#");

                entity.Property(e => e.PrimaryDisplay)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Primary_Display");

                entity.Property(e => e.PrimaryFiber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Primary_Fiber");

                entity.Property(e => e.PrimaryFiberPercentage)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Primary_Fiber_Percentage");

                entity.Property(e => e.ProductClass)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Product_Class");

                entity.Property(e => e.ProductType)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Product_Type");

                entity.Property(e => e.Program)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.RadiantPanelAstme648)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Radiant_Panel_ASTME648");

                entity.Property(e => e.SampleBox)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Box");

                entity.Property(e => e.SampleBoxAvailability)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Box_Availability");

                entity.Property(e => e.SampleBoxEnabled)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Box_Enabled");

                entity.Property(e => e.SampleId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Sample_ID");

                entity.Property(e => e.SecondFiber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Second_Fiber");

                entity.Property(e => e.SecondFiberPercentage)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Second_Fiber_Percentage");

                entity.Property(e => e.SizeName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Size_Name");

                entity.Property(e => e.SoilTreatment)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Soil_Treatment");

                entity.Property(e => e.StainTreatment)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Stain_Treatment");

                entity.Property(e => e.StaticAatcc134)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Static_AATCC134");

                entity.Property(e => e.Status)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.StatusFl)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Status_FL");

                entity.Property(e => e.Stitches)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.StyleColorComboFromAp)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Style_Color_Combo_From_AP");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Supplier_Name");

                entity.Property(e => e.SupplierProductName)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Supplier_Product_Name");

                entity.Property(e => e.Taxonomy)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.ThirdFiber)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Third_Fiber");

                entity.Property(e => e.ThirdFiberPercentage)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Third_Fiber_Percentage");

                entity.Property(e => e.TotalWeight)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Total_Weight");

                entity.Property(e => e.YarnTwist)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasColumnName("Yarn_Twist");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.DivisionLabelName)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Division_Label_Name");

                entity.Property(e => e.DivisionLabelType)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Division_Label_Type");

                entity.Property(e => e.MerchandisedProductId)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Merchandised_Product_ID");

                entity.Property(e => e.SampleId)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_ID");
            });

            modelBuilder.Entity<Sample>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Sample");

                entity.Property(e => e.Binder)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Border)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.CharacterRatingByColor)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Character_Rating_by_Color");

                entity.Property(e => e.Feeler)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Msrp)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("MSRP");

                entity.Property(e => e.MsrpCanada)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("MSRP_Canada");

                entity.Property(e => e.MultipleColorLines)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Multiple_Color_Lines");

                entity.Property(e => e.OurPrice)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Our_Price");

                entity.Property(e => e.OurPriceCanada)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Our_Price_Canada");

                entity.Property(e => e.QuickShip)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Quick_Ship");

                entity.Property(e => e.RrpUs)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("RRP_US");

                entity.Property(e => e.SampleId)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_ID");

                entity.Property(e => e.SampleName)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Name");

                entity.Property(e => e.SampleNote)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Note");

                entity.Property(e => e.SampleSize)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Size");

                entity.Property(e => e.SampleType)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sample_Type");

                entity.Property(e => e.SampledColorSku)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sampled_Color_SKU");

                entity.Property(e => e.SampledWithMerchProductId)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sampled_With_Merch_Product_ID");

                entity.Property(e => e.SamplingColorDescription)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Sampling_Color_Description");

                entity.Property(e => e.SharedCard)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Shared_Card");

                entity.Property(e => e.SplitBoard)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Split_Board");

                entity.Property(e => e.TradeUp)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Trade_Up");

                entity.Property(e => e.WoodImaging)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("Wood_Imaging");
            });

            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Duration).HasMaxLength(255);

                entity.Property(e => e.MerchandisedProductId)
                    .HasMaxLength(255)
                    .HasColumnName("Merchandised_Product_ID");

                entity.Property(e => e.ProductWarrantyTypeCode)
                    .HasMaxLength(255)
                    .HasColumnName("Product_Warranty_Type_Code");

                entity.Property(e => e.Provider).HasMaxLength(255);

                entity.Property(e => e.SampleId)
                    .HasMaxLength(255)
                    .HasColumnName("Sample_ID");

                entity.Property(e => e.WarrantyPeriod)
                    .HasMaxLength(255)
                    .HasColumnName("Warranty_Period");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
