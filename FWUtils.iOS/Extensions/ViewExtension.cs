using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace FWUtils.iOS.Extensions
{
    public static class ViewExtension
    {
        public static void AdjustHeightAccordingToContent(this UIWebView webView)
        {
            var contentHeight = webView.ScrollView.ContentSize.Height;
            var webViewHeight = webView.Frame.Height;
            CGSize mWebViewTextSize = webView.SizeThatFits(new CGSize(1.0f, 1.0f));
            CGRect mWebViewFrame = webView.Frame;
            mWebViewFrame = new CGRect(webView.Frame.X, webView.Frame.Y, webView.Frame.Width, mWebViewTextSize.Height);
            webView.Frame = mWebViewFrame;
            //Disable bouncing in webview
            foreach (var subview in webView.Subviews)
            {
                if (subview.GetType() == typeof(UIScrollView))
                    ((UIScrollView)subview).Bounces = false;
            }
        }

        public static UIImage ConvertToImage(this UIView view)
        {
            UIImage image;
            UIGraphics.BeginImageContextWithOptions(view.Frame.Size, false, 0.0f); // This will produce a fine quality image.
            view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }

        public static void Share(this UIImage image)
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
    }
}
