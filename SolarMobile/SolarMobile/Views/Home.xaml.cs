using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SolarMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }

        public void startButtonClicked(object sender, EventArgs e)
        {
            var mainPage = this.Parent.Parent as TabbedPage;
            mainPage.CurrentPage = mainPage.Children[1];
        }
    }
}