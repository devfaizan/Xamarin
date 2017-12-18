using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using System.Threading.Tasks;
using System;
using FWUtils.Droid.Extensions;
using FWUtils.Droid.ExtendedControls;
using FWUtils.Droid.Interfaces;
using System.IO;
using Java.Lang;
using Android.Graphics;
using PDFToImage.Droid.Scripts;

namespace PDFToImage.Droid
{
    [Activity(Label = "PDFToImage.Droid", MainLauncher = true)]
    public class MainActivity : Activity, IWebViewClientActivity, IValueCallback
    {
        WebView webview;
        WebView webview2;
        LinearLayout mainLayout;
        ProgressDialog progressDialog;
        readonly string url = "https://docs.google.com/gview?embedded=true&url=http://altwebtest01.zaifworks.com/CertDownload.aspx?code=cert-10075-99ZH";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            webview = new WebView(this);
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            mainLayout.AddView(webview, new LinearLayout.LayoutParams(1354, 100000));

            progressDialog = new ProgressDialog(this);

            if (webview != null)
            {
                webview.Settings.JavaScriptEnabled = true;
                webview.Settings.SetPluginState(WebSettings.PluginState.On);
                webview.SetWebViewClient(new ExtendedWebViewClient(this));

                webview.LoadUrl(url);
                progressDialog.SetTitle("Please wait...");
                progressDialog.Show();
            }
        }

        public void OnError()
        {
            //throw new NotImplementedException();
        }

        public async void OnWebViewPageFinshed(WebView view, string url)
        {
            if (view.Progress == 100)
            {
                progressDialog.Dismiss();

                await LoadPDF();
            }
        }

        private async Task LoadPDF()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            webview.EvaluateJavascript("javascript: (function(){return { loaded: " + GoogleGViewHelperScripts.ScriptGViewLoaded + ", result: " + GoogleGViewHelperScripts.ScriptGetClientHeight + " }})()", this);
        }

        private async Task RenderPDF()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            webview.EvaluateJavascript("javascript: (function(){return { loaded: " + GoogleGViewHelperScripts.ScriptScrollBottom + ", result: " + GoogleGViewHelperScripts.ScriptGetClientHeight + " }})()", this);
        }

        private async Task CheckIfPDFRendered()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            webview2.EvaluateJavascript("javascript: (function(){return { loaded: " + GoogleGViewHelperScripts.ScriptCheckPDFRendered + ", result: " + GoogleGViewHelperScripts.ScriptRemoveBar + " }})()", this);
        }

        private async Task GetPDFHtml()
        {
            webview.EvaluateJavascript("javascript: (function(){return { loaded: " + GoogleGViewHelperScripts.ScriptGViewLoaded + ", result: " + GoogleGViewHelperScripts.ScriptGetClientHeight + " }})()", this);
        }

        public async void OnReceiveValue(Java.Lang.Object result)
        {
            try
            {
                var json = result as Java.Lang.String;
                var str = result.ToString();
                JsonReturn jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonReturn>(str);
                if (!jsonObject.loaded && webview2 == null)
                    await LoadPDF();
                else
                {
                    if (webview2 == null)
                    {
                        mainLayout.RemoveViewAt(0);
                        webview2 = webview;
                        webview2.SetBackgroundColor(Color.White);
                        webview2.Settings.JavaScriptEnabled = true;
                        webview2.Settings.SetPluginState(WebSettings.PluginState.On);
                        mainLayout.AddView(webview2, new LinearLayout.LayoutParams(1354, Convert.ToInt32(jsonObject.result)));
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        await CheckIfPDFRendered();

                    }
                    else if (jsonObject.loaded)
                    {
                        var image = webview2.ToImage();
                        image.SaveAndAddToGallery(Android.OS.Environment.DirectoryDownloads, Guid.NewGuid().ToString() + ".png", this);
                        Toast.MakeText(this, "Image created added to library", ToastLength.Short);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        await CheckIfPDFRendered();
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debugger.Log(0, "re", ex.ToString());
            }
        }


        public class JsonReturn
        {
            public bool loaded;
            public string result;
        }
    }
}

