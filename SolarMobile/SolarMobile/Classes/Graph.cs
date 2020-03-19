using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;

namespace SolarMobile.Views
{

    class Graph
    {
        public static List<double> cashFlux { get; set; }

        public static List<Microcharts.Entry> GenerationChart()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            string[] month = {"January", "February", "March",
                "April", "May", "June", "July", "August",
                "September", "October", "November", "December"};

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            for (int i = 0; i < 12; i++)
            {
                float value = (float)(Calculation.generatedMonthPower[i]);

                var entry = new Microcharts.Entry(value)
                {
                    Label = month[i],
                    ValueLabel = String.Format( culture, "{0:N0} kWh", value),
                    Color = SKColor.Parse("#7BCCB5")
                };

                entries.Add(entry);

            }

            return entries;
        }

        public static List<Microcharts.Entry> NewBillChart()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            string[] month = {"January", "February", "March",
                "April", "May", "June", "July", "August",
                "September", "October", "November", "December"};

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            for (int i = 0; i < 12; i++)
            {
                float value = (float)((Calculation.generatedMonthPower[i]-Calculation.kwhConsumed)*Calculation.kwhFee);

                var entry = new Microcharts.Entry(value)
                {
                    Label = month[i],
                    ValueLabel = String.Format(culture, "$ {0:N2}", value),
                    Color = SKColor.Parse("#7BCCB5")
                };

                entries.Add(entry);

            }

            return entries;
        }

        public static List<Microcharts.Entry> CashFluxChart()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            DateTime date = DateTime.Now;
            int year = date.Year;

            cashFlux = new List<double>();

            double baseAnualEconomy = Calculation.generatedMonthPower.Sum()*Calculation.kwhFee;

            cashFlux.Add(-Calculation.totalCost);

            for (int j = 0; j < 25; j++)
            {

                cashFlux.Add(
                    cashFlux.Last()
                    + baseAnualEconomy * Math.Pow((1 + Calculation.anualIncreasekwh), (double)j));

            }

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            for (int i = 0; i < 26; i++)
            {
                float value = (float)cashFlux[i];

                var entry = new Microcharts.Entry(value)
                {
                    Label = String.Format(culture, "{0:N0}", year+i),
                    ValueLabel = String.Format(culture, "$ {0:N2}", value),
                    Color = SKColor.Parse("#7BCCB5")
                };

                if (i % 5 == 0)
                {
                    entries.Add(entry);
                }

            }

            return entries;
        }


        public static List<Microcharts.Entry> TotalReturn()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            float value1 = Math.Abs((float)cashFlux.First());
            float value2 = Math.Abs((float)cashFlux.Last());

            var entry1 = new Microcharts.Entry(value1)
            {
                Label = String.Format(culture, "{0:N1} %", 100*(value1) / (value1 + value2)),
                ValueLabel = String.Format(culture, "$ {0:N2}", value1),
                Color = SKColor.Parse("#8D021F")
            };
            var entry2 = new Microcharts.Entry(value2)
            {
                Label = String.Format(culture, "{0:N1} %", 100*(value2) / (value1 + value2)),
                ValueLabel = String.Format(culture, "$ {0:N2}", value2),
                Color = SKColor.Parse("#7BCCB5")
            };

            entries.Add(entry1);
            entries.Add(entry2);

            return entries;


        }
    }
}
