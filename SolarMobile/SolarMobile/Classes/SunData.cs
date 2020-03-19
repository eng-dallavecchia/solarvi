using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Globalization;

namespace SolarMobile.Views
{
    class SunData
    {


        public static async Task GetIrrad()
        {
            var currentDate = DateTime.Now;
            int lastYear = currentDate.Year - 1;

            string requestUrl = "https://power.larc.nasa.gov/cgi-bin/v1/DataAccess.py?request=execute&identifier="
                     + "SinglePoint&parameters=T2M,PS,ALLSKY_SFC_SW_DWN&startDate="
                     + lastYear.ToString() + "&endDate="
                     + lastYear.ToString() + "&userCommunity=SSE&tempAverage=INTERANNUAL&outputList=JSON,ASCII&lat="
                     + String.Format(CultureInfo.InvariantCulture,"{0:N4}", Calculation.latitude) + "&lon="
                     + String.Format(CultureInfo.InvariantCulture,"{0:N4}", Calculation.longitude) + "&user=anonymous";


            var httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            string content = await response.Content.ReadAsStringAsync();
            var sunJson = JsonConvert.DeserializeObject<Rootobject>(content);
            
            var irradianceDictionary = sunJson.features.Last().properties.parameter.ALLSKY_SFC_SW_DWN;
            Calculation.monthIrradiance = new List<double>();

            foreach (var el in irradianceDictionary)
            {
                Calculation.monthIrradiance.Add(el.Value);
            }
        }




        public static T DownloadJsonData<T>(string url) where T : new()
        {
            {
                using (var w = new WebClient())
                {
                    var json_data = string.Empty;
                    // attempt to download JSON data as a string
                    try
                    { 
                        json_data = w.DownloadString(url);
                    }
                    catch (Exception) { }
                    return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
                    // if string with JSON data is not empty, deserialize it to class and return its instance 
                }
            }
        }

        public class Rootobject
        {
            public Feature[] features { get; set; }
            public Header header { get; set; }
            public object[] messages { get; set; }
            public Outputs outputs { get; set; }
            public Parameterinformation parameterInformation { get; set; }
            public object[][] time { get; set; }
            public string type { get; set; }
        }

        public class Header
        {
            public string api_version { get; set; }
            public string endDate { get; set; }
            public string fillValue { get; set; }
            public string startDate { get; set; }
            public string title { get; set; }
        }

        public class Outputs
        {
            public string ascii { get; set; }
            public string json { get; set; }
        }

        public class Parameterinformation
        {
            public ALLSKY_SFC_SW_DWN ALLSKY_SFC_SW_DWN { get; set; }
            public PS PS { get; set; }
            public T2M T2M { get; set; }
        }

        public class ALLSKY_SFC_SW_DWN
        {
            public string longname { get; set; }
            public string units { get; set; }
        }

        public class PS
        {
            public string longname { get; set; }
            public string units { get; set; }
        }

        public class T2M
        {
            public string longname { get; set; }
            public string units { get; set; }
        }

        public class Feature
        {
            public Geometry geometry { get; set; }
            public Properties properties { get; set; }
            public string type { get; set; }
        }

        public class Geometry
        {
            public float[] coordinates { get; set; }
            public string type { get; set; }
        }

        public class Properties
        {
            public Parameter parameter { get; set; }
        }

        public class Parameter
        {
            public Dictionary<string, double> ALLSKY_SFC_SW_DWN { get; set; }
            public Dictionary<string, double> PS { get; set; }
            public Dictionary<string, double> T2M { get; set; }

        }

       

    }
}


