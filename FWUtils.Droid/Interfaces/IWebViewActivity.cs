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

namespace FWUtils.Droid.Interfaces
{
    /// <summary>
    /// Consists of signature on activity used in ExtendedWebViewClient activity
    /// </summary>
    public interface IWebViewClientActivity
    {
        void OnWebViewPageFinshed(Android.Webkit.WebView view, string url);
        void OnError();
    }
}
