using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Valuation_Comparer.Models;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Web.Helpers;
using System.Collections;

namespace Valuation_Comparer.Controllers
{
    public class ValuationController : Controller
    {
        //
        // GET: /Valuation/

        public ActionResult Index()
        {
            return View(new ParcelSearchModel());
        }

        [HttpPost]
        public ActionResult Search(ParcelSearchModel model)
        {
            if (model.HouseNo != null && model.StreetName != null)
                ViewBag.Address = model.HouseNo + ((model.StreetDirection != null && model.StreetDirection.Length > 0) ? (" " + model.StreetDirection) : "") + " " + model.StreetName + ((model.Suffix != null && model.Suffix.Length > 0) ? (" " + model.Suffix) : "");
            else
                ViewBag.Address = "";
            ViewBag.Title = "Search near " + (("".Equals(ViewBag.Address)) ? "(empty)" : ViewBag.Address);
            ViewBag.ZIP = model.ZIP;
            ViewBag.ValuationType = (model.ValuationType.Equals("B")) ? "Land & Improvements (Building)" : ((model.ValuationType.Equals("L")) ? "Land" : "Improvements (Building)");
            List<ParcelModel> parcels = GetParcelsFromSearch(model);
            IEnumerable<ParcelModel> relatedParcels = parcels;

            if (model.PropertyClass.Equals("R") || model.FilterBySubdivision)
            {
                ParcelModel baseParcel = new ParcelModel();
                var baseParcelQuery = from parcel in parcels
                                      where (parcel.PropertyAddress.ToLower().Contains(ViewBag.Address.ToLower()))
                                      select parcel;
                foreach (ParcelModel firstParcel in baseParcelQuery)
                {
                    baseParcel = firstParcel;
                }
                relatedParcels = parcels.Where(a => a.SubdivisionNo == baseParcel.SubdivisionNo);
            }
            else
            {
                relatedParcels = parcels.Where(a => 1==1);
            }

            ViewBag.Rows = relatedParcels.Count();
            
            relatedParcels = relatedParcels.OrderBy(p => p.CurrentValue);

            if (relatedParcels.Count() > 0)
                ViewBag.ValuationByStreetChartSrc = ValuationByStreetChart(relatedParcels);

            return View(relatedParcels.ToList());
        }

        public String ValuationByStreetChart(IEnumerable<ParcelModel> parcels)
        {
            IList DataSets = new ArrayList();
            //    new { X = 0, Y = 1 },
            //    new { X = 1, Y = 2}
            //};

            for(int i = 0; i < parcels.Count(); i++) 
            {
                ParcelModel parcel = parcels.ElementAt(i);
                DataSets.Add(new { X = i, Y = parcel.CurrentValue });
            }

            var chart = new Chart(600, 400, ChartTheme.Green)
                .AddTitle("Valuation by Street")
                .DataBindTable(DataSets, "X");

            String base64image = Convert.ToBase64String(chart.GetBytes());

            //data:image/png;base64,

            return "data:image/png;base64," + base64image;
        }

        private List<ParcelModel> GetParcelsFromSearch(ParcelSearchModel searchModel)
        {
            Decimal? mean = 0;
            Decimal? median = 0;
            List<ParcelModel> parcels = new List<ParcelModel>();
            SqlConnection conn = new SqlConnection
            ("Data Source=sql2k803.discountasp.net;Initial Catalog=SQL2008_566895_casestudy;User ID=hackomaha;Password=Hack123");
            SqlCommand sproc = new SqlCommand("usp_get_DC_Assessor_Data", conn);

            sproc.CommandType = CommandType.StoredProcedure;
            SqlParameter pinParam = new SqlParameter("@Pin", (searchModel.PIN == null) ? "" : searchModel.PIN);
            SqlParameter housenoParam = new SqlParameter("@house#", (searchModel.HouseNo == null) ? "" : searchModel.HouseNo);
            SqlParameter streetnameParam = new SqlParameter("@Street", (searchModel.StreetName == null) ? "" : searchModel.StreetName);
            SqlParameter suffixParam = new SqlParameter("@Suffix", (searchModel.Suffix == null) ? "" : searchModel.Suffix);
            SqlParameter streetdirectionParam = new SqlParameter("@dir", (searchModel.StreetDirection == null) ? "" : searchModel.StreetDirection);
            SqlParameter zipParam = new SqlParameter("@zip", (searchModel.ZIP == null) ? "" : searchModel.ZIP);
            SqlParameter classParam = new SqlParameter("@class", (searchModel.PropertyClass == null) ? "" : searchModel.PropertyClass);
            SqlParameter lowerParam = new SqlParameter("@lower", searchModel.LowerLimit);
            SqlParameter upperParam = new SqlParameter("@upper", searchModel.UpperLimit);
            SqlParameter valu_catParam = new SqlParameter("@valu_cat", (searchModel.ValuationType == null) ? "" : searchModel.ValuationType);

            // pin, houseno, street, suffix, dir, zip, class, lower, upper, valu_cat

            sproc.Parameters.Add(pinParam);
            sproc.Parameters.Add(housenoParam);
            sproc.Parameters.Add(streetnameParam);
            sproc.Parameters.Add(suffixParam);
            sproc.Parameters.Add(streetdirectionParam);
            sproc.Parameters.Add(zipParam);
            sproc.Parameters.Add(classParam);
            sproc.Parameters.Add(lowerParam);
            sproc.Parameters.Add(upperParam);
            sproc.Parameters.Add(valu_catParam);

            conn.Open();

            SqlDataReader myReader = sproc.ExecuteReader();
            while (myReader.Read())
            {
                ParcelModel parcel = new ParcelModel();
                parcel.PIN = myReader.GetInt64(0).ToString();
                parcel.OwnerName = myReader.GetString(1);
                parcel.PropertyAddress = myReader.GetString(2);
                parcel.MinimumValue = getDecimal(myReader, 3);
                parcel.LowerPercent = getDecimal(myReader, 4);
                parcel.Mean = getDecimal(myReader, 5);
                mean = parcel.Mean;
                parcel.Median = getDecimal(myReader, 6);
                median = parcel.Median;
                parcel.CurrentValue = getInt32(myReader, 7);
                parcel.UpperPercent = getDecimal(myReader, 8);
                parcel.MaximumValue = getDecimal(myReader, 9);
                parcel.AssessorURL = myReader.GetString(10);
                parcel.TreasurerURL = myReader.GetString(11);
                parcel.X_COORD = getDecimal(myReader, 12);
                parcel.Y_COORD = getDecimal(myReader, 13);
                parcel.NumBuildings = getInt32(myReader, 14);
                parcel.BldgSqFt = getDecimal(myReader, 15);
                parcel.SubdivisionNo = getInt32(myReader, 16);
                parcel.DollarsPerBldgSqFt = getDecimal(myReader, 17);
                parcel.DollarsPerLandSqFt = getDecimal(myReader, 18);
                parcel.Acres = getDecimal(myReader, 19);
                parcel.LandSqFt = getDecimal(myReader, 20);

                if (searchModel.ValuationType.Equals("I"))
                {
                    parcel.SqFt = parcel.BldgSqFt;
                    parcel.DollarsPerSqFt = parcel.DollarsPerBldgSqFt;
                }
                else
                {
                    parcel.SqFt = parcel.LandSqFt;
                    parcel.DollarsPerSqFt = parcel.DollarsPerLandSqFt;
                }
                parcels.Add(parcel);
            };
            ViewBag.Parcels = parcels.Take(500);
            ViewBag.ParcelCount = parcels.Count;
            ViewBag.Mean = mean;
            ViewBag.Median = median;
            //ViewBag.JsonOutput = convertParcelsToJson(parcels);
            myReader.Close();
            return parcels;
        }

        /// <summary>
        /// Return decimal value if it exists, or return null
        /// </summary>
        /// <param name="column">SQL table reader column</param>
        /// <returns>decimal result or null</returns>
        private Decimal? getDecimal(SqlDataReader reader, int column)
        {
            Decimal? rval = null;
            decimal output = -1;
            Boolean isnull = reader[column].GetType().Name.Equals("DBNull");
            
            if (isnull)
                return null;
            
            Decimal.TryParse(reader.GetDecimal(column).ToString(), out output);
            rval = output;
            
            
            return rval;
        }

        /// <summary>
        /// Return integer value if it exists, or return null
        /// </summary>
        /// <param name="column">SQL table reader column</param>
        /// <returns>int32 result or null</returns>
        private Int32? getInt32(SqlDataReader reader, int column)
        {
            Int32? rval = null;
            int output = -1;
            Boolean isnull = reader[column].GetType().Name.Equals("DBNull");

            if (isnull)
                return null;

            Int32.TryParse(reader.GetInt32(column).ToString(), out output);
            if (output >= 0)
                rval = output;
            

            return rval;
        }

        private String convertParcelsToJson(List<ParcelModel> parcels)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            String rval = JsonConvert.SerializeObject(parcels, Formatting.None, jsonSerializerSettings);
            
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("Parcels");
                jsonWriter.WriteStartArray();
                foreach (ParcelModel parcel in parcels)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("Parcel");
                    sb.Append(JsonConvert.SerializeObject(parcel));
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WriteEndObject();
                jsonWriter.Close();
            }
            return sb.ToString();
        }
    }
}
