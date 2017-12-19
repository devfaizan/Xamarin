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
using PDFToImage.Droid.Models;

namespace PDFToImage.Droid
{
    [Activity(Label = "PDFToImage.Droid", MainLauncher = true)]
    public class MainActivity : Activity, IWebViewClientActivity, IValueCallback
    {
        #region --- Controls ---
        WebView webview;
        WebView webview2;
        LinearLayout mainLayout;
        ProgressDialog progressDialog;
        Button btnConvert;
        EditText txtUrl;
        #endregion

        #region --- Fields ---
        readonly string gViewURL = "https://docs.google.com/gview?embedded=true&url=";
        string url;
        #endregion

        #region --- Activity Events/Methods ---
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            webview = new WebView(this);
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            txtUrl = FindViewById<EditText>(Resource.Id.editText1);
            btnConvert = FindViewById<Button>(Resource.Id.button1);
            progressDialog = new ProgressDialog(this);

            // test pdfs
            var page1 = "http://unec.edu.az/application/uploads/2014/12/pdf-sample.pdf";
            var page2 = "http://www.menatelecom.com/media/document/pdfsample.pdf";
            var page3 = "https://www.antennahouse.com/XSLsample/pdf-v32/Sample-multi-pdf3.pdf";
            var page4 = "https://www.ets.org/Media/Tests/GRE/pdf/gre_research_validity_data.pdf";
            var page5 = "http://www.pdf995.com/samples/pdf.pdf";

            txtUrl.Text = page4;
            btnConvert.Click += BtnConvert_Click;
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            url = gViewURL + txtUrl.Text;

            if (webview != null)
                mainLayout.RemoveView(webview);

            webview = new WebView(this);

            mainLayout.AddView(webview, new LinearLayout.LayoutParams(1354, 100000));

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

        /// <summary>
        /// Invokes when OnError is called on web view having ExtendedWebViewClient
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        public void OnError()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Invokes when OnWebViewPageFinshed is called on web view having ExtendedWebViewClient
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        public async void OnWebViewPageFinshed(WebView view, string url)
        {
            if (view.Progress == 100)
            {
                progressDialog.Dismiss();

                await LoadPDF();
            }
        }

        /// <summary>
        /// Invokes when EvaluateJavascript gets called
        /// </summary>
        /// <param name="result"></param>
        public async void OnReceiveValue(Java.Lang.Object result)
        {
            try
            {
                var json = result as Java.Lang.String;
                var str = result.ToString();
                var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonReturn>(str);
                if (jsonObject != null && !jsonObject.loaded && webview2 == null)
                    await LoadPDF();
                else
                {
                    if (webview2 == null)
                    {
                        mainLayout.RemoveView(webview);
                        webview2 = webview;
                        webview2.SetBackgroundColor(Color.White);
                        webview2.Settings.JavaScriptEnabled = true;
                        webview2.Settings.SetPluginState(WebSettings.PluginState.On);

                        int height = (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, Convert.ToInt32(jsonObject.result), this.Resources.DisplayMetrics);

                        mainLayout.AddView(webview2, new LinearLayout.LayoutParams(1354, height));
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        await CheckIfPDFRendered();

                    }
                    else if (jsonObject.loaded)
                    {
                        var image = webview2.ToImage();
                        image.SaveAndAddToGallery(Android.OS.Environment.DirectoryDownloads, Guid.NewGuid().ToString() + ".jpeg", this);
                        Toast.MakeText(this, "Image created added to library", ToastLength.Short).Show();
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
        #endregion

        #region --- Helper Methods ---
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
        #endregion
    }
}

