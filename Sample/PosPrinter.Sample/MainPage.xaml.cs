using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PosPrinter.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {

            try
            {

                var bytes = App.GetTextBytes("Hello World");
                Stream stream = new MemoryStream(bytes);
                TestImage.Source = ImageSource.FromStream(() => stream);
                //using (var printer = new Printer("192.168.1.222"))
                //{
                //    await printer.ConnectAsync();

                //    printer.SetAlignment(PrintAlignment.CenterAlignment);
                //    printer.PrintLine("Testing");

                //    var bytes = App.GetTextBytes("مرحبا");
                //    printer.SendBytes(bytes);
                //    printer.Reset();
                //    //printer.PrintArabic("A");

                //    printer.CutPaper();

                //    printer.Disconnect();
                //}
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Cancel");
            }
        }
    }
}
