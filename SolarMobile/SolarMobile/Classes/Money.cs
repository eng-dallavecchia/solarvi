using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace SolarMobile.Views
{
    class Money
    {
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
            public string _base { get; set; }
            public string date { get; set; }
            public Dictionary<string,double> rates { get; set; }
        }

    }
}
