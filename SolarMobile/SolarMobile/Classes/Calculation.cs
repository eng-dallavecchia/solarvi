using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Globalization;

namespace SolarMobile.Views
{
    public class Calculation
    {
        public static double latitude { get; set; }
        public static double longitude { get; set; }
        public static string country { get; set; }
        public static string countryCode { get; set; }
        public static string province { get; set; }
        public static List<double> monthIrradiance { get; set; }
        public static List<double> angledMonthIrradiance { get; set; }
        public static List<double> generatedMonthPower { get; set; }
        public static double kwhFee { get; set; }
        public static double kwhConsumed { get; set; }
        public static double coverage { get; set; }
        public static double energyBill { get; set; }
        public static double minimumPower { get; set; }
        public static double totalCost { get; set; }
        public static double anualIncreasekwh { get; set; } = 0.05;


        public async static Task GetLocation()
        {     

            try
            {

                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {

                    Xamarin.Essentials.GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, timeout: TimeSpan.MaxValue);
                    location = await Geolocation.GetLocationAsync(request);
                    
                }


                if (location != null)
                {
                    Calculation.latitude = location.Latitude;
                    Calculation.longitude = location.Longitude;
                }

                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                
               
                
                if (placemark != null)
                {
                    Calculation.country = placemark.CountryName;
                    Calculation.province = placemark.AdminArea;
                    Calculation.countryCode = placemark.CountryCode;

                }

            }
            catch (Exception LocationException)
            {
                Console.WriteLine(LocationException.ToString());
            }

        }
       // public static void GetLocationIrradiance()
       // {
       //     var currentDate = DateTime.Now;
       //     int lastYear = currentDate.Year - 1;

       //string requestUrl =  "https://power.larc.nasa.gov/cgi-bin/v1/DataAccess.py?request=execute&identifier="
       //         +"SinglePoint&parameters=T2M,PS,ALLSKY_SFC_SW_DWN&startDate="
       //         +lastYear.ToString() +"&endDate="
       //         +lastYear.ToString() + "&userCommunity=SSE&tempAverage=INTERANNUAL&outputList=JSON,ASCII&lat="
       //         +Calculation.latitude.ToString() + "&lon="
       //         +Calculation.longitude.ToString() + "&user=anonymous";


       //  SunData.Rootobject sunJson =  SunData.DownloadJsonData<SunData.Rootobject>(requestUrl);

       //     var irradianceDictionary = sunJson.features.Last().properties.parameter.ALLSKY_SFC_SW_DWN;
       //     Calculation.monthIrradiance = new List<double>();

       //     foreach (var el in irradianceDictionary)
       //     {
       //         Calculation.monthIrradiance.Add(el.Value);
       //     }
       // }

        public static void SolarCalculus()
        {


            double solarTime = (longitude % 15) / 15;
            double dayAngleAtNoon = (solarTime / 24) * 360;

            double toRadians = Math.PI / 180;
            double[] delta = new double[365];
            double[] a = new double[365];
            double[] psi = new double[365];
            double[] thetaz = new double[365];
            double[] cos_incidence = new double[365];
            double[] monthCorrection = new double[365];
            double beta = Math.Abs(latitude);

            try
            {
                for (int i = 0; i < 365; i++)
                {
                    delta[i] = -23.45 * Math.Sin(toRadians * 360 / 365.25 * (((double)i + 1) - 264));
                    a[i] = 180 / (Math.PI) * Math.Asin(Math.Sin(toRadians * delta[i]) * Math.Sin(toRadians * latitude) + Math.Cos(toRadians * delta[i]) * Math.Cos(toRadians * latitude) * Math.Cos(toRadians * dayAngleAtNoon));
                    psi[i] = 180 / Math.PI * Math.Acos((Math.Sin(toRadians * a[i]) * Math.Sin(toRadians * latitude) - Math.Sin(toRadians * delta[i])) / (Math.Cos(toRadians * a[i]) * Math.Cos(toRadians * latitude)));
                    thetaz[i] = latitude - delta[i];
                    cos_incidence[i] =
                          Math.Cos(toRadians * delta[i]) * Math.Sin(toRadians * dayAngleAtNoon) * Math.Sin(toRadians * beta) * Math.Sin(toRadians * psi[i])
                        + Math.Cos(toRadians * delta[i]) * Math.Cos(toRadians * dayAngleAtNoon) * Math.Sin(toRadians * latitude) * Math.Sin(toRadians * beta) * Math.Cos(toRadians * psi[i])
                        - Math.Sin(toRadians * delta[i]) * Math.Cos(toRadians * latitude) * Math.Sin(toRadians * beta) * Math.Cos(toRadians * psi[i])
                        + Math.Cos(toRadians * delta[i]) * Math.Cos(toRadians * dayAngleAtNoon) * Math.Cos(toRadians * latitude) * Math.Cos(toRadians * beta)
                        + Math.Sin(toRadians * delta[i]) * Math.Sin(toRadians * latitude) * Math.Cos(toRadians * beta);

                    if (i <= 30)
                    {
                        monthCorrection[0] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 58)
                    {
                        monthCorrection[1] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 28);
                    }
                    else if (i <= 89)
                    {
                        monthCorrection[2] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 119)
                    {
                        monthCorrection[3] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 30);
                    }
                    else if (i <= 150)
                    {
                        monthCorrection[4] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 180)
                    {
                        monthCorrection[5] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 30);
                    }
                    else if (i <= 211)
                    {
                        monthCorrection[6] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 242)
                    {
                        monthCorrection[7] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 272)
                    {
                        monthCorrection[8] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 30);
                    }
                    else if (i <= 303)
                    {
                        monthCorrection[9] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }
                    else if (i <= 333)
                    {
                        monthCorrection[10] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 30);
                    }
                    else if (i <= 364)
                    {
                        monthCorrection[11] += cos_incidence[i] / (Math.Cos(toRadians * thetaz[i]) * 31);
                    }

                }

                angledMonthIrradiance = new List<double>();

                for (int j = 0; j < 12; j++)
                {
                    angledMonthIrradiance.Add(monthIrradiance[j] * monthCorrection[j]);
                }

                double kwhConsumedByDay = kwhConsumed / 30;
                double efficiency = 0.75;

                minimumPower = (Calculation.coverage/100)*(kwhConsumedByDay) / angledMonthIrradiance.Average();
                minimumPower = minimumPower / efficiency;


                generatedMonthPower = new List<double>();

                for(int i = 0; i < 12; i++)
                {
                    generatedMonthPower.Add(efficiency*minimumPower * angledMonthIrradiance[i] * 30);
                }

            }
            catch (Exception angledCorrectionException)
            {
                Console.WriteLine(angledCorrectionException.ToString());
            }
        }



        public Calculation()
        {
            
        }
    
    }


}

