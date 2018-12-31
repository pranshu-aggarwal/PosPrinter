using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PosPrinter.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            App.GetTextBytes += App_GetTextBytes;

            return base.FinishedLaunching(app, options);
        }

        byte[] App_GetTextBytes(string arg)
        {
            var uiImage = GetTextUIImage(arg);


            throw new NotImplementedException("Similar to android we have to find a way to convert with image in 2d byte array");
        }

        public static UIImage GetTextUIImage( string sText, int iFontSize = 14)
        {
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();

            UIGraphics.BeginImageContext(new CGSize(80, 50));
            sText.DrawString(new CGPoint(0, 0), UIFont.SystemFontOfSize(12));

            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return result;
        }

    }
}
