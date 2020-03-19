using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using SkiaSharp;
using Microcharts;
using System.IO;
using System.Reflection;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using System.Threading;

namespace SolarMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SolarPage : ContentPage
    {
        CultureInfo culture = CultureInfo.CurrentCulture;

        public SolarPage()
        {
            InitializeComponent();
            Geoinformation();
            GetLocationIrradiance();
        }


        public async void Geoinformation()
        {
            try
            {

                await Calculation.GetLocation();
                countryLabel.Text = Calculation.country;
                provinceLabel.Text = Calculation.province;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Couldn't access your location. Verify your permission settings.", "OK");
            }
        }
        public async Task GetLocationIrradiance()
        {

            try
            {
                await SunData.GetIrrad();
                //Calculation.GetLocationIrradiance();

                ghi.IsVisible = true;
                ghi.Text = ghi.Text.Replace("V1", Calculation.monthIrradiance[12].ToString());
            }
            catch
            {
                await DisplayAlert("Error", "Couldn't get your location irradiance. Connect to the internet or try again later.", "OK");
            }
        }



        public void energyBillUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                double.TryParse(energyBill.Text.Replace("$", ""), out double value);
                Calculation.energyBill = value;
                energyBill.Text = String.Format(culture, "$ {0:N2}", value);
            }

            catch (Exception parseEnergyBill)
            {
                DisplayAlert("Error", "Insert a valid number!", "OK");
            }
            try
            {
                double.TryParse(kwhFee.Text.Replace("$", ""), out double v1);
                double.TryParse(coverage.Text.Replace("%", ""), out double v2);
                GenerateReport();
            }
            catch (Exception) { }

        }

        public void kwhFeeUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                double.TryParse(kwhFee.Text.Replace("$", ""), out double value);
                Calculation.kwhFee = value;
                kwhFee.Text = String.Format(culture, "$ {0:N4}", value);

                Calculation.kwhConsumed = Calculation.energyBill / Calculation.kwhFee;
            }
            catch (Exception parseKwhFee)
            {
                DisplayAlert("Error", "Insert a valid number!", "OK");
            }
            try
            {
                double.TryParse(energyBill.Text.Replace("$", ""), out double v1);
                double.TryParse(coverage.Text.Replace("%", ""), out double v2);
                GenerateReport();
            }
            catch (Exception) { };
        }

        public void coverageUnfocused(object sender, FocusEventArgs e)
        {
            try
            {
                double.TryParse(coverage.Text.Replace("%", ""), out double value);
                Calculation.coverage = value;
                coverage.Text = String.Format(culture, "{0:N0} %", value);

            }
            catch (Exception parseKwhFee)
            {
                DisplayAlert("Error", "Insert a valid number!", "OK");
            }
            try
            {
                double.TryParse(energyBill.Text.Replace("$", ""), out double v1);
                double.TryParse(kwhFee.Text.Replace("$", ""), out double v2);
                GenerateReport();
            }
            catch (Exception) { };
        }
        public void GenerateReport()
        {
            try
            {
                Calculation.SolarCalculus();


                genChart.Chart = new BarChart() { Entries = Graph.GenerationChart() };
                billChart.Chart = new LineChart() { Entries = Graph.NewBillChart() };

                generationIntro.IsVisible = true;
                generationTitle.IsVisible = true;
                newBillIntro.IsVisible = true;
                newBillTitle.IsVisible = true;
            }
            catch (Exception)
            {
                DisplayAlert("Error", "It was not possible to generate a report", "OK");
            }

            try
            {

                Country countryForFinance = Country.GetCountryForFinances();

                Calculation.totalCost = (countryForFinance.totalCost * Calculation.minimumPower) / countryForFinance.currencyValue;

                cashFluxChart.Chart = new LineChart() { Entries = Graph.CashFluxChart() };

                cashFluxTitle.IsVisible = true;

                financesIntro.IsVisible = true;
                financesIntro.Text = financesIntro.Text.Replace("COUNTRY", countryForFinance.name);
                financesIntro.Text = financesIntro.Text.Replace("CURRENCY", countryForFinance.currency);
                financesIntro.Text = financesIntro.Text.Replace("TOTALCOST", String.Format(culture, "{0:N2}", Calculation.totalCost));

                pieChartTitle.IsVisible = true;
                pieChartIntro.IsVisible = true;
                pieChart.Chart = new DonutChart() { Entries = Graph.TotalReturn() };
        


            }

            catch (Exception) { }
        }
    }
}


        
