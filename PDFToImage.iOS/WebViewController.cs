using System;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;

namespace PDFToImage.iOS
{
    public partial class WebViewController : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public WebViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Intercept URL loading to handle native calls from browser
            WebView.ShouldStartLoad += HandleShouldStartLoad;
            WebView.LoadFinished += WebView_LoadFinished;
            // Render the view from the type generated from RazorView.cshtml
            var model = new Model1 { Text = "Text goes here" };
            var template = new RazorView { Model = model };
            var page = template.GenerateString();

            // Load the rendered HTML into the view with a base URL 
            // that points to the root of the bundled Resources folder
            //WebView.LoadHtmlString (page, NSBundle.MainBundle.BundleUrl);
            WebView.LoadRequest(new NSUrlRequest(new NSUrl("http://www.pdf995.com/samples/pdf.pdf")));

            // Perform any additional setup after loading the view, typically from a nib.
        }

        private async void WebView_LoadFinished(object sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            var contentHeight = WebView.ScrollView.ContentSize.Height;
            var webViewHeight = WebView.Frame.Height;

            CGSize mWebViewTextSize = WebView.SizeThatFits(new CGSize(1.0f, 1.0f)); // Pass about any size
            CGRect mWebViewFrame = WebView.Frame;
            mWebViewFrame = new CGRect(WebView.Frame.X, WebView.Frame.Y, WebView.Frame.Width, mWebViewTextSize.Height);
            WebView.Frame = mWebViewFrame;

            //Disable bouncing in webview
            foreach (var subview in WebView.Subviews)
            {
                if (subview.GetType() == typeof(UIScrollView))
                    ((UIScrollView)subview).Bounces = false;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));

            UIImage image;
            UIGraphics.BeginImageContextWithOptions(WebView.Frame.Size, false, 0.0f); //This helps to produce fine quality image
            View.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            Share(image);
        }

        void Share(UIImage image)
        {
            var img = image;
            //var img = UIImage.LoadFromData(NSData.FromArray(imageData));

            var item = NSObject.FromObject(img);
            var activityItems = new[] { item };
            var activityController = new UIActivityViewController(activityItems, null);

            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            topController.PresentViewController(activityController, true, () => { });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        bool HandleShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            // If the URL is not our own custom scheme, just let the webView load the URL as usual
            const string scheme = "hybrid:";

            if (request.Url.Scheme != scheme.Replace(":", ""))
                return true;

            // This handler will treat everything between the protocol and "?"
            // as the method name.  The querystring has all of the parameters.
            var resources = request.Url.ResourceSpecifier.Split('?');
            var method = resources[0];
            var parameters = System.Web.HttpUtility.ParseQueryString(resources[1]);

            if (method == "UpdateLabel")
            {
                var textbox = parameters["textbox"];

                // Add some text to our string here so that we know something
                // happened on the native part of the round trip.
                var prepended = string.Format("C# says: {0}", textbox);

                // Build some javascript using the C#-modified result
                var js = string.Format("SetLabelText('{0}');", prepended);

                webView.EvaluateJavascript(js);
            }

            return false;
        }
    }
}

