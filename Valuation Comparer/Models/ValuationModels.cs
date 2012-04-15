using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Valuation_Comparer.Models
{
    
    public class ParcelModel
    {
        [Display(Name = "PIN")] // string
        public String PIN { get; set; }

        [Display(Name = "Owner Name")] // string
        public String OwnerName { get; set; }

        [Required]
        [Display(Name = "Property Address")] // string
        public String PropertyAddress { get; set; }

        [Display(Name = "Minimum Value")] // Decimal
        public Decimal? MinimumValue { get; set; }

        [Display(Name = "Lower Percent")] // Decimal
        public Decimal? LowerPercent { get; set; }

        [Display(Name = "Mean")] // Decimal
        public Decimal? Mean { get; set; }

        [Display(Name = "Median")] // Decimal
        public Decimal? Median { get; set; }

        [Display(Name = "Current Value")] // Int32
        public Int32? CurrentValue { get; set; }

        [Display(Name = "Upper Percent")] // Decimal
        public Decimal? UpperPercent { get; set; }

        [Display(Name = "Maximum Value")] // Decimal
        public Decimal? MaximumValue { get; set; }

        [Display(Name = "X Coordinate")]
        public Decimal? X_COORD { get; set; }

        [Display(Name = "Y Coordinate")]
        public Decimal? Y_COORD { get; set; }

        [Display(Name = "Assessor URL")]
        public String AssessorURL { get; set; }

        [Display(Name = "Treasurer URL")]
        public String TreasurerURL { get; set; }

        [Display(Name = "Land Square Feet")]
        public Decimal? LandSqFt { get; set; }

        [Display(Name = "Building Square Foot")]
        public Decimal? BldgSqFt { get; set; }

        [Display(Name = "Display Square Foot")]
        public Decimal? SqFt { get; set; }

        [Display(Name = "$ Per Building Square Foot")]
        [DataType(DataType.Currency)]
        public Decimal? DollarsPerBldgSqFt { get; set; }

        [Display(Name = "Acres")]
        public Decimal? Acres { get; set; }

        [Display(Name = "$ Per Acre Sq Ft")]
        [DataType(DataType.Currency)]
        public Decimal? DollarsPerLandSqFt { get; set; }

        [Display(Name = "Display $ Per Sq Ft")]
        [DataType(DataType.Currency)]
        public Decimal? DollarsPerSqFt { get; set; }

        [Display(Name = "Number of Buildings")]
        public Int32? NumBuildings { get; set; }

        [Display(Name = "Subdivision Number")]
        public Int32? SubdivisionNo { get; set; }
    }

    public class ParcelSearchModel
    {
        public static List<String> ValidStreetDirections = new List<String>() { 
            "", "N", "S", "E", "W" 
        }; 
        public static List<String> ValidStreetSuffixes = new List<String>() { 
            "", "AV", "BLVD", "CR", "CT", "DR", "DR-E", "DR-S", "HWY", "LANE", "PLAZA", "PL", "PT", "PKWY", "PKWY-S", "PKWY-N", "RD", "ST", "ST-E", "ST-S", "TRAIL", "TERRACE"
        };

        public static List<String> ValidPropertyTypes = new List<String>() { 
            "R:Residential", "C:Commercial", "I:Industrial"
        };

        public static List<String> ValidValueCategories = new List<String>() {
            "B:Both", "L:Land", "I:Improvements"
        };

        public static IEnumerable<SelectListItem>
            PropertyTypes = from x in ValidPropertyTypes
                               select new SelectListItem
                               {
                                   Selected = false,
                                   Text = x.Split(':')[1],
                                   Value = x.Split(':')[0]
                               };

        public static IEnumerable<SelectListItem> 
            StreetDirections = from x in ValidStreetDirections
                                select new SelectListItem {
                                    Selected = false,
                                    Text = x,
                                    Value = x
        };

        public static IEnumerable<SelectListItem>
            StreetSuffixes = from x in ValidStreetSuffixes
                               select new SelectListItem
                               {
                                   Selected = false,
                                   Text = x,
                                   Value = x
                               };

        public static IEnumerable<SelectListItem>
            ValueCategories = from x in ValidValueCategories
                             select new SelectListItem
                             {
                                 Selected = false,
                                 Text = x.Split(':')[1],
                                 Value = x.Split(':')[0]
                             };

        /// <summary>
        /// Search model
        /// Defaults to upper limit of 80, lower limit of 20
        /// </summary>
        public ParcelSearchModel()
        {
            this.PIN = "";
            this.HouseNo = "";
            this.StreetName = "";
            this.StreetDirection = "";
            this.Suffix = "";
            this.ZIP = "";
            this.PropertyClass = "";
            this.ValuationType = "";
            this.UpperLimit = 80;
            this.LowerLimit = 20;
            this.FilterBySubdivision = true;
        }

        [Display(Name="PIN")]
        public String PIN { get; set; }
     
        [Required]
        [Display(Name="House #")]
        [DataType(DataType.Text)]
        public String HouseNo { get; set; }

        [Required]
        [Display(Name = "Street Name")]
        [DataType(DataType.Text)]
        public String StreetName { get; set; }

        [Display(Name = "Street Direction")]
        public String StreetDirection { get; set; }

        [Display(Name = "Suffix")]
        [DataType(DataType.Text)]
        public String Suffix { get; set; }

        [Display(Name = "ZIP")]
        [DataType(DataType.Text)]
        public String ZIP { get; set; }
        
        [Display(Name = "Property Class")]
        public String PropertyClass { get; set; }

        [Required]
        [Display(Name = "Upper Limit")]
        [DataType(DataType.Text)]
        [Range(1, 100)]
        public int UpperLimit { get; set; }

        [Required]
        [Display(Name = "Lower Limit")]
        [DataType(DataType.Text)]
        [Range(1, 100)]
        public int LowerLimit { get; set; }

        [Display(Name="Valuation Type")]
        public String ValuationType { get; set; }

        [Display(Name="Restrict to parcel subdivision")]
        public Boolean FilterBySubdivision { get; set; }
    }
}