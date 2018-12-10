using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Net;
using System.Xml;
using System.IO;
using InterviewTest.Models;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace InterviewTest.Controllers.Home
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BannerTrackingReport()
        {
            // EXERCISE #1 CODE HERE
            // Rest of work done in BannerTrackingReport.cshtml and Banner.cs
            Banner[] tempBanners = null;
            SqlConnection conn = new SqlConnection();

            //Actual Connection String
            conn.ConnectionString = "Password=Wp12739#;" +
                                    "Persist Security Info=True;" +
                                    "User ID=webapp-iis-617;" +
                                    "Initial Catalog=webapp;" +
                                    "Data Source=CL-CGC-4SQ01";

            //Local Connection String
            //conn.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=webapp;Integrated Security=True";

            var query = "SELECT Banner.BannerId, Banner.Name, Banner.Link, Banner.Image, BannerTrack.ImpressionCount, BannerTrack.ClickCount, BannerTrack.CreateDate" +
                        " FROM [dbo].[Banner] Banner" +
                        " JOIN [dbo].[BannerTracking] BannerTrack" +
                            " ON Banner.BannerId = BannerTrack.BannerId" +
                        " WHERE BannerTrack.CreateDate >= '2015-11-18'" +
                        " AND BannerTrack.CreateDate <= '2015-11-24'" +
                        " ORDER BY Banner.Name;";
            using (var command = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var sqlReader = command.ExecuteReader())
                {
                    List<Banner> tempList = new List<Banner>();
                    while (sqlReader.Read())
                    {
                        if (sqlReader.HasRows)
                        {
                            tempList.Add(new Banner
                            {
                                BannerId = sqlReader.GetInt32(0),
                                BannerName = sqlReader.GetString(1),
                                BannerLink = sqlReader.GetString(2) + "/" + sqlReader.GetString(3),
                                ImpressionCount = sqlReader.GetInt64(4),
                                ClickCount = sqlReader.GetInt64(5)
                            });
                        }
                        tempBanners = tempList.ToArray();
                    }
                }
            }

            string currentBanner = "";
            List<Banner> model = new List<Banner>();
            long totalImpression = 0;
            long totalClick = 0;
            for (int i = 0; i < tempBanners.Length; i++)
            {
                if (currentBanner != tempBanners[i].BannerName)
                {
                    currentBanner = tempBanners[i].BannerName;
                    if (i != 0)
                    {
                        model.Add(new Banner
                        {
                            BannerId = tempBanners[i - 1].BannerId,
                            BannerName = tempBanners[i - 1].BannerName,
                            BannerLink = tempBanners[i - 1].BannerLink,
                            ImpressionCount = totalImpression,
                            ClickCount = totalClick
                        });
                        totalImpression = tempBanners[i].ImpressionCount;
                        totalClick = tempBanners[i].ClickCount;
                    }
                }
                else
                {
                    totalImpression += tempBanners[i].ImpressionCount;
                    totalClick += tempBanners[i].ClickCount;
                }
                if (i == tempBanners.Length)
                {
                    model.Add(new Banner
                    {
                        BannerId = tempBanners[i].BannerId,
                        BannerName = tempBanners[i].BannerName,
                        BannerLink = tempBanners[i].BannerLink,
                        ImpressionCount = totalImpression,
                        ClickCount = totalClick
                    });
                }
            }
            ViewBag.list = model;

            return View();
        }

        public HttpWebRequest SOAPWebRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://ws-design.idevdesign.net/hotels.asmx");
            request.Headers.Add(@"SOAPAction:http://ws.design.americaneagle.com/GetHotel;");
            request.ContentType = "application/soap+xml;charset=\"utf-8\"";
            request.Accept = "text/xml";
            request.Method = "POST";
            return request;
        }

        public static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelope, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelope.Save(stream);
            }
        }

        public FinalResult InvokeRequest(int hotelId)
        {
            HttpWebRequest request = SOAPWebRequest();

            XmlDocument SOAPRequestBody = new XmlDocument();

            SOAPRequestBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap12:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' 
                                 xmlns:xsd='http://www.w3.org/2001/XMLSchema' 
                                 xmlns:soap12='http://www.w3.org/2003/05/soap-envelope'>
                    <soap12:Header>
                        <HotelCredentials xmlns=""http://ws.design.americaneagle.com"">
                            <username>aeTraining</username>
                            <password>ZZZ</password>
                        </HotelCredentials>
                    </soap12:Header>
                    <soap12:Body>
                        <GetHotel xmlns = 'http://ws.design.americaneagle.com'>
                            <hotelId>" + hotelId + @"</hotelId>
                        </GetHotel>
                    </soap12:Body>
                </soap12:Envelope>");

            InsertSoapEnvelopeIntoWebRequest(SOAPRequestBody, request);

            string serviceResult;

            using (WebResponse ServiceResponse = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(ServiceResponse.GetResponseStream()))
                {
                    serviceResult = reader.ReadToEnd();
                }
            }

            XDocument doc = XDocument.Parse(serviceResult);
            XNamespace ns = "http://ws.design.americaneagle.com";

            var tempObject = (from o in doc.Root.Descendants(ns + "GetHotelResult")
                              select new
                              {
                                  Name = (string)o.Element(ns + "Name"),
                                  AirportCode = (string)o.Element(ns + "AirportCode"),
                                  Address1 = (string)o.Element(ns + "Address1"),
                                  Address2 = (string)o.Element(ns + "Address2"),
                                  Address3 = (string)o.Element(ns + "Address3"),
                                  City = (string)o.Element(ns + "City"),
                                  StateProvince = (string)o.Element(ns + "StateProvince"),
                                  Country = (string)o.Element(ns + "Country"),
                                  PostalCode = (string)o.Element(ns + "PostalCode"),
                              }).FirstOrDefault();

            string addressFormater;

            if (tempObject.Address2 == null)
            {
                if(tempObject.Address3 == null)
                {
                    addressFormater = tempObject.Address1 + "\n"
                                    + tempObject.City + ", " + tempObject.StateProvince + " " + tempObject.Country + "\n"
                                    + tempObject.PostalCode;
                }
                else
                {
                    addressFormater = tempObject.Address1 + "\n"
                                    + tempObject.Address2 + "\n"
                                    + tempObject.City + ", " + tempObject.StateProvince + " " + tempObject.Country + "\n"
                                    + tempObject.PostalCode;
                }
            }
            else
            {
                addressFormater = tempObject.Address1 + "\n"
                                + tempObject.Address2 + "\n"
                                + tempObject.Address3 + "\n"
                                + tempObject.City + ", " + tempObject.StateProvince + " " + tempObject.Country + "\n"
                                + tempObject.PostalCode;
            }

            FinalResult finalResult = new FinalResult();

            finalResult.resultName = tempObject.Name;
            finalResult.resultAirportCode = tempObject.AirportCode;
            finalResult.resultAddress = addressFormater;

            
            return finalResult;
        }
        [HttpGet]
        public ActionResult HotelsWebService()
        {
            // EXERCISE #2 CODE HERE
            //Rest of work done in above 3 methods, in XMLResult.cs File in Model, and in HotelWebService.cshtml in Views/Home
            int hotelId = 105304;

            FinalResult hotelResult = new FinalResult();
            hotelResult = InvokeRequest(hotelId);

            ViewData["HotelResult"] = hotelResult;

            return View();
        }

        [HttpGet]
        public ActionResult CustomValidator()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult CustomValidatorPost([Bind(Include = "ID, state, textBox")] Validator valid)
        {
            //EXERCISE #3 CODE HERE
            //The Model Validator.cs was created, along with some modification to the CustomValidator.cshtml file was done for this excercise
            string errorMessage = "Neither Field is filled out, therefore this form is invalid";
            string noErrorMessage = "No Errors!";

            if (valid.ID == 0)
            {
                ViewBag.Message = errorMessage;
                return View("CustomValidator");
            }
            else if (ModelState.IsValid)
            {
                ViewBag.Msg = noErrorMessage;
                return View("CustomValidator");
            }
            else
            {
                ViewBag.Msg = "";
                return View("CustomValidator");
            }
        }

        public ActionResult HelperSql()
        {
            return View();
        }
    }
}