using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace SolarMobile.Views
{
    class Country
    {
        public string name { get; set; }
        public string countryCode { get; set; }
        public double totalCost { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string currency { get; set; }
        public double distance { get; set; }
        public double currencyValue { get; set; }



        public static Country GetCountryForFinances()
        {
            var money = Money.DownloadJsonData<Money.Rootobject>("https://api.exchangeratesapi.io/latest");
            money.rates.Add("EUR", 1);


            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("SolarMobile.Data.Costs.csv");
            string line = string.Empty;
            List<Country> countries = new List<Country>();

            using (var reader = new System.IO.StreamReader(stream))
            {
                while ((line = reader.ReadLine()) != null)
                {

                    string[] lineElement = line.Split(';');
                    Console.WriteLine(lineElement[0]);


                    Country lineCountry = new Country();
                    lineCountry.countryCode = lineElement[0];
                    lineCountry.name = lineElement[1];
                    lineCountry.totalCost = double.Parse(lineElement[2]);
                    lineCountry.latitude = double.Parse(lineElement[3]);
                    lineCountry.longitude = double.Parse(lineElement[4]);
                    lineCountry.currency = lineElement[5];
                    lineCountry.currencyValue = money.rates["USD"] / money.rates[lineCountry.currency];

                    countries.Add(lineCountry);

                }
            }


            foreach (Country country in countries)
            {
                if(Equals(country.countryCode, Calculation.countryCode))
                {
                    return country;
                }
            }

           foreach(Country country in countries)
            {
                

                country.distance = Xamarin.Essentials.Location.CalculateDistance(Calculation.latitude, Calculation.longitude,
                    country.latitude, country.longitude, 0);

                //Console.WriteLine("{0} : {1} : {2} : {3}",country.name.ToString(),country.latitude.ToString(),country.longitude.ToString(),country.distance.ToString());
                //double R = 6371e3;
                //double LA1 = country.latitude * Math.PI / 180;
                //double LA2 = Calculation.latitude * Math.PI / 180;
                //double dLA = (LA2 - LA1);
                //double dLON = (Calculation.longitude-country.longitude) * Math.PI / 180;


                //double a = Math.Sin(dLA / 2) * Math.Sin(dLA / 2)
                //    + Math.Cos(LA1) * Math.Cos(LA2)
                //    * Math.Sin(dLON / 2) * Math.Sin(dLON / 2);

                //double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                //country.distance = c * R;
            }

            double minDistance = 5e10;
            Country testCountry = new Country();
           foreach(Country country in countries)
            {
                if (country.distance < minDistance)
                {
                    minDistance = country.distance;
                    testCountry = country;
                }
            }


            Country nearestCountry = (from el in countries
                                      where el.distance == countries.Min(x => x.distance)
                                      select el).First();

        


            return nearestCountry;
        }



    }
}
