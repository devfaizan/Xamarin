using System;
using Foundation;
using UIKit;
using System.Threading.Tasks;
using FWUtils.iOS.Extensions;

namespace PDFToImage.iOS
{
    public partial class PDFtoImageViewController : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public PDFtoImageViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            WebView.LoadFinished += WebView_LoadFinished;
            WebView.LoadRequest(new NSUrlRequest(new NSUrl("http://www.pdf995.com/samples/pdf.pdf")));
        }

        private async void WebView_LoadFinished(object sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            WebView.AdjustHeightAccordingToContent();

            await Task.Delay(TimeSpan.FromSeconds(2));

            var image = WebView.ConvertToImage();

            image.Share();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}

