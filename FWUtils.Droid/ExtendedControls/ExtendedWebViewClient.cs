using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using FWUtils.Droid.Interfaces;

namespace FWUtils.Droid.ExtendedControls
{
    /// <summary>
    /// Provides the functionality to customize on page finished with activity of type IWebViewClientActivity
    /// </summary>
    public class ExtendedWebViewClient : WebViewClient
    {
        IWebViewClientActivity _activity;

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            return true;
        }
        public ExtendedWebViewClient(IWebViewClientActivity activity)
        {
            _activity = activity;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            _activity.OnWebViewPageFinshed(view, url);
        }

       
    }
}